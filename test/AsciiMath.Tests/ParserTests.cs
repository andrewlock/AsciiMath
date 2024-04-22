using FluentAssertions;
using Xunit.Abstractions;

namespace AsciiMath.Tests;

public class ParserTests(ITestOutputHelper output)
{
    [Fact]
    public void ParsesSample()
    {
        var asciiMath = "int_-1^1 sqrt(1-x^2)dx = pi/2";
        var converted = Parser.ToMathMl(asciiMath);
        output.WriteLine(converted);
    }

    [Theory]
    [InlineData(MathMlDisplayType.None, """<math><msub><mo>log</mo><mn>2</mn></msub></math>""")]
    [InlineData(MathMlDisplayType.Block, """<math display="block"><msub><mo>log</mo><mn>2</mn></msub></math>""")]
    [InlineData(MathMlDisplayType.Inline, """<math display="inline"><msub><mo>log</mo><mn>2</mn></msub></math>""")]
    public void AddsDisplayAttribute(MathMlDisplayType display, string expected)
    {
        var asciiMath = "log_2";
        var converted = Parser.ToMathMl(asciiMath, new() { DisplayType = display });
        converted.Should().Be(expected);
    }

    [Theory]
    [InlineData("log_2", """<math title="log_2"><msub><mo>log</mo><mn>2</mn></msub></math>""")]
    [InlineData("\"someText\"", """<math title="&quot;someText&quot;"><mtext>someText</mtext></math>""")]
    [InlineData("f = \"x\"", """<math title="f = &quot;x&quot;"><mi>f</mi><mo>=</mo><mtext>x</mtext></math>""")]
    public void AddsTitle(string asciiMath, string expected)
    {
        var converted = Parser.ToMathMl(asciiMath, new() { IncludeTitle = true});
        converted.Should().Be(expected);
    }
    
    [Theory]
    [MemberData(nameof(TestSpec.AstTests), MemberType = typeof(TestSpec))]
    public void ParsesAstCorrectly(string asciiMath)
    {
        var spec = TestSpec.Specs[asciiMath];
        var parsed = ExpressionParser.Parse(spec.asciiMath);
        if (spec.ast is SequenceNode expected)
        {
            var actual = parsed.Should().NotBeNull().And.BeOfType<SequenceNode>().Subject;

            actual.Count.Should().Be(expected.Count);
            for (int i = 0; i < actual.Count; i++)
            {
                actual[i].Should().Be(expected[i]);
            }
        }

        parsed.Should().NotBeNull().And.Be(spec.ast);
    }

    // These are the test cases from the asciidoctor
    // Note that some of the test cases have subsequently been modified
    // based on insights from https://github.com/asciidoctor/asciimath/issues/60
    [Theory]
    [MemberData(nameof(TestSpec.MathMlTests), MemberType = typeof(TestSpec))]
    public void ConvertsToMathMlCorrectly(string asciiMath)
    {
        var spec = TestSpec.Specs[asciiMath];
        var converted = Parser.ToMathMl(spec.asciiMath);

        converted.Should().Be(spec.mathml);
    }

    // These are the test cases from the asciimath.js unit tests: https://github.com/asciimath/asciimathml/blob/master/test/unittests.js
    [Theory(Skip = "A lot of these cases aren't supported")]
    [MemberData(nameof(AsciiMathTestSpec.AllTests), MemberType = typeof(AsciiMathTestSpec))]
    public void ConvertsToMathMlCorrectly2(string input, string output)
    {
        var converted = Parser.ToMathMl(input);

        var expected = output
            .Replace(">-<", ">\u2212<")
            .Replace(">〈<", ">\u2329<")
            .Replace(">〉<", ">\u232A<");

        converted.Should().Be($"<math>{expected}</math>");
    }
}