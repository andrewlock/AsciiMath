namespace AsciiMath;

internal readonly struct Token(TokenType type, ReadOnlyMemory<char> text, Symbol? symbol = null, Converter? converter = null)
{
    public readonly TokenType Type = type;
    public readonly  ReadOnlyMemory<char> Text = text;
    public readonly Symbol? Symbol = symbol;
    public readonly Converter? Converter = converter;
}

internal readonly struct SymbolDetail(Symbol? symbol, TokenType type, Converter? converter = null)
{
    public readonly Symbol? Symbol = symbol;
    public readonly TokenType Type = type;
    public readonly Converter? Converter = converter;
}

internal enum TokenType
{
    Symbol,
    Text,
    Number,
    Identifier,
    // Operator,
    Unary,
    Binary,
    Infix,
    LeftParen,
    RightParen,
    LeftRightParen,
    Eof,
}

internal readonly struct Converter(
    Func<Node, Node>? convertUnary = null,
    Func<Node, Node>? convertBinary1 = null,
    Func<Node, Node>? convertBinary2 = null)
{
    public readonly Func<Node, Node>? ConvertUnary = convertUnary;
    public readonly Func<Node, Node>? ConvertBinary1 = convertBinary1;
    public readonly Func<Node, Node>? ConvertBinary2 = convertBinary2;
}