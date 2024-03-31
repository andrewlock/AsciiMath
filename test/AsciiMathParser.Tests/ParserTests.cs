using FluentAssertions;

namespace AsciiMathParser.Tests;

public class ParserTests
{
    [Theory]
    [MemberData(nameof(TestSpec.AstTests), MemberType = typeof(TestSpec))]
    public void ParsesAstCorrectly(object testArg)
    {
        var spec = (TestSpec)testArg;
        var parsed = Parser.Parse(spec.asciiMath);
        parsed.Should().NotBeNull().And.Be(spec.ast);
    }
    
}