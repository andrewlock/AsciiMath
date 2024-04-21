namespace AsciiMath;

/// <summary>
/// The type of <c>display</c> attribute to add to the top-level &lt;math&gt; element.
/// </summary>
public enum MathMlDisplayType
{
    /// <summary>
    /// Don't add a <c>display</c> attribute.
    /// </summary>
    None,

    /// <summary>
    /// Add <c>display="block"</c>.
    /// </summary>
    Block,

    /// <summary>
    /// Add <c>display="block"</c>.
    /// </summary>
    Inline,
}