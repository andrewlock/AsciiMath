using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;

namespace AsciiMathParser;

/// <summary>
/// Parser for ASCIIMath expressions.
/// </summary>
/// <remarks>
/// The syntax for ASCIIMath in EBNF style notation is
///
/// expr = ( simp ( fraction | sub | super ) )+
/// simp = constant | paren_expr | unary_expr | binary_expr | text
/// fraction = '/' simp
/// super = '^' simp
/// sub =  '_' simp super?
/// paren_expr = lparen expr rparen
/// lparen = '(' | '[' | '{' | '(:' | '{:'
/// rparen = ')' | ']' | '}' | ':)' | ':}'
/// unary_expr = unary_op simp
/// unary_op = 'sqrt' | 'text'
/// binary_expr = binary_op simp simp
/// binary_op = 'frac' | 'root' | 'stackrel'
/// text = '"' [^"]* '"'
/// constant = number | symbol | identifier
/// number = '-'? [0-9]+ ( '.' [0-9]+ )?
/// symbol = /* any string in the symbol table */
/// identifier = [A-z]
///
/// ASCIIMath is parsed left to right without any form of operator precedence.
/// When parsing the 'constant' the parser will try to find the longest matching string in the symbol
/// table starting at the current position of the parser. If no matching string can be found the
/// character at the current position of the parser is interpreted as an identifier instead.
/// </remarks>
internal class Parser
{
    public static Node? Parse(string input)
        => ParseExpression(new Tokenizer(input), null);

    private static Node? ParseExpression(Tokenizer tokenizer, TokenType? closeParenType)
    {
        Node? expression = null;
        while (ParseIntermediateExpression(tokenizer, closeParenType) is { } i1)
        {
            var token1 = tokenizer.GetNext();
            if (token1 is { Type: TokenType.Infix, Symbol: Symbol.frac })
            {
                var i2 = ParseIntermediateExpression(tokenizer, closeParenType);
                expression = i2 is null
                    ? ConcatExpressions(expression, i1)
                    : ConcatExpressions(
                        expression,
                        new InfixOpNode(SymbolNode.From(token1), TryUnwrapParen(i1), TryUnwrapParen(i2)));
            }
            else if (token1.Type == TokenType.Eof)
            {
                expression = ConcatExpressions(expression, i1);
                break;
            }
            else
            {
                expression = ConcatExpressions(expression, i1);
                tokenizer.PushBack(token1);
                if (token1.Type == closeParenType)
                {
                    break;
                }
            }
        }

        return expression;
    }

    private static Node? ParseIntermediateExpression(Tokenizer tokenizer, TokenType? closeParenType)
    {
        var s = ParseSimpleExpression(tokenizer, closeParenType);
        Node? sub = null;
        Node? sup = null;

        var token1 = tokenizer.GetNext();
        if (token1.Type == TokenType.Infix)
        {
            switch (token1.Symbol)
            {
                case Symbol.sub:
                    sub = ParseSimpleExpression(tokenizer, closeParenType);
                    if (sub is not null)
                    {
                        var t2 = tokenizer.GetNext();
                        if (t2.Type == TokenType.Infix && t2.Symbol == Symbol.sup)
                        {
                            sup = ParseSimpleExpression(tokenizer, closeParenType);
                        }
                        else
                        {
                            tokenizer.PushBack(t2);
                        }
                    }

                    break;
                case Symbol.sup:
                    sup = ParseSimpleExpression(tokenizer, closeParenType);
                    break;
                default:
                    tokenizer.PushBack(token1);
                    break;
            }
        }
        else
        {
            tokenizer.PushBack(token1);
        }

        return (sub, sup) switch
        {
            ({ }, { }) => new SubSupNode(s, TryUnwrapParen(sub), TryUnwrapParen(sup)),
            ({ }, _) => new SubSupNode(s, TryUnwrapParen(sub), null),
            (_, { }) => new SubSupNode(s, null, TryUnwrapParen(sup)),
            (_, _) => s,
        };
    }

    private static Node? ParseSimpleExpression(Tokenizer tokenizer, TokenType? closeParenType)
    {
        var token1 = tokenizer.GetNext();
        return token1.Type switch
        {
            TokenType.LeftParen => HandleLeftParen(tokenizer, token1),
            TokenType.LeftRightParen => HandleLeftParen(tokenizer, token1),
            TokenType.RightParen => HandleRightParen(tokenizer, closeParenType, token1),
            TokenType.Unary => HandleUnary(tokenizer, closeParenType, token1),
            TokenType.Binary => HandleBinary(tokenizer, closeParenType, token1),
            TokenType.Eof => null,
            TokenType.Number => new NumberNode(token1.Text),
            TokenType.Text => new TextNode(token1.Text),
            TokenType.Identifier => new IdentifierNode(token1.Text),
            _ => SymbolNode.From(token1),
        };

        static Node HandleLeftParen(Tokenizer tokenizer, Token token1)
        {
            var closeWith = token1.Type == TokenType.LeftParen
                ? TokenType.RightParen
                : TokenType.LeftRightParen;

            var token2 = tokenizer.GetNext();
            if (token2.Type == closeWith)
            {
                return ParenNode.From(token1, null, token2);
            }
                
            tokenizer.PushBack(token2);

            var expression = ParseExpression(tokenizer, closeWith);

            token2 = tokenizer.GetNext();
            if (token2.Type == closeWith)
            {
                var paren = ParenNode.From(token1, expression, token2);
                return ConvertToMatrix(paren);
            }
                
            tokenizer.PushBack(token2);
            return token1.Type == TokenType.LeftRightParen
                ? ConcatExpressions(SymbolNode.From(token1), expression)
                : ParenNode.From(token1, expression, null);
            
            static Node ConvertToMatrix(ParenNode node)
            {
                IList<Node> rows;
                IList<Node> separators;
                if (node.Expression is SequenceNode seq)
                {
                    // odd are separators, even are rows
                    rows = new Node[(seq.Count + 1) / 2];
                    separators = new Node[seq.Count / 2];
                    var i = 0;
                    foreach (var child in seq)
                    {
                        var index = i / 2;
                        if ((i % 2) == 0)
                        {
                            rows[index] = child;
                        }
                        else
                        {
                            separators[index] = child;
                        }
                    }
                }
                else if (node.Expression is ParenNode)
                {
                    rows = [node.Expression];
                    separators = [];
                }
                else
                {
                    return node;
                }

                if(!(rows.Count >= 1
                   && rows.Count > separators.Count
                   && AreAllMatrixSeparator(separators)
                   && AreAllMatrixParenNodes(rows)))
                {
                    return node;
                }

                throw new NotImplementedException();
                // List<List<Node>> mappedRows = new();
                // foreach(var row in rows)
                // {
                //     // TODO: This doesn't look right?
                //     //var rowContent = row.Expression;
                //     if (row is SequenceNode sequence)
                //     {
                //         List<List<Node>>? chunks = null;
                //         List<Node>? currentChunk = null;
                //
                //         foreach (var child in sequence)
                //         {
                //             if (IsMatrixSeparator(child))
                //             {
                //                 // bail out if jagged
                //                 if (mappedRows.Count > 0 && currentChunk?.Count != mappedRows[0].Count)
                //                 {
                //                     return node;
                //                 }
                //
                //                 chunks ??= new();
                //                 chunks.Add(currentChunk ?? []);
                //                 currentChunk = null;
                //             }
                //             else
                //             {
                //                 currentChunk ??= new();
                //                 currentChunk.Add(child);
                //             }
                //         }
                //         
                //         chunks ??= new();
                //         chunks.Add(currentChunk ?? []);
                //     }
                //     else
                //     {
                //         if (row is InnerNode n)
                //         {
                //             if (n.Count == 0)
                //             {
                //                 mappedRows.Add([null]);
                //             } else if (n.Count == 1)
                //             {
                //                 mappedRows.Add([n[0]]);
                //             }
                //             else
                //             {
                //                 mappedRows.Add([n.ToSeq()]);
                //             }
                //         }
                //
                //     }
                //
                // });



                static bool IsMatrixSeparator(Node n) => n is IdentifierNode i && i.Value.Equals(",".AsMemory());

                static bool AreAllMatrixSeparator(IEnumerable<Node> nodes)
                {
                    foreach (var node in nodes)
                    {
                        if (!IsMatrixSeparator(node))
                        {
                            return false;
                        }
                    }

                    return true;
                }

                static bool AreAllMatrixParenNodes(IEnumerable<Node> nodes)
                {
                    foreach (var node in nodes)
                    {
                        if (node is ParenNode { LParen: { } lParen, RParen: { } rParen })
                        {
                            if (!(lParen.Type == TokenType.LeftParen
                                 && rParen.Type == TokenType.LeftParen
                                 && (lParen.Text.Span is "(" && lParen.Value == Symbol.lparen
                                    && rParen.Text.Span is ")"  && rParen.Value == Symbol.rparen)
                                 && (lParen.Text.Span is "[" && lParen.Value == Symbol.lbracket
                                    && rParen.Text.Span is "]"  && rParen.Value == Symbol.rbracket)))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }

                    return true;
                }

                // static Node ToSeq(ICollection<Node> nodes)
                // {
                //     if 
                //     def expression(*e)
                //     case e.length
                //         when 0
                //     nil
                //         when 1
                //     e[0]
                //     else
                //     Sequence.new(e)
                //     end
                //         end
                //
                // }
            }
        }

        static Node? HandleRightParen(Tokenizer tokenizer2, TokenType? closeParenType, Token token3)
        {
            if (closeParenType is null)
            {
                return SymbolNode.From(token3);
            }

            tokenizer2.PushBack(token3);
            return null;
        }

        static Node HandleUnary(Tokenizer tokenizer, TokenType? closeParenType, Token token1)
        {
            var s = TryUnwrapParen(ParseSimpleExpression(tokenizer, closeParenType)) ?? IdentifierNode.Empty;
            s = ConvertNode(s, token1.Converter?.ConvertUnary);
            return new UnaryOpNode(SymbolNode.From(token1), s);
        }

        static Node HandleBinary(Tokenizer tokenizer, TokenType? closeParenType, Token token1)
        {
            var s1 = TryUnwrapParen(ParseSimpleExpression(tokenizer, closeParenType)) ?? IdentifierNode.Empty;
            var s2 = TryUnwrapParen(ParseSimpleExpression(tokenizer, closeParenType)) ?? IdentifierNode.Empty;
            s1 = ConvertNode(s1, token1.Converter?.ConvertBinary1);
            s2 = ConvertNode(s2, token1.Converter?.ConvertBinary2);
            return new BinaryOpNode(SymbolNode.From(token1), s1, s2);
        }
    }

    private static Node ConcatExpressions(Node? e1, Node? e2)
        => (e1, e2) switch
        {
            (SequenceNode s1, SequenceNode s2) => new SequenceNode([.. s1, .. s2]),
            (SequenceNode s1, null) => s1,
            (SequenceNode s1, { } s2) => new SequenceNode([.. s1, s2]),
            (null, { } s2) => s2,
            ({ } s1, SequenceNode s2) => new SequenceNode([s1, .. s2]),
            ({ } s1, null) => s1,
            ({ } s1, { } s2) => new SequenceNode([s1, s2]),
            (null, null) => throw new InvalidOperationException("Cannot concat two null expressions"),
        };

    [return: NotNullIfNotNull(nameof(node))]
    private static Node? TryUnwrapParen(Node? node)
        => node is ParenNode paren ? GroupNode.From(paren) : node;

    private static Node ConvertNode(Node node, object? token1Converter)
    {
        // TODO: implement converters
        return node;
    }
}