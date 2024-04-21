using System.Buffers;
using System.Globalization;
using System.Text;

namespace AsciiMath;

public static class StringBuilderExtensions
{
    private static readonly SearchValues<char> ValuesToEscape = SearchValues.Create("&<>");

    public static StringBuilder AppendEscapedText(this StringBuilder sb, ReadOnlyMemory<char> text, bool escapeNonAscii)
    {
        // check for non-ascii and Values to escape
        if (text.Span.IndexOfAny(ValuesToEscape) == -1
            && (!escapeNonAscii || text.Span.IndexOfAnyExceptInRange((char)0, (char)127) == -1))
        {
            // all fine, append everything
            sb.Append(text);
            return sb;
        }

        // need to loop through
        foreach (var c in text.Span)
        {
            if (c == '&')
            {
                sb.Append("&amp;");
            }else if (c == '<')
            {
                sb.Append("&lt;");
            }
            else if (c == '>')
            {
                sb.Append("&gt;");
            }
            else if (c > 127 && escapeNonAscii)
            {
                sb.AppendFormat(CultureInfo.InvariantCulture, $"&#x{((int)c):X};");
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb;
    }
}