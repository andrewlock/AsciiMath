namespace AsciiMath;

/// <summary>
/// Options for controlling MathML generation
/// </summary>
public class MathMlOptions
{
    internal static readonly MathMlOptions Defaults = new();

    /// <summary>
    /// The type of <c>display</c> attribute to add to the top-level &lt;math&gt; element.
    /// </summary>
    /// <remarks>Defaults to <see cref="MathMlDisplayType.None"/></remarks>
    public MathMlDisplayType DisplayType { get; set; } = MathMlDisplayType.None;

    /// <summary>
    /// Whether to add the provided AsciiMath input as a
    /// <c>title</c> attribute to the top-level &lt;math&gt; block.
    /// </summary>
    /// <remarks>Defaults to <c>false</c></remarks>
    public bool IncludeTitle { get; set; } = false;
}