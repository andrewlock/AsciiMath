using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace AsciiMathParser;

internal abstract class Node : IFormattable
{
    public Node? Parent { get; set; }
    public abstract string ToString(string? format, IFormatProvider? formatProvider);
}

internal abstract class ValueNode<T>(T value) : Node
{
    public T Value { get; } = value;
}

internal class NumberNode(ReadOnlyMemory<char> number) : ValueNode<ReadOnlyMemory<char>>(number)
{
    public override string ToString(string? format, IFormatProvider? formatProvider) => ToString();
    public override string ToString() => number.ToString();
    
    protected bool Equals(NumberNode other)
    {
        return Value.Span.SequenceEqual(other.Value.Span);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((NumberNode)obj);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}

internal class TextNode(ReadOnlyMemory<char> text) : ValueNode<ReadOnlyMemory<char>>(text)
{
    public override string ToString(string? format, IFormatProvider? formatProvider) => ToString();
    public override string ToString() => $"\"{text}\"";
    
    protected bool Equals(TextNode other)
    {
        return Value.Span.SequenceEqual(other.Value.Span);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((TextNode)obj);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}

internal class SymbolNode(Symbol symbol, ReadOnlyMemory<char> text, TokenType type) : ValueNode<Symbol>(symbol)
{
    public ReadOnlyMemory<char> Text { get; } = text;
    public TokenType Type { get; } = type;

    public override string ToString(string? format, IFormatProvider? formatProvider) => ToString();
    public override string ToString() => Text.ToString();

    protected bool Equals(SymbolNode other)
    {
        return Value == other.Value && Text.Span.SequenceEqual(other.Text.Span) && Type == other.Type;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((SymbolNode)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)Value, Text, (int)Type);
    }

    [return: NotNullIfNotNull(nameof(token))]
    public static SymbolNode? From(Token? token)
    {
        if (token is null)
        {
            return null;
        }

        if (token.Value.Symbol is null)
        {
            throw new ArgumentException("Token.Symbol was null, but requires a value");
        }

        return new SymbolNode(token.Value.Symbol.Value, token.Value.Text, token.Value.Type);
    }
}

internal class IdentifierNode(ReadOnlyMemory<char> text) : ValueNode<ReadOnlyMemory<char>>(text)
{
    public static readonly IdentifierNode Empty = new("".AsMemory());

    public override string ToString(string? format, IFormatProvider? formatProvider) => ToString();
    public override string ToString() => Value.ToString();
    
    protected bool Equals(IdentifierNode other)
    {
        return Value.Span.SequenceEqual(other.Value.Span);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((IdentifierNode)obj);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}

internal abstract class InnerNode : Node
{
    protected List<Node> Children { get; } = new();

    public IEnumerator<Node> GetEnumerator() => Children.GetEnumerator();

    public void Add(Node node)
    {
        if (node.Parent is InnerNode inner)
        {
            inner.Remove(node);
        }
        node.Parent = this;
        Children.Add(node);
    }

    public bool Remove(Node item)
    {
        item.Parent = null;
        return Children.Remove(item);
    }

    public int Count => Children.Count;

    public Node this[int index] => Children[index];

    public SequenceNode ToSeq() => new SequenceNode(Children);
}

internal class SequenceNode: InnerNode
{
    public SequenceNode(IEnumerable<Node> nodes)
    {
        foreach (var node in nodes)
        {
            Add(node);
        }
    }

    public override string ToString(string? format, IFormatProvider? formatProvider) => ToString();
    public override string ToString() => string.Join(" ", Children);
    protected bool Equals(SequenceNode other)
    {
        return Children.SequenceEqual(other.Children);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((SequenceNode)obj);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        foreach (var child in Children)
        {
            hashCode.Add(child);
        }

        return hashCode.ToHashCode();
    }
}

internal class ParenNode : InnerNode
{
    public ParenNode(SymbolNode? lparen, Node? expression, SymbolNode? rparen)
    {
        if (lparen is not null && lparen.Type != TokenType.LeftParen)
        {
            throw new ArgumentException($"lparen must be {nameof(TokenType.LeftParen)}, but found {lparen.Type}", nameof(lparen));
        }
        if (rparen is not null && rparen.Type != TokenType.RightParen)
        {
            throw new ArgumentException($"rparen must be {nameof(TokenType.RightParen)}, but found {rparen.Type}", nameof(rparen));
        }

        LParen = lparen;
        RParen = rparen;
        if (expression is not null)
        {
            Add(expression);
        }
    }

    public SymbolNode? LParen { get; }

    public SymbolNode? RParen { get; }

    public Node? Expression => Children.Count > 0 ? Children[0] : null;

    public override string ToString() => $"{(LParen != null ? LParen : "")}{Expression}{(RParen != null ? RParen : "")}";
    public override string ToString(string? format, IFormatProvider? formatProvider) => ToString();

    public static ParenNode From(Token lparen, Node? expression, Token? rparen)
        => new(SymbolNode.From(lparen), expression, SymbolNode.From(rparen));

    protected bool Equals(ParenNode other)
    {
        return Equals(LParen, other.LParen) && Equals(RParen, other.RParen) && Equals(Expression, other.Expression);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((ParenNode)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(LParen, RParen, Expression);
    }
}

internal class MatrixNode : InnerNode
{
    public MatrixNode(SymbolNode lparen, IEnumerable<IEnumerable<Node>> rows, SymbolNode rparen)
    {
        if (lparen.Type != TokenType.LeftParen)
        {
            throw new ArgumentException($"lparen must be {nameof(TokenType.LeftParen)}, but found {lparen.Type}", nameof(lparen));
        }
        if (rparen.Type != TokenType.RightParen)
        {
            throw new ArgumentException($"rparen must be {nameof(TokenType.RightParen)}, but found {rparen.Type}", nameof(rparen));
        }

        LParen = lparen;
        RParen = rparen;
        foreach (var nodes in rows)
        {
            Add(new MatrixRowNode(nodes));
        }
    }

    public SymbolNode LParen { get; }

    public SymbolNode RParen { get; }

    public override string ToString() => $"{LParen}{string.Join(",", Children)}{RParen}";
    public override string ToString(string? format, IFormatProvider? formatProvider) => ToString();
}

internal class MatrixRowNode(IEnumerable<Node> nodes) : InnerNode
{
    public override string ToString() => $"({string.Join(",", nodes)})";
    public override string ToString(string? format, IFormatProvider? formatProvider) => ToString();
}

internal class EmptyNode : InnerNode
{
    public static readonly EmptyNode Instance = new();

    private EmptyNode()
    {
    }

    public override string ToString() => "";
    public override string ToString(string? format, IFormatProvider? formatProvider) => ToString();
}

internal class GroupNode : InnerNode
{
    public GroupNode(SymbolNode? lparen, Node? expression, SymbolNode? rparen)
    {
        if (lparen is not null && lparen.Type != TokenType.LeftParen)
        {
            throw new ArgumentException($"lparen must be {nameof(TokenType.LeftParen)}, but found {lparen.Type}", nameof(lparen));
        }
        if (rparen is not null && rparen.Type != TokenType.RightParen)
        {
            throw new ArgumentException($"rparen must be {nameof(TokenType.RightParen)}, but found {rparen.Type}", nameof(rparen));
        }

        LParen = lparen;
        RParen = rparen;
        if (expression is not null)
        {
            Add(expression);
        }
    }

    public SymbolNode? LParen { get; }

    public SymbolNode? RParen { get; }

    public Node? Expression => Children.Count > 0 ? Children[0] : null;

    public override string ToString() => $"{(LParen != null ? LParen : "")}{Expression}{(RParen != null ? RParen : "")}";
    public override string ToString(string? format, IFormatProvider? formatProvider) => ToString();

    public static GroupNode From(ParenNode parenNode)
        => new(parenNode.LParen, parenNode.Expression, parenNode.RParen);

    protected bool Equals(GroupNode other)
    {
        return Equals(LParen, other.LParen) && Equals(RParen, other.RParen) && Equals(Expression, other.Expression);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((GroupNode)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(LParen, RParen, Expression);
    }
}

internal class UnaryOpNode : InnerNode
{
    public UnaryOpNode(SymbolNode @operator, Node operand1)
    {
        if (@operator.Type != TokenType.Unary)
        {
            throw new ArgumentException($"operator must be {nameof(TokenType.Unary)}, but found {@operator.Type}", nameof(@operator));
        }
        
        Children.Add(@operator);
        Children.Add(operand1);
        Operator = @operator;
    }

    public SymbolNode Operator { get; }

    public Node Operand => Children[1];

    public override string ToString() => $"{Operator} {Operand}";
    public override string ToString(string? format, IFormatProvider? formatProvider) => ToString();
    
    protected bool Equals(UnaryOpNode other)
    {
        return Equals(Operator, other.Operator) && Equals(Operand, other.Operand);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((UnaryOpNode)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Operator, Operand);
    }
}

internal class BinaryOpNode : InnerNode
{
    public BinaryOpNode(SymbolNode @operator, Node operand1, Node operand2)
    {
        if (@operator.Type != TokenType.Binary)
        {
            throw new ArgumentException($"operator must be {nameof(TokenType.Binary)}, but found {@operator.Type}", nameof(@operator));
        }
        
        Children.Add(@operator);
        Children.Add(operand1);
        Children.Add(operand2);
        Operator = @operator;
    }

    public SymbolNode Operator { get; }

    public Node Operand1 => Children[1];
    public Node Operand2 => Children[2];

    public override string ToString() => $"{Operator} {Operand1} {Operand2}";
    public override string ToString(string? format, IFormatProvider? formatProvider) => ToString();
    
    protected bool Equals(BinaryOpNode other)
    {
        return Equals(Operand1, other.Operand1) && Equals(Operand2, other.Operand2) && Equals(Operator, other.Operator);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((BinaryOpNode)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Operand1, Operand2, Operator);
    }
}

internal class InfixOpNode : InnerNode
{
    public InfixOpNode(SymbolNode @operator, Node operand1, Node operand2)
    {
        if (@operator.Type != TokenType.Infix)
        {
            throw new ArgumentException($"operator must be {nameof(TokenType.Infix)}, but found {@operator.Type}", nameof(@operator));
        }
        
        Children.Add(@operator);
        Children.Add(operand1);
        Children.Add(operand2);
        Operator = @operator;
    }

    public SymbolNode Operator { get; }

    public Node Operand1 => Children[1];
    public Node Operand2 => Children[2];

    public override string ToString() => $"{Operand1} {Operator} {Operand2}";
    public override string ToString(string? format, IFormatProvider? formatProvider) => ToString();
    
    protected bool Equals(InfixOpNode other)
    {
        return Equals(Operand1, other.Operand1) && Equals(Operand2, other.Operand2) && Equals(Operator, other.Operator);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((InfixOpNode)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Operand1, Operand2, Operator);
    }
}

internal class SubSupNode : InnerNode
{
    public SubSupNode(Node expression, Node? sub, Node? sup)
    {
        Children.Add(expression);
        Children.Add(sub ?? EmptyNode.Instance);
        Children.Add(sup ?? EmptyNode.Instance);
    }

    public Node BaseExpression => Children[0];
    
    public Node? SubExpression
    {
        get
        {
            var child = Children[1];
            return child is EmptyNode ? null : child;
        }
    }

    public Node? SupExpression
    {
        get
        {
            var child = Children[2];
            return child is EmptyNode ? null : child;
        }
    }

    public override string ToString(string? format, IFormatProvider? formatProvider) => ToString();

    public override string ToString()
    {
        var sub = SubExpression;
        var sup = SupExpression;
        return (sub, sup) switch
        {
            (null, null) => BaseExpression.ToString()!,
            ({ }, null) => $"{BaseExpression}_{sub}",
            (null, { }) => $"{BaseExpression}^{sup}",
            ({ }, { }) => $"{BaseExpression}_{sub}^{sup}",
        };
    }
    
    protected bool Equals(SubSupNode other)
    {
        return Equals(BaseExpression, other.BaseExpression) && Equals(SubExpression, other.SubExpression) && Equals(SupExpression, other.SupExpression);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((SubSupNode)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(BaseExpression, SubExpression, SupExpression);
    }
}

