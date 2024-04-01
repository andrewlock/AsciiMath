using FluentAssertions;

namespace AsciiMathParser.Tests;

public class ParserTests
{
    [Theory]
    [MemberData(nameof(TestSpec.AstTests), MemberType = typeof(TestSpec))]
    public void ParsesAstCorrectly(string asciiMath)
    {
        var spec = TestSpec.Specs[asciiMath];
        var parsed = Parser.Parse(spec.asciiMath);
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
        var parsed = Parser.Parse(spec.asciiMath);
        var converted = MarkupBuilder.AppendExpression(parsed, []);

        converted.Should().Be(spec.mathml);
    }
}