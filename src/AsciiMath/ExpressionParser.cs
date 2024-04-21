using System.Diagnostics.CodeAnalysis;

namespace AsciiMath;

internal static class ExpressionParser
{
    internal static Node? Parse(string input)
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
            // Not 100% this _can't_ be null... but we'll see
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

                        i++;
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

                List<List<Node?>> mappedRows = new();
                List<Node>? currentChunk = null;
                foreach(var row in rows)
                {
                    var rowContent = row is ParenNode p ? p.Expression
                        : row is GroupNode g ? g.Expression : null;

                    if (rowContent is SequenceNode sequence)
                    {
                        var mappedRow = new List<Node?>();
                        currentChunk ??= new();
                        currentChunk.Clear();
                
                        foreach (var child in sequence)
                        {
                            if (IsMatrixSeparator(child))
                            {
                                // bail out if jagged
                                mappedRow.Add(ConvertChunkToNode(currentChunk));
                                currentChunk.Clear();
                            }
                            else
                            {
                                currentChunk.Add(child);
                            }
                        }

                        mappedRow.Add(ConvertChunkToNode(currentChunk));
                        mappedRows.Add(mappedRow);
                    }
                    else
                    {
                        mappedRows.Add([ToExpression([rowContent])]);
                    }
                }

                return new MatrixNode(node.LParen, mappedRows, node.RParen);

                static Node? ConvertChunkToNode(List<Node> chunk)
                    => chunk.Count == 1 ? chunk[0] : ToExpression(chunk);

                static bool IsMatrixSeparator(Node n) => n is IdentifierNode { Value.Span: "," };

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
                                  && rParen.Type == TokenType.RightParen
                                  && ((lParen.Text.Span is "(" 
                                       && lParen.Value == Symbol.lparen
                                       && rParen.Text.Span is ")"
                                       && rParen.Value == Symbol.rparen)
                                      || (lParen.Text.Span is "["
                                          && lParen.Value == Symbol.lbracket
                                          && rParen.Text.Span is "]"
                                          && rParen.Value == Symbol.rbracket))))
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

                static Node? ToExpression(ICollection<Node> node)
                    => node switch
                    {
                        { Count: 0 } => null,
                        { Count: 1 } inner => inner.First(),
                        _ => new SequenceNode(node),
                        // I'm not sure which of these are correct :thinkin:
                        // InnerNode n => n.ToSeq(),
                        // _ => new SequenceNode([node]),
                    };
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
            s = token1.Converter?.ConvertUnary?.Invoke(s) ?? s;
            return new UnaryOpNode(SymbolNode.From(token1), s);
        }

        static Node HandleBinary(Tokenizer tokenizer, TokenType? closeParenType, Token token)
        {
            var s1 = TryUnwrapParen(ParseSimpleExpression(tokenizer, closeParenType)) ?? IdentifierNode.Empty;
            var s2 = TryUnwrapParen(ParseSimpleExpression(tokenizer, closeParenType)) ?? IdentifierNode.Empty;
            s1 = token.Converter?.ConvertBinary1?.Invoke(s1) ?? s1;
            s2 = token.Converter?.ConvertBinary2?.Invoke(s2) ?? s2;

            return new BinaryOpNode(SymbolNode.From(token), s1, s2);
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
        => node is ParenNode paren
           && (paren.LParen is null || paren.LParen.Type == TokenType.LeftParen)
           && (paren.RParen is null || paren.RParen.Type == TokenType.RightParen)
            ? GroupNode.From(paren)
            : node;
}