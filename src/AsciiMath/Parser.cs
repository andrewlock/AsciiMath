using System.Diagnostics.CodeAnalysis;

namespace AsciiMath;

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
public static class Parser
{
    /// <summary>
    /// Parse the provided AsciiMath string, and convert it to a MathML string
    /// </summary>
    /// <param name="asciiMath">The AsciiMath input string</param>
    /// <returns>The MathML string, or <see cref="string.Empty"/> if the string cannot be parsed</returns>
    public static string ToMathMl(string asciiMath)
        => ToMathMl(asciiMath, MathMlOptions.Defaults);

    /// <summary>
    /// Parse the provided AsciiMath string, and convert it to a MathML string
    /// </summary>
    /// <param name="asciiMath">The AsciiMath input string</param>
    /// <param name="options">Options to use for generating the final MathML</param>
    /// <returns>The MathML string, or <see cref="string.Empty"/> if the string cannot be parsed</returns>
    public static string ToMathMl(string asciiMath, MathMlOptions options)
    {
        ArgumentNullException.ThrowIfNull(asciiMath);
        ArgumentNullException.ThrowIfNull(options);

        if (ExpressionParser.Parse(asciiMath) is not { } node)
        {
            return String.Empty;
        }

        return MathMlMarkupBuilder.Instance.Serialize(node, options, asciiMath);
    }
}