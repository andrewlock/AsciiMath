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

    [Theory]
    [MemberData(nameof(TestSpec.MathMlTests), MemberType = typeof(TestSpec))]
    public void ConvertsToMathMlCorrectly(string asciiMath)
    {
        var spec = TestSpec.Specs[asciiMath];
        var converted = Parser.ToMathMl(spec.asciiMath);

        converted.Should().Be(spec.mathml);
    }

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