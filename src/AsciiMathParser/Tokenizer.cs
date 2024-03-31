using System.Buffers;
using System.Diagnostics;

namespace AsciiMathParser;

/// <summary>
/// Splits an ASCIIMath expression into a sequence of tokens.
/// </summary>
/// <remarks>
///
/// Each token is represented as a Hash containing the keys :value and :type.
/// The :value key is used to store the text associated with each token.
/// The :type key indicates the semantics of the token. The value for :type will be one
/// of the following symbols:
///
/// - :symbol a symbolic name or a bit of text without any further semantics
/// - :text a bit of arbitrary text
/// - :number a number
/// - :operator a mathematical operator symbol
/// - :unary a unary operator (e.g., sqrt, text, ...)
/// - :infix an infix operator (e.g, /, _, ^, ...)
/// - :binary a binary operator (e.g., frac, root, ...)
/// - :eof indicates no more tokens are available
/// </remarks>
internal class Tokenizer(string input)
{
    private static readonly SearchValues<char> Numbers = SearchValues.Create("0123456789");

    private ReadOnlyMemory<char> _span = input.AsMemory();
    private Token? _pushedBack;

    public Token GetNext()
    {
        if (_pushedBack is { } pushedBack)
        {
            _pushedBack = null;
            return pushedBack;
        }

        int firstNonWhitespace = 0;
        for (; firstNonWhitespace < _span.Length; firstNonWhitespace++)
        {
            if (!char.IsWhiteSpace(_span.Span[firstNonWhitespace]))
            {
                break;
            }
        }

        if (firstNonWhitespace == _span.Length)
        {
            return new Token(TokenType.Eof, ReadOnlyMemory<char>.Empty);
        }

        _span = _span.Slice(firstNonWhitespace);
        return _span.Span[0] switch
        {
            '"' => ReadQuotedText(),
            't' when _span.Span.Slice(0, 5) is "text(" => ReadTexText(),
            '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9' => ReadNumber() ?? ReadSymbol() ?? throw new NotImplementedException(),
            _ => ReadSymbol()  ?? throw new NotImplementedException(),
        };
    }

    public void PushBack(Token token)
    {
        if (token.Type != TokenType.Eof)
        {
            Debug.Assert(_pushedBack is null);
            _pushedBack = token;
        }
    }
    
    private Token ReadQuotedText()
    {
        Debug.Assert(_span.Span[0] == '"');
        var span = _span.Slice(1);
        var nextQuote = span.Span.IndexOf('"');
        if (nextQuote < 0)
        {
            _span = ReadOnlyMemory<char>.Empty;
            return new Token(TokenType.Text, span);
        }

        _span = span.Slice(nextQuote + 1);
        span = span.Slice(0, nextQuote);

        return new Token(TokenType.Text, span);
    }

    private Token ReadTexText()
    {
        Debug.Assert(_span.Span.Slice(0, 5) is "text(");
        var span = _span.Slice(5);
        var nextQuote = span.Span.IndexOf(')');
        if (nextQuote < 0)
        {
            _span = ReadOnlyMemory<char>.Empty;
            return new Token(TokenType.Text, span);
        }

        _span = span.Slice(nextQuote + 1);
        span = span.Slice(0, nextQuote);

        return new Token(TokenType.Text, span);
    }
        
    private Token? ReadNumber()
    {
        Debug.Assert(_span.Span.IndexOfAny(Numbers) == 0);
        ReadOnlyMemory<char> span;

        var indexOfFirstNonNumber = _span.Span.IndexOfAnyExcept(Numbers);
        if (indexOfFirstNonNumber < 0)
        {
            // everything is a number
            span = _span;
            _span = ReadOnlyMemory<char>.Empty;
            return new Token(TokenType.Number, span);
        }

        if (indexOfFirstNonNumber == _span.Length - 1
            || _span.Span[indexOfFirstNonNumber] != '.')
        {
            // the first non-number is the last character, so can't be a decimal
            span = _span.Slice(0, indexOfFirstNonNumber);
            _span = _span.Slice(indexOfFirstNonNumber);
            return new Token(TokenType.Number, span);
        }

        // we have a decimal, check for numbers
        var indexofSecondNonNumber = _span.Slice(indexOfFirstNonNumber + 1).Span.IndexOfAnyExcept(Numbers);
        if (indexofSecondNonNumber == 0)
        {
            // first character after '.' was not a number, so ignore the dot
            span = _span.Slice(0, indexOfFirstNonNumber);
            _span = _span.Slice(indexOfFirstNonNumber);
            return new Token(TokenType.Number, span);
        }

        if (indexofSecondNonNumber < 0)
        {
            // everything is a number
            span = _span;
            _span = ReadOnlyMemory<char>.Empty;
            return new Token(TokenType.Number, span);
        }

        // final case, extract the number portion
        span = _span.Slice(0, indexofSecondNonNumber);
        _span = _span.Slice(indexofSecondNonNumber);
        return new Token(TokenType.Number, span);
    }

    private Token? ReadSymbol()
    {
        int length = 0;
        while (length < SymbolTable.MaxKeyLength && length < _span.Length)
        {
            var currentChar = _span.Span[length];
            if (currentChar == '\\' && length < _span.Length - 2)
            {
                var next = _span.Span[length + 1];
                if (char.IsWhiteSpace(next) || char.IsNumber(next))
                {
                    // ok, increment and continue
                    length += 2;
                }
                else{
                    // no longer a symbol
                    break;
                }
            }
            else if (!char.IsWhiteSpace(currentChar) && !char.IsNumber(currentChar))
            {
                // still symbol
                length++;
            }
            else
            {
                // no longer a symbol
                break;
            }
        }

        if (length == 0)
        {
            return null;
        }

        // try to get the symbol from the table
        var span = _span.Slice(0, length);
        while (length > 0)
        {
            span = _span.Slice(0, length);
            if (SymbolTable.TryGetEntry(span.Span, out var s))
            {
                var symbol = s.Value;
                _span = _span.Slice(length);
                return new Token(symbol.Type, span, symbol.Symbol, symbol.Converter);
            }
            
            // not found, try shortening and repeat
            length--;
        }
        
        Debug.Assert(length == 0);
        Debug.Assert(span.Length == 1);
        
        // not found, return the single value as an identifier
        _span = _span.Slice(1);
        return new Token(TokenType.Identifier, span);
    }
}
