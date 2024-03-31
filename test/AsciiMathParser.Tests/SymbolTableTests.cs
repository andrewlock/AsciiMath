using FluentAssertions;
using FluentAssertions.Execution;
using Xunit.Abstractions;

namespace AsciiMathParser.Tests;

public class SymbolTableTests
{
    [Fact]
    public void SymbolTable_Includes_All_ExpectedSymbols()
    {
        using var scope = new AssertionScope();

        var expectedSymbols = SymbolTable.AllSymbols.Value;
        foreach (var (key, value) in expectedSymbols)
        {
            SymbolTable.TryGetEntry(key, out var actual).Should().BeTrue();
            actual.Should().Be(value, $"should have correct value for {key}");
        }
    }

    [Fact]
    public void SymbolTable_HasCorrectMaxLength()
    {
        var expectedLength = SymbolTable.AllSymbols.Value.Keys.MaxBy(x => x.Length)?.Length ?? 0;
        SymbolTable.MaxKeyLength.Should().Be(expectedLength);
    }
}

    // public class TestData(string ascii, string? html = null, string? mathMl = null, string? mathMlWord = null, string? latex = null)
    // {
    //     private static readonly TestData[] All =
    //     [
    //         new("underset(_)(hat A) = hat A exp j vartheta_0",
    //             mathMl:
    //             """<math><munder><mover accent="true"><mi>A</mi><mo>^</mo></mover><mo>_</mo></munder><mo>=</mo><mover accent="true"><mi>A</mi><mo>^</mo></mover><mi>exp</mi><mi>j</mi><msub><mi>&#x3D1;</mi><mn>0</mn></msub></math>""",
    //             mathMlWord:
    //             """<math><munder><mrow><mover accent="true"><mrow><mi>A</mi></mrow><mo>^</mo></mover></mrow><mrow><mo>_</mo></mrow></munder><mo>=</mo><mover accent="true"><mrow><mi>A</mi></mrow><mo>^</mo></mover><mi>exp</mi><mi>j</mi><msub><mrow><mi>&#x3D1;</mi></mrow><mrow><mn>0</mn></mrow></msub></math>""",
    //             latex: """\\underset{\\text{–}}{\\hat{A}} = \\hat{A} \\exp j \\vartheta_0"""
    //         ),
    //
    //         new("x+b/(2a)<+-sqrt((b^2)/(4a^2)-c/a)",
    //             mathMl:
    //             """<math><mi>x</mi><mo>+</mo><mfrac><mi>b</mi><mrow><mn>2</mn><mi>a</mi></mrow></mfrac><mo>&lt;</mo><mo>&#xB1;</mo><msqrt><mrow><mfrac><msup><mi>b</mi><mn>2</mn></msup><mrow><mn>4</mn><msup><mi>a</mi><mn>2</mn></msup></mrow></mfrac><mo>&#x2212;</mo><mfrac><mi>c</mi><mi>a</mi></mfrac></mrow></msqrt></math>""",
    //             latex: """x + \\frac{b}{2 a} < \\pm \\sqrt{\\frac{b^2}{4 a^2} - \\frac{c}{a}}"""
    //         ),
    //
    //         new("a^2 + b^2 = c^2",
    //             mathMl:
    //             """<math><msup><mi>a</mi><mn>2</mn></msup><mo>+</mo><msup><mi>b</mi><mn>2</mn></msup><mo>=</mo><msup><mi>c</mi><mn>2</mn></msup></math>""",
    //             html:
    //             """<span class="math-inline"><span class="math-identifier">a</span><span class="math-subsup"><span class="math-smaller"><span class="math-number">2</span></span><span class="math-smaller">&#x200D;</span></span><span class="math-operator">+</span><span class="math-identifier">b</span><span class="math-subsup"><span class="math-smaller"><span class="math-number">2</span></span><span class="math-smaller">&#x200D;</span></span><span class="math-operator">=</span><span class="math-identifier">c</span><span class="math-subsup"><span class="math-smaller"><span class="math-number">2</span></span><span class="math-smaller">&#x200D;</span></span></span>""",
    //             latex: """a^2 + b^2 = c^2"""
    //         ),
    //
    //         new("x = (-b+-sqrt(b^2-4ac))/(2a)",
    //             mathMl:
    //             """<math><mi>x</mi><mo>=</mo><mfrac><mrow><mo>&#x2212;</mo><mi>b</mi><mo>&#xB1;</mo><msqrt><mrow><msup><mi>b</mi><mn>2</mn></msup><mo>&#x2212;</mo><mn>4</mn><mi>a</mi><mi>c</mi></mrow></msqrt></mrow><mrow><mn>2</mn><mi>a</mi></mrow></mfrac></math>""",
    //             latex: """x = \\frac{- b \\pm \\sqrt{b^2 - 4 a c}}{2 a}"""
    //         ),
    //
    //         new("m = (y_2 - y_1)/(x_2 - x_1) = (Deltay)/(Deltax)",
    //             mathMl:
    //             """<math><mi>m</mi><mo>=</mo><mfrac><mrow><msub><mi>y</mi><mn>2</mn></msub><mo>&#x2212;</mo><msub><mi>y</mi><mn>1</mn></msub></mrow><mrow><msub><mi>x</mi><mn>2</mn></msub><mo>&#x2212;</mo><msub><mi>x</mi><mn>1</mn></msub></mrow></mfrac><mo>=</mo><mfrac><mrow><mo>&#x394;</mo><mi>y</mi></mrow><mrow><mo>&#x394;</mo><mi>x</mi></mrow></mfrac></math>""",
    //             html:
    //             """<span class="math-inline"><span class="math-identifier">m</span><span class="math-operator">=</span><span class="math-blank">&#x200D;</span><span class="math-fraction"><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-row"><span class="math-identifier">y</span><span class="math-subsup"><span class="math-smaller">&#x200D;</span><span class="math-smaller"><span class="math-number">2</span></span></span><span class="math-operator">&#x2212;</span><span class="math-identifier">y</span><span class="math-subsup"><span class="math-smaller">&#x200D;</span><span class="math-smaller"><span class="math-number">1</span></span></span></span></span></span></span></span><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-row"><span class="math-identifier">x</span><span class="math-subsup"><span class="math-smaller">&#x200D;</span><span class="math-smaller"><span class="math-number">2</span></span></span><span class="math-operator">&#x2212;</span><span class="math-identifier">x</span><span class="math-subsup"><span class="math-smaller">&#x200D;</span><span class="math-smaller"><span class="math-number">1</span></span></span></span></span></span></span></span></span><span class="math-operator">=</span><span class="math-blank">&#x200D;</span><span class="math-fraction"><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-row"><span class="math-operator">&#x394;</span><span class="math-identifier">y</span></span></span></span></span></span><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-row"><span class="math-operator">&#x394;</span><span class="math-identifier">x</span></span></span></span></span></span></span></span>""",
    //             latex: """m = \\frac{y_2 - y_1}{x_2 - x_1} = \\frac{\\Delta y}{\\Delta x}"""
    //         ),
    //
    //         new("f\'(x) = lim_(Deltax->0)(f(x+Deltax)-f(x))/(Deltax)",
    //             mathMl:
    //             """<math><mi>f</mi><mo>&#x2032;</mo><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow><mo>=</mo><munder><mo>lim</mo><mrow><mo>&#x394;</mo><mi>x</mi><mo>&#x2192;</mo><mn>0</mn></mrow></munder><mfrac><mrow><mi>f</mi><mrow><mo>(</mo><mrow><mi>x</mi><mo>+</mo><mo>&#x394;</mo><mi>x</mi></mrow><mo>)</mo></mrow><mo>&#x2212;</mo><mi>f</mi><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow></mrow><mrow><mo>&#x394;</mo><mi>x</mi></mrow></mfrac></math>""",
    //             html:
    //             """<span class="math-inline"><span class="math-identifier">f</span><span class="math-operator">&#x2032;</span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">x</span><span class="math-brace">)</span></span><span class="math-operator">=</span><span class="math-blank">&#x200D;</span><span class="math-underover"><span class="math-smaller"><span class="math-blank">&#x200D;</span></span><span class="math-operator">lim</span><span class="math-smaller"><span class="math-row"><span class="math-operator">&#x394;</span><span class="math-identifier">x</span><span class="math-operator">&#x2192;</span><span class="math-number">0</span></span></span></span><span class="math-blank">&#x200D;</span><span class="math-fraction"><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-row"><span class="math-identifier">f</span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">x</span><span class="math-operator">+</span><span class="math-operator">&#x394;</span><span class="math-identifier">x</span><span class="math-brace">)</span></span><span class="math-operator">&#x2212;</span><span class="math-identifier">f</span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">x</span><span class="math-brace">)</span></span></span></span></span></span></span><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-row"><span class="math-operator">&#x394;</span><span class="math-identifier">x</span></span></span></span></span></span></span></span>""",
    //             latex:
    //             """f \' ( x ) = \\lim_{\\Delta x \\rightarrow 0} \\frac{f \\left ( x + \\Delta x \\right ) - f ( x )}{\\Delta x}"""
    //         ),
    //
    //         new("d/dx [x^n] = nx^(n - 1)",
    //             mathMl:
    //             """<math><mfrac><mi>d</mi><mi>dx</mi></mfrac><mrow><mo>[</mo><msup><mi>x</mi><mi>n</mi></msup><mo>]</mo></mrow><mo>=</mo><mi>n</mi><msup><mi>x</mi><mrow><mi>n</mi><mo>&#x2212;</mo><mn>1</mn></mrow></msup></math>""",
    //             html:
    //             """<span class="math-inline"><span class="math-blank">&#x200D;</span><span class="math-fraction"><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-identifier">d</span></span></span></span></span><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-identifier">dx</span></span></span></span></span></span><span class="math-row"><span class="math-brace">[</span><span class="math-identifier">x</span><span class="math-subsup"><span class="math-smaller"><span class="math-identifier">n</span></span><span class="math-smaller">&#x200D;</span></span><span class="math-brace">]</span></span><span class="math-operator">=</span><span class="math-identifier">n</span><span class="math-identifier">x</span><span class="math-subsup"><span class="math-smaller"><span class="math-row"><span class="math-identifier">n</span><span class="math-operator">&#x2212;</span><span class="math-number">1</span></span></span><span class="math-smaller">&#x200D;</span></span></span>""",
    //             latex: """\\frac{d}{dx} [ x^n ] = n x^{n - 1}"""
    //         ),
    //
    //         new("int_a^b f(x) dx = [F(x)]_a^b = F(b) - F(a)",
    //             mathMl:
    //             """<math><msubsup><mo>&#x222B;</mo><mi>a</mi><mi>b</mi></msubsup><mi>f</mi><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow><mi>dx</mi><mo>=</mo><msubsup><mrow><mo>[</mo><mrow><mi>F</mi><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow></mrow><mo>]</mo></mrow><mi>a</mi><mi>b</mi></msubsup><mo>=</mo><mi>F</mi><mrow><mo>(</mo><mi>b</mi><mo>)</mo></mrow><mo>&#x2212;</mo><mi>F</mi><mrow><mo>(</mo><mi>a</mi><mo>)</mo></mrow></math>""",
    //             html:
    //             """<span class="math-inline"><span class="math-operator">&#x222B;</span><span class="math-subsup"><span class="math-smaller"><span class="math-identifier">b</span></span><span class="math-smaller"><span class="math-identifier">a</span></span></span><span class="math-identifier">f</span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">x</span><span class="math-brace">)</span></span><span class="math-identifier">dx</span><span class="math-operator">=</span><span class="math-row"><span class="math-brace">[</span><span class="math-identifier">F</span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">x</span><span class="math-brace">)</span></span><span class="math-brace">]</span></span><span class="math-subsup"><span class="math-smaller"><span class="math-identifier">b</span></span><span class="math-smaller"><span class="math-identifier">a</span></span></span><span class="math-operator">=</span><span class="math-identifier">F</span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">b</span><span class="math-brace">)</span></span><span class="math-operator">&#x2212;</span><span class="math-identifier">F</span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">a</span><span class="math-brace">)</span></span></span>""",
    //             latex: """\\int_a^b f ( x ) dx = {\\left [ F ( x ) \\right ]}_a^b = F ( b ) - F ( a )"""
    //         ),
    //
    //         new("int_a^b f(x) dx = f(c)(b - a)",
    //             mathMl:
    //             """<math><msubsup><mo>&#x222B;</mo><mi>a</mi><mi>b</mi></msubsup><mi>f</mi><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow><mi>dx</mi><mo>=</mo><mi>f</mi><mrow><mo>(</mo><mi>c</mi><mo>)</mo></mrow><mrow><mo>(</mo><mrow><mi>b</mi><mo>&#x2212;</mo><mi>a</mi></mrow><mo>)</mo></mrow></math>""",
    //             html:
    //             """<span class="math-inline"><span class="math-operator">&#x222B;</span><span class="math-subsup"><span class="math-smaller"><span class="math-identifier">b</span></span><span class="math-smaller"><span class="math-identifier">a</span></span></span><span class="math-identifier">f</span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">x</span><span class="math-brace">)</span></span><span class="math-identifier">dx</span><span class="math-operator">=</span><span class="math-identifier">f</span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">c</span><span class="math-brace">)</span></span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">b</span><span class="math-operator">&#x2212;</span><span class="math-identifier">a</span><span class="math-brace">)</span></span></span>""",
    //             latex: """\\int_a^b f ( x ) dx = f ( c ) ( b - a )"""
    //         ),
    //
    //         new("ax^2 + bx + c = 0",
    //             mathMl:
    //             """<math><mi>a</mi><msup><mi>x</mi><mn>2</mn></msup><mo>+</mo><mi>b</mi><mi>x</mi><mo>+</mo><mi>c</mi><mo>=</mo><mn>0</mn></math>""",
    //             html:
    //             """<span class="math-inline"><span class="math-identifier">a</span><span class="math-identifier">x</span><span class="math-subsup"><span class="math-smaller"><span class="math-number">2</span></span><span class="math-smaller">&#x200D;</span></span><span class="math-operator">+</span><span class="math-identifier">b</span><span class="math-identifier">x</span><span class="math-operator">+</span><span class="math-identifier">c</span><span class="math-operator">=</span><span class="math-number">0</span></span>""",
    //             latex: """a x^2 + b x + c = 0"""
    //         ),
    //
    //         new("\"average value\"=1/(b-a) int_a^b f(x) dx",
    //             mathMl:
    //             """<math><mtext>average value</mtext><mo>=</mo><mfrac><mn>1</mn><mrow><mi>b</mi><mo>&#x2212;</mo><mi>a</mi></mrow></mfrac><msubsup><mo>&#x222B;</mo><mi>a</mi><mi>b</mi></msubsup><mi>f</mi><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow><mi>dx</mi></math>""",
    //             html:
    //             """<span class="math-inline"><span class="math-text">average value</span><span class="math-operator">=</span><span class="math-blank">&#x200D;</span><span class="math-fraction"><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-number">1</span></span></span></span></span><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-row"><span class="math-identifier">b</span><span class="math-operator">&#x2212;</span><span class="math-identifier">a</span></span></span></span></span></span></span><span class="math-operator">&#x222B;</span><span class="math-subsup"><span class="math-smaller"><span class="math-identifier">b</span></span><span class="math-smaller"><span class="math-identifier">a</span></span></span><span class="math-identifier">f</span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">x</span><span class="math-brace">)</span></span><span class="math-identifier">dx</span></span>""",
    //             latex: """\\text{average value} = \\frac{1}{b - a} \\int_a^b f ( x ) dx"""
    //         ),
    //
    //         new("d/dx[int_a^x f(t) dt] = f(x)",
    //             mathMl:
    //             """<math><mfrac><mi>d</mi><mi>dx</mi></mfrac><mrow><mo>[</mo><mrow><msubsup><mo>&#x222B;</mo><mi>a</mi><mi>x</mi></msubsup><mi>f</mi><mrow><mo>(</mo><mi>t</mi><mo>)</mo></mrow><mi>dt</mi></mrow><mo>]</mo></mrow><mo>=</mo><mi>f</mi><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow></math>""",
    //             html:
    //             """<span class="math-inline"><span class="math-blank">&#x200D;</span><span class="math-fraction"><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-identifier">d</span></span></span></span></span><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-identifier">dx</span></span></span></span></span></span><span class="math-row"><span class="math-brace">[</span><span class="math-operator">&#x222B;</span><span class="math-subsup"><span class="math-smaller"><span class="math-identifier">x</span></span><span class="math-smaller"><span class="math-identifier">a</span></span></span><span class="math-identifier">f</span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">t</span><span class="math-brace">)</span></span><span class="math-identifier">dt</span><span class="math-brace">]</span></span><span class="math-operator">=</span><span class="math-identifier">f</span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">x</span><span class="math-brace">)</span></span></span>""",
    //             latex: """\\frac{d}{dx} \\left [ \\int_a^x f ( t ) dt \\right ] = f ( x )"""
    //         ),
    //
    //         new("hat(ab) bar(xy) ul(A) vec(v)",
    //             mathMl:
    //             """<math><mover accent="true"><mrow><mi>a</mi><mi>b</mi></mrow><mo>^</mo></mover><mover accent="true"><mrow><mi>x</mi><mi>y</mi></mrow><mo>&#xAF;</mo></mover><munder accentunder="true"><mi>A</mi><mo>_</mo></munder><mover accent="true"><mi>v</mi><mo>&#x2192;</mo></mover></math>""",
    //             html:
    //             """<span class="math-inline"><span class="math-blank">&#x200D;</span><span class="math-underover"><span class="math-smaller"><span class="math-operator">^</span></span><span class="math-row"><span class="math-identifier">a</span><span class="math-identifier">b</span></span><span class="math-smaller"><span class="math-blank">&#x200D;</span></span></span><span class="math-blank">&#x200D;</span><span class="math-underover"><span class="math-smaller"><span class="math-operator">&#xAF;</span></span><span class="math-row"><span class="math-identifier">x</span><span class="math-identifier">y</span></span><span class="math-smaller"><span class="math-blank">&#x200D;</span></span></span><span class="math-blank">&#x200D;</span><span class="math-underover"><span class="math-smaller"><span class="math-blank">&#x200D;</span></span><span class="math-identifier">A</span><span class="math-smaller"><span class="math-operator">_</span></span></span><span class="math-blank">&#x200D;</span><span class="math-underover"><span class="math-smaller"><span class="math-operator">&#x2192;</span></span><span class="math-identifier">v</span><span class="math-smaller"><span class="math-blank">&#x200D;</span></span></span></span>""",
    //             latex: """\\hat{a b} \\overline{x y} \\underline{A} \\vec{v}"""
    //         ),
    //
    //         new("z_12^34",
    //             mathMl: """<math><msubsup><mi>z</mi><mn>12</mn><mn>34</mn></msubsup></math>""",
    //             html:
    //             """<span class="math-inline"><span class="math-identifier">z</span><span class="math-subsup"><span class="math-smaller"><span class="math-number">34</span></span><span class="math-smaller"><span class="math-number">12</span></span></span></span>""",
    //             latex: """z_{12}^{34}"""
    //         ),
    //
    //         new("lim_(x->c)(f(x)-f(c))/(x-c)",
    //             mathMl:
    //             """<math><munder><mo>lim</mo><mrow><mi>x</mi><mo>&#x2192;</mo><mi>c</mi></mrow></munder><mfrac><mrow><mi>f</mi><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow><mo>&#x2212;</mo><mi>f</mi><mrow><mo>(</mo><mi>c</mi><mo>)</mo></mrow></mrow><mrow><mi>x</mi><mo>&#x2212;</mo><mi>c</mi></mrow></mfrac></math>""",
    //             html:
    //             """<span class="math-inline"><span class="math-blank">&#x200D;</span><span class="math-underover"><span class="math-smaller"><span class="math-blank">&#x200D;</span></span><span class="math-operator">lim</span><span class="math-smaller"><span class="math-row"><span class="math-identifier">x</span><span class="math-operator">&#x2192;</span><span class="math-identifier">c</span></span></span></span><span class="math-blank">&#x200D;</span><span class="math-fraction"><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-row"><span class="math-identifier">f</span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">x</span><span class="math-brace">)</span></span><span class="math-operator">&#x2212;</span><span class="math-identifier">f</span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">c</span><span class="math-brace">)</span></span></span></span></span></span></span><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-row"><span class="math-identifier">x</span><span class="math-operator">&#x2212;</span><span class="math-identifier">c</span></span></span></span></span></span></span></span>""",
    //             latex: """\\lim_{x \\rightarrow c} \\frac{f ( x ) - f ( c )}{x - c}"""
    //         ),
    //
    //         new("int_0^(pi/2) g(x) dx",
    //             mathMl:
    //             """<math><msubsup><mo>&#x222B;</mo><mn>0</mn><mfrac><mi>&#x3C0;</mi><mn>2</mn></mfrac></msubsup><mi>g</mi><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow><mi>dx</mi></math>""",
    //             html:
    //             """<span class="math-inline"><span class="math-operator">&#x222B;</span><span class="math-subsup"><span class="math-smaller"><span class="math-blank">&#x200D;</span><span class="math-fraction"><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-identifier">&#x3C0;</span></span></span></span></span><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-number">2</span></span></span></span></span></span></span><span class="math-smaller"><span class="math-number">0</span></span></span><span class="math-identifier">g</span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">x</span><span class="math-brace">)</span></span><span class="math-identifier">dx</span></span>""",
    //             latex: """\\int_0^{\\frac{\\pi}{2}} g ( x ) dx"""
    //         ),
    //
    //         new("sum_(n=0)^oo a_n",
    //             mathMl:
    //             """<math><munderover><mo>&#x2211;</mo><mrow><mi>n</mi><mo>=</mo><mn>0</mn></mrow><mo>&#x221E;</mo></munderover><msub><mi>a</mi><mi>n</mi></msub></math>""",
    //             html:
    //             """<span class="math-inline"><span class="math-blank">&#x200D;</span><span class="math-underover"><span class="math-smaller"><span class="math-operator">&#x221E;</span></span><span class="math-operator">&#x2211;</span><span class="math-smaller"><span class="math-row"><span class="math-identifier">n</span><span class="math-operator">=</span><span class="math-number">0</span></span></span></span><span class="math-identifier">a</span><span class="math-subsup"><span class="math-smaller">&#x200D;</span><span class="math-smaller"><span class="math-identifier">n</span></span></span></span>""",
    //             latex: """\\sum_{n = 0}^\\infty a_n"""
    //         ),
    //
    //         new("((1),(42))",
    //             mathMl:
    //             """<math><mrow><mo>(</mo><mtable><mtr><mtd><mn>1</mn></mtd></mtr><mtr><mtd><mn>42</mn></mtd></mtr></mtable><mo>)</mo></mrow></math>""",
    //             html:
    //             """<span class="math-inline"><span class="math-row"><span class="math-brace" style="font-size: 200%;">(</span><span class="math-matrix" style="grid-template-columns:repeat(1,1fr);grid-template-rows:repeat(2,1fr);"><span class="math-row"><span class="math-number">1</span></span><span class="math-row"><span class="math-number">42</span></span></span><span class="math-brace" style="font-size: 200%;">)</span></span></span>""",
    //             latex: """\\left ( \\begin{matrix} 1 \\\\ 42 \\end{matrix} \\right )"""
    //         ),
    //
    //         new("((1,42))",
    //             mathMl:
    //             """<math><mrow><mo>(</mo><mtable><mtr><mtd><mn>1</mn></mtd><mtd><mn>42</mn></mtd></mtr></mtable><mo>)</mo></mrow></math>""",
    //             html:
    //             """<span class="math-inline"><span class="math-row"><span class="math-brace" style="font-size: 100%;">(</span><span class="math-matrix" style="grid-template-columns:repeat(2,1fr);grid-template-rows:repeat(1,1fr);"><span class="math-row"><span class="math-number">1</span></span><span class="math-row"><span class="math-number">42</span></span></span><span class="math-brace" style="font-size: 100%;">)</span></span></span>""",
    //             latex: """\\left ( \\begin{matrix} 1 & 42 \\end{matrix} \\right )"""
    //         ),
    //
    //         new("((1,2,3),(4,5,6),(7,8,9))",
    //             mathMl:
    //             """<math><mrow><mo>(</mo><mtable><mtr><mtd><mn>1</mn></mtd><mtd><mn>2</mn></mtd><mtd><mn>3</mn></mtd></mtr><mtr><mtd><mn>4</mn></mtd><mtd><mn>5</mn></mtd><mtd><mn>6</mn></mtd></mtr><mtr><mtd><mn>7</mn></mtd><mtd><mn>8</mn></mtd><mtd><mn>9</mn></mtd></mtr></mtable><mo>)</mo></mrow></math>""",
    //             html:
    //             """<span class="math-inline"><span class="math-row"><span class="math-brace" style="font-size: 300%;">(</span><span class="math-matrix" style="grid-template-columns:repeat(3,1fr);grid-template-rows:repeat(3,1fr);"><span class="math-row"><span class="math-number">1</span></span><span class="math-row"><span class="math-number">2</span></span><span class="math-row"><span class="math-number">3</span></span><span class="math-row"><span class="math-number">4</span></span><span class="math-row"><span class="math-number">5</span></span><span class="math-row"><span class="math-number">6</span></span><span class="math-row"><span class="math-number">7</span></span><span class="math-row"><span class="math-number">8</span></span><span class="math-row"><span class="math-number">9</span></span></span><span class="math-brace" style="font-size: 300%;">)</span></span></span>""",
    //             latex: """\\left ( \\begin{matrix} 1 & 2 & 3 \\\\ 4 & 5 & 6 \\\\ 7 & 8 & 9 \\end{matrix} \\right )"""
    //         ),
    //
    //         new("|(a,b),(c,d)|=ad-bc",
    //             mathMl:
    //             """<math><mrow><mo>|</mo><mtable><mtr><mtd><mi>a</mi></mtd><mtd><mi>b</mi></mtd></mtr><mtr><mtd><mi>c</mi></mtd><mtd><mi>d</mi></mtd></mtr></mtable><mo>|</mo></mrow><mo>=</mo><mi>a</mi><mi>d</mi><mo>&#x2212;</mo><mi>b</mi><mi>c</mi></math>""",
    //             mathMlWord:
    //             """<math><mfenced open="|" close="|"><mtable><mtr><mtd><mi>a</mi></mtd><mtd><mi>b</mi></mtd></mtr><mtr><mtd><mi>c</mi></mtd><mtd><mi>d</mi></mtd></mtr></mtable></mfenced><mo>=</mo><mi>a</mi><mi>d</mi><mo>&#x2212;</mo><mi>b</mi><mi>c</mi></math>""",
    //             html:
    //             """<span class="math-inline"><span class="math-row"><span class="math-brace" style="font-size: 200%;">|</span><span class="math-matrix" style="grid-template-columns:repeat(2,1fr);grid-template-rows:repeat(2,1fr);"><span class="math-row"><span class="math-identifier">a</span></span><span class="math-row"><span class="math-identifier">b</span></span><span class="math-row"><span class="math-identifier">c</span></span><span class="math-row"><span class="math-identifier">d</span></span></span><span class="math-brace" style="font-size: 200%;">|</span></span><span class="math-operator">=</span><span class="math-identifier">a</span><span class="math-identifier">d</span><span class="math-operator">&#x2212;</span><span class="math-identifier">b</span><span class="math-identifier">c</span></span>""",
    //             latex: """\\left | \\begin{matrix} a & b \\\\ c & d \\end{matrix} \\right | = a d - b c"""
    //         ),
    //
    //         new("((a_(11), cdots , a_(1n)),(vdots, ddots, vdots),(a_(m1), cdots , a_(mn)))",
    //             mathMl:
    //             """<math><mrow><mo>(</mo><mtable><mtr><mtd><msub><mi>a</mi><mn>11</mn></msub></mtd><mtd><mo>&#x22EF;</mo></mtd><mtd><msub><mi>a</mi><mrow><mn>1</mn><mi>n</mi></mrow></msub></mtd></mtr><mtr><mtd><mo>&#x22EE;</mo></mtd><mtd><mo>&#x22F1;</mo></mtd><mtd><mo>&#x22EE;</mo></mtd></mtr><mtr><mtd><msub><mi>a</mi><mrow><mi>m</mi><mn>1</mn></mrow></msub></mtd><mtd><mo>&#x22EF;</mo></mtd><mtd><msub><mi>a</mi><mrow><mi>m</mi><mi>n</mi></mrow></msub></mtd></mtr></mtable><mo>)</mo></mrow></math>""",
    //             html:
    //             """<span class="math-inline"><span class="math-row"><span class="math-brace" style="font-size: 300%;">(</span><span class="math-matrix" style="grid-template-columns:repeat(3,1fr);grid-template-rows:repeat(3,1fr);"><span class="math-row"><span class="math-identifier">a</span><span class="math-subsup"><span class="math-smaller">&#x200D;</span><span class="math-smaller"><span class="math-number">11</span></span></span></span><span class="math-row"><span class="math-operator">&#x22EF;</span></span><span class="math-row"><span class="math-identifier">a</span><span class="math-subsup"><span class="math-smaller">&#x200D;</span><span class="math-smaller"><span class="math-row"><span class="math-number">1</span><span class="math-identifier">n</span></span></span></span></span><span class="math-row"><span class="math-operator">&#x22EE;</span></span><span class="math-row"><span class="math-operator">&#x22F1;</span></span><span class="math-row"><span class="math-operator">&#x22EE;</span></span><span class="math-row"><span class="math-identifier">a</span><span class="math-subsup"><span class="math-smaller">&#x200D;</span><span class="math-smaller"><span class="math-row"><span class="math-identifier">m</span><span class="math-number">1</span></span></span></span></span><span class="math-row"><span class="math-operator">&#x22EF;</span></span><span class="math-row"><span class="math-identifier">a</span><span class="math-subsup"><span class="math-smaller">&#x200D;</span><span class="math-smaller"><span class="math-row"><span class="math-identifier">m</span><span class="math-identifier">n</span></span></span></span></span></span><span class="math-brace" style="font-size: 300%;">)</span></span></span>""",
    //             latex:
    //             """\\left ( \\begin{matrix} a_{11} & \\cdots & a_{1 n} \\\\ \\vdots & \\ddots & \\vdots \\\\ a_{m 1} & \\cdots & a_{m n} \\end{matrix} \\right )"""
    //         ),
    //
    //         new("sum_(k=1)^n k = 1+2+ cdots +n=(n(n+1))/2",
    //             mathMl:
    //             """<math><munderover><mo>&#x2211;</mo><mrow><mi>k</mi><mo>=</mo><mn>1</mn></mrow><mi>n</mi></munderover><mi>k</mi><mo>=</mo><mn>1</mn><mo>+</mo><mn>2</mn><mo>+</mo><mo>&#x22EF;</mo><mo>+</mo><mi>n</mi><mo>=</mo><mfrac><mrow><mi>n</mi><mrow><mo>(</mo><mrow><mi>n</mi><mo>+</mo><mn>1</mn></mrow><mo>)</mo></mrow></mrow><mn>2</mn></mfrac></math>""",
    //             html:
    //             """<span class="math-inline"><span class="math-blank">&#x200D;</span><span class="math-underover"><span class="math-smaller"><span class="math-identifier">n</span></span><span class="math-operator">&#x2211;</span><span class="math-smaller"><span class="math-row"><span class="math-identifier">k</span><span class="math-operator">=</span><span class="math-number">1</span></span></span></span><span class="math-identifier">k</span><span class="math-operator">=</span><span class="math-number">1</span><span class="math-operator">+</span><span class="math-number">2</span><span class="math-operator">+</span><span class="math-operator">&#x22EF;</span><span class="math-operator">+</span><span class="math-identifier">n</span><span class="math-operator">=</span><span class="math-blank">&#x200D;</span><span class="math-fraction"><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-row"><span class="math-identifier">n</span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">n</span><span class="math-operator">+</span><span class="math-number">1</span><span class="math-brace">)</span></span></span></span></span></span></span><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-number">2</span></span></span></span></span></span></span>""",
    //             latex: """\\sum_{k = 1}^n k = 1 + 2 + \\cdots + n = \\frac{n ( n + 1 )}{2}"""
    //         ),
    //
    //         new("\"Скорость\"=(\"Расстояние\")/(\"Время\")",
    //             mathMl:
    //             """<math><mtext>&#x421;&#x43A;&#x43E;&#x440;&#x43E;&#x441;&#x442;&#x44C;</mtext><mo>=</mo><mfrac><mtext>&#x420;&#x430;&#x441;&#x441;&#x442;&#x43E;&#x44F;&#x43D;&#x438;&#x435;</mtext><mtext>&#x412;&#x440;&#x435;&#x43C;&#x44F;</mtext></mfrac></math>""",
    //             html:
    //             """<span class="math-inline"><span class="math-text">&#x421;&#x43A;&#x43E;&#x440;&#x43E;&#x441;&#x442;&#x44C;</span><span class="math-operator">=</span><span class="math-blank">&#x200D;</span><span class="math-fraction"><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-text">&#x420;&#x430;&#x441;&#x441;&#x442;&#x43E;&#x44F;&#x43D;&#x438;&#x435;</span></span></span></span></span><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-text">&#x412;&#x440;&#x435;&#x43C;&#x44F;</span></span></span></span></span></span></span>""",
    //             latex: """\\text{Скорость} = \\frac{\\text{Расстояние}}{\\text{Время}}"""
    //         ),
    //
    //         new("bb (a + b) + cc c = fr (d^n)",
    //             mathMl:
    //             """<math><mstyle mathvariant="bold"><mrow><mi>a</mi><mo>+</mo><mi>b</mi></mrow></mstyle><mo>+</mo><mstyle mathvariant="script"><mi>c</mi></mstyle><mo>=</mo><mstyle mathvariant="fraktur"><msup><mi>d</mi><mi>n</mi></msup></mstyle></math>""",
    //             latex: """\\mathbf{a + b} + \\mathscr{c} = \\mathfrak{d^n}"""
    //         ),
    //
    //         new("max()",
    //             mathMl: """<math><mo>max</mo><mrow><mo>(</mo><mo>)</mo></mrow></math>""",
    //             html:
    //             """<span class="math-inline"><span class="math-operator">max</span><span class="math-row"><span class="math-brace">(</span><span class="math-brace">)</span></span></span>""",
    //             latex: """\\max (  )"""
    //         ),
    //
    //         new("""text("foo")""",
    //             mathMl: """<math><mtext>"foo"</mtext></math>""",
    //             html: """<span class="math-inline"><span class="math-text">"foo"</span></span>""",
    //             latex: """\\text{"foo"}"""
    //         ),
    //
    //         new("ubrace(1 + 2) obrace(3 + 4",
    //             mathMl:
    //             """<math><munder accentunder="true"><mrow><mn>1</mn><mo>+</mo><mn>2</mn></mrow><mo>&#x23DF;</mo></munder><mover accent="true"><mrow><mn>3</mn><mo>+</mo><mn>4</mn></mrow><mo>&#x23DE;</mo></mover></math>""",
    //             latex: """\\underbrace{1 + 2} \\overbrace{3 + 4}"""
    //         ),
    //
    //         new("s\'_i = {(- 1, if s_i > s_(i + 1)),( + 1, if s_i <= s_(i + 1)):}",
    //             mathMl:
    //             """<math><mi>s</mi><msub><mo>&#x2032;</mo><mi>i</mi></msub><mo>=</mo><mrow><mo>{</mo><mtable><mtr><mtd><mrow><mo>&#x2212;</mo><mn>1</mn></mrow></mtd><mtd><mrow><mo>if</mo><msub><mi>s</mi><mi>i</mi></msub><mo>&gt;</mo><msub><mi>s</mi><mrow><mi>i</mi><mo>+</mo><mn>1</mn></mrow></msub></mrow></mtd></mtr><mtr><mtd><mrow><mo>+</mo><mn>1</mn></mrow></mtd><mtd><mrow><mo>if</mo><msub><mi>s</mi><mi>i</mi></msub><mo>&#x2264;</mo><msub><mi>s</mi><mrow><mi>i</mi><mo>+</mo><mn>1</mn></mrow></msub></mrow></mtd></mtr></mtable></mrow></math>""",
    //             latex:
    //             """s \'_i = \\left \\{ \\begin{matrix} - 1 & \\operatorname{if} s_i > s_{i + 1} \\\\ + 1 & \\operatorname{if} s_i \\le s_{i + 1} \\end{matrix} \\right ."""
    //         ),
    //
    //         new("s\'_i = {(, if s_i > s_(i + 1)),( + 1,):}",
    //             mathMl:
    //             """<math><mi>s</mi><msub><mo>&#x2032;</mo><mi>i</mi></msub><mo>=</mo><mrow><mo>{</mo><mtable><mtr><mtd></mtd><mtd><mrow><mo>if</mo><msub><mi>s</mi><mi>i</mi></msub><mo>&gt;</mo><msub><mi>s</mi><mrow><mi>i</mi><mo>+</mo><mn>1</mn></mrow></msub></mrow></mtd></mtr><mtr><mtd><mrow><mo>+</mo><mn>1</mn></mrow></mtd><mtd></mtd></mtr></mtable></mrow></math>""",
    //             latex:
    //             """s \'_i = \\left \\{ \\begin{matrix}  & \\operatorname{if} s_i > s_{i + 1} \\\\ + 1 &  \\end{matrix} \\right ."""
    //         ),
    //
    //         new("{:(a,b),(c,d):}",
    //             mathMl:
    //             """<math><mtable><mtr><mtd><mi>a</mi></mtd><mtd><mi>b</mi></mtd></mtr><mtr><mtd><mi>c</mi></mtd><mtd><mi>d</mi></mtd></mtr></mtable></math>""",
    //             latex: """\\begin{matrix} a & b \\\\ c & d \\end{matrix}"""
    //         ),
    //
    //         new("overset (a + b) (c + d)",
    //             mathMl:
    //             """<math><mover><mrow><mi>c</mi><mo>+</mo><mi>d</mi></mrow><mrow><mi>a</mi><mo>+</mo><mi>b</mi></mrow></mover></math>""",
    //             html:
    //             """<span class="math-inline"><span class="math-blank">&#x200D;</span><span class="math-underover"><span class="math-smaller"><span class="math-row"><span class="math-identifier">a</span><span class="math-operator">+</span><span class="math-identifier">b</span></span></span><span class="math-row"><span class="math-identifier">c</span><span class="math-operator">+</span><span class="math-identifier">d</span></span><span class="math-smaller"><span class="math-blank">&#x200D;</span></span></span></span>""",
    //             latex: """\\overset{a + b}{c + d}"""
    //         ),
    //
    //         new("underset a b",
    //             mathMl: """<math><munder><mi>b</mi><mi>a</mi></munder></math>""",
    //             html:
    //             """<span class="math-inline"><span class="math-blank">&#x200D;</span><span class="math-underover"><span class="math-smaller"><span class="math-blank">&#x200D;</span></span><span class="math-identifier">b</span><span class="math-smaller"><span class="math-identifier">a</span></span></span></span>""",
    //             latex: """\\underset{a}{b}"""
    //         ),
    //
    //         new("sin a_c^b",
    //             mathMl: """<math><mi>sin</mi><msubsup><mi>a</mi><mi>c</mi><mi>b</mi></msubsup></math>""",
    //             html:
    //             """<span class="math-inline"><span class="math-identifier">sin</span><span class="math-identifier">a</span><span class="math-subsup"><span class="math-smaller"><span class="math-identifier">b</span></span><span class="math-smaller"><span class="math-identifier">c</span></span></span></span>""",
    //             latex: """\\sin a_c^b"""
    //         ),
    //
    //         new("max a_c^b",
    //             mathMl: """<math><mo>max</mo><msubsup><mi>a</mi><mi>c</mi><mi>b</mi></msubsup></math>""",
    //             html:
    //             """<span class="math-inline"><span class="math-operator">max</span><span class="math-identifier">a</span><span class="math-subsup"><span class="math-smaller"><span class="math-identifier">b</span></span><span class="math-smaller"><span class="math-identifier">c</span></span></span></span>""",
    //             latex: """\\max a_c^b"""
    //         ),
    //
    //         new("norm a_c^b",
    //             mathMl:
    //             """<math><msubsup><mrow><mo>&#x2225;</mo><mi>a</mi><mo>&#x2225;</mo></mrow><mi>c</mi><mi>b</mi></msubsup></math>""",
    //             html:
    //             """<span class="math-inline"><span class="math-row"><span class="math-brace">&#x2225;</span><span class="math-identifier">a</span><span class="math-brace">&#x2225;</span></span><span class="math-subsup"><span class="math-smaller"><span class="math-identifier">b</span></span><span class="math-smaller"><span class="math-identifier">c</span></span></span></span>""",
    //             latex: """{\\lVert a \\rVert}_c^b"""
    //         ),
    //
    //         new("overarc a_b^c",
    //             mathMl:
    //             """<math><msubsup><mover accent="true"><mi>a</mi><mo>&#x23DC;</mo></mover><mi>b</mi><mi>c</mi></msubsup></math>""",
    //             latex: """{\\overset{\\frown}{a}}_b^c"""
    //         ),
    //
    //         new("frown a_b^c",
    //             mathMl: """<math><mo>&#x2322;</mo><msubsup><mi>a</mi><mi>b</mi><mi>c</mi></msubsup></math>""",
    //             latex: """\\frown a_b^c"""
    //         ),
    //
    //         new("sin(a_c^b)",
    //             mathMl:
    //             """<math><mi>sin</mi><mrow><mo>(</mo><msubsup><mi>a</mi><mi>c</mi><mi>b</mi></msubsup><mo>)</mo></mrow></math>""",
    //             latex: """\\sin ( a_c^b )"""
    //         ),
    //
    //         new("text(a)a2)",
    //             mathMl: """<math><mtext>a</mtext><mi>a</mi><mn>2</mn><mo>)</mo></math>""",
    //             html:
    //             """<span class="math-inline"><span class="math-text">a</span><span class="math-identifier">a</span><span class="math-number">2</span><span class="math-operator">)</span></span>""",
    //             latex: """\\text{a} a 2 )"""
    //         ),
    //
    //         new("cancel(a_b^c) cancel a_b^c",
    //             mathMl:
    //             """<math><menclose notation="updiagonalstrike"><msubsup><mi>a</mi><mi>b</mi><mi>c</mi></msubsup></menclose><msubsup><menclose notation="updiagonalstrike"><mi>a</mi></menclose><mi>b</mi><mi>c</mi></msubsup></math>""",
    //             latex: """\\cancel{a_b^c} {\\cancel{a}}_b^c"""
    //         ),
    //
    //         new("color(red)(x) color(#123)(y) color(#1234ab)(z) colortext(blue)(a_b^c)",
    //             mathMl:
    //             """<math><mstyle mathcolor="#ff0000"><mi>x</mi></mstyle><mstyle mathcolor="#112233"><mi>y</mi></mstyle><mstyle mathcolor="#1234ab"><mi>z</mi></mstyle><mstyle mathcolor="#0000ff"><msubsup><mi>a</mi><mi>b</mi><mi>c</mi></msubsup></mstyle></math>""",
    //             latex:
    //             """{\\color{red} x} {\\color[RGB]{17,34,51} y} {\\color[RGB]{18,52,171} z} {\\color{blue} a_b^c}"""
    //         ),
    //
    //         new(@"{ x\ : \ x in A ^^ x in B }",
    //             mathMl:
    //             """<math><mrow><mo>{</mo><mrow><mi>x</mi><mo>&#xA0;</mo><mo>:</mo><mo>&#xA0;</mo><mi>x</mi><mo>&#x2208;</mo><mi>A</mi><mo>&#x2227;</mo><mi>x</mi><mo>&#x2208;</mo><mi>B</mi></mrow><mo>}</mo></mrow></math>""",
    //             latex: """\\left \\{ x \\; : \\; x \\in A \\wedge x \\in B \\right \\}"""
    //         ),
    //
    //         new("ii",
    //             mathMl: """<math><mstyle mathvariant="italic"><mi></mi></mstyle></math>"""
    //         ),
    //
    //         new("rm(ms)",
    //             mathMl: """<math><mstyle mathvariant="normal"><mrow><mi>m</mi><mi>s</mi></mrow></mstyle></math>"""
    //         ),
    //
    //         new("hat",
    //             mathMl: """<math><mover accent="true"><mi></mi><mo>^</mo></mover></math>"""
    //         ),
    //
    //         new("40% * 3!",
    //             mathMl: """<math><mn>40</mn><mo>%</mo><mo>&#x22C5;</mo><mn>3</mn><mo>!</mo></math>""",
    //             html:
    //             """<span class="math-inline"><span class="math-number">40</span><span class="math-operator">%</span><span class="math-operator">&#x22C5;</span><span class="math-number">3</span><span class="math-operator">!</span></span>""",
    //             latex: """40 \% \cdot 3 !"""
    //         ),
    //
    //         new("R(alpha_(K+1)|x)",
    //             mathMl:
    //             """<math><mi>R</mi><mrow><mo>(</mo><mrow><msub><mi>&#x3B1;</mi><mrow><mi>K</mi><mo>+</mo><mn>1</mn></mrow></msub><mo>|</mo><mi>x</mi></mrow><mo>)</mo></mrow></math>""",
    //             latex: """R \\left ( \\alpha_{K + 1} | x \\right )"""
    //         ),
    //
    //         new("~a mlt b mgt -+c",
    //             latex: """\\sim a \\ll b \\gg \\mp c""",
    //             mathMl:
    //             """<math><mo>&#x223C;</mo><mi>a</mi><mo>&#x226A;</mo><mi>b</mi><mo>&#x226B;</mo><mo>&#x2213;</mo><mi>c</mi></math>"""
    //         ),
    //
    //         new("a+b+...+c",
    //             latex: """a + b + \ldots + c""",
    //             mathMl: """<math><mi>a</mi><mo>+</mo><mi>b</mi><mo>+</mo><mo>&#x2026;</mo><mo>+</mo><mi>c</mi></math>"""
    //         ),
    //
    //         new("frac{a}{b}",
    //             latex: """\frac{a}{b}""",
    //             mathMl: """<math><mfrac><mi>a</mi><mi>b</mi></mfrac></math>"""
    //         ),
    //
    //         new("ubrace(((1, 0),(0, 1)))_(\"Adjustment to texture space\")",
    //             latex:
    //             """\underbrace{\left ( \begin{matrix} 1 & 0 \\\\ 0 & 1 \end{matrix} \right )}_{\text{Adjustment to texture space}}""",
    //             mathMl:
    //             """<math><munder><munder accentunder="true"><mrow><mo>(</mo><mtable><mtr><mtd><mn>1</mn></mtd><mtd><mn>0</mn></mtd></mtr><mtr><mtd><mn>0</mn></mtd><mtd><mn>1</mn></mtd></mtr></mtable><mo>)</mo></mrow><mo>&#x23DF;</mo></munder><mtext>Adjustment to texture space</mtext></munder></math>"""
    //         ),
    //
    //
    //         }