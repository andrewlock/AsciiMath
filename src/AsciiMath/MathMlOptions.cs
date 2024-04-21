namespace AsciiMath;

/// <summary>
/// Options for controlling MathML generation
/// </summary>
public class MathMlOptions
{
    internal static readonly MathMlOptions Defaults = new();

    /// <summary>
    /// If <c>true</c>, adds <c>display="block"</c> to the top level &lt;math&gt; block.
    /// If <c>false</c>, adds <c>display="inline"</c> to the top level &lt;math&gt; block. 
    /// If <c>null</c>, no additional attributes are added 
    /// </summary>
    /// <remarks>Defaults to <c>null</c></remarks>
    public bool? IsBlock { get; set; }
}