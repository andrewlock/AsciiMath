namespace AsciiMath.Tests;

internal static class TestAst
{
    public static SymbolNode symbol(string text)
    {
        return SymbolTable.TryGetEntry(text, out var entry)
            ? new SymbolNode(entry.Value.Symbol, text.AsMemory(), entry.Value.Type)
            : null;
    }

    public static NumberNode number(string text) => new(text.AsMemory());
    public static TextNode text(string text) => new(text.AsMemory());
    public static IdentifierNode identifier(string text) => new(text.AsMemory());
    
    public static UnaryOpNode unary(SymbolNode @operator, object e) => new(@operator, ToAst(e));
    public static BinaryOpNode binary(SymbolNode @operator, object e1, object e2) => new(@operator, ToAst(e1), ToAst(e2));
    public static InfixOpNode infix(object e1, SymbolNode @operator, object e2) => new(@operator, ToAst(e1), ToAst(e2));
    public static SubSupNode subsup(object e, object sub, object sup) => new(ToAst(e), ToAst(sub), ToAst(sup));
    public static SubSupNode sub(object e, object sub) => new(ToAst(e), ToAst(sub), null);
    public static SubSupNode sup(object e, object sup) => new(ToAst(e), null, ToAst(sup));
    public static ColorNode color(int r, int g, int b, string text) => new(text, r, g, b);

    public static MatrixNode matrix(object[][] args)
        => matrix(symbol("("), args, symbol(")"));

    public static MatrixNode matrix(SymbolNode lparen, object[][] args, SymbolNode rparen)
    {
        // convert rows
        var rows = new Node[args.Length][];

        for (var i = 0; i < args.Length; i++)
        {
            var argRow = args[i];
            rows[i] = new Node[argRow.Length];
            for (var j = 0; j < argRow.Length; j++)
            {
                rows[i][j] = ToAst(argRow[j]);
            }
        }

        return new MatrixNode(lparen, rows, rparen);
    }
    
    
    public static GroupNode grseq(params object[] arr) => group(seq(arr));

    public static GroupNode group(params object[] args)
    {
        SymbolNode lparen;
        Node e;
        SymbolNode rparen;
        
        if (args.Length == 1)
        {
            lparen = symbol("(");
            e = ToAst(args[0]);
            rparen = symbol(")");
        }
        else if (args.Length == 3)
        {
            lparen = args[0] as SymbolNode;
            e = ToAst(args[1]);
            rparen = args[2] as SymbolNode;
        }
        else
        {
            throw new InvalidOperationException("Invalid number of args");
        }

        return new GroupNode(lparen, e, rparen);
    }
    
    public static ParenNode paren(params object[] args)
    {
        SymbolNode lparen = null;
        Node e = null;
        SymbolNode rparen = null;

        if (args is null || args.Length == 1)
        {
            lparen = symbol("(");
            e = ToAst(args?[0]);
            rparen = symbol(")");
        }
        else if (args.Length == 3)
        {
            lparen = args[0] as SymbolNode;
            e = ToAst(args[1]);
            rparen = args[2] as SymbolNode;
        }
        else if (args.Length != 0)
        {
            throw new InvalidOperationException("Invalid number of args");
        }

        return new ParenNode(lparen, e, rparen);
    }
    
    public static Node seq(params object[] arr)
        => arr.Length switch
        {
            0 => null,
            1 => ToAst(arr[0]),
            _ => new SequenceNode(arr.Select(x => ToAst(x))),
        };

    private static Node ToAst(object value)
    {
        if (value is string or char)
        {
            var v = value.ToString();
            var s = TestAst.symbol(v);
            if (s is not null)
            {
                return s;
            }
            else if (decimal.TryParse(v, out _))
            {
                return number(v);
            }
            else if (v.Length > 1)
            {
                return text(v);
            }
            else
            {
                return identifier(v);
            }
        }
        else if (value is object[] arr)
        {
            return seq(arr);
        }

        return value as Node;
    }
}