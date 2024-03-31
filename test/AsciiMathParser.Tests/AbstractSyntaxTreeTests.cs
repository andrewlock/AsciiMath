using FluentAssertions;

namespace AsciiMathParser.Tests;

public class AbstractSyntaxTreeTests
{
    [Fact]
    public void ValueNodeEquality()
    {
        // Same
        new SymbolNode(Symbol.abs, "test".AsMemory(), TokenType.Symbol).Should()
            .Be(new SymbolNode(Symbol.abs, "test".AsMemory(), TokenType.Symbol));
        new NumberNode("123".AsMemory()).Should().Be(new NumberNode("123".AsMemory()));
        new TextNode("123".AsMemory()).Should().Be(new TextNode("123".AsMemory()));
        new IdentifierNode("123".AsMemory()).Should().Be(new IdentifierNode("123".AsMemory()));
        
        // Different
        new SymbolNode(Symbol.abs, "test".AsMemory(), TokenType.Symbol).Should()
            .NotBe(new SymbolNode(Symbol.because, "test".AsMemory(), TokenType.Symbol));
        new SymbolNode(Symbol.abs, "test1".AsMemory(), TokenType.Symbol).Should()
            .NotBe(new SymbolNode(Symbol.abs, "test2".AsMemory(), TokenType.Symbol));
        new SymbolNode(Symbol.abs, "test".AsMemory(), TokenType.Symbol).Should()
            .NotBe(new SymbolNode(Symbol.abs, "test".AsMemory(), TokenType.Binary));
        new NumberNode("123".AsMemory()).Should().NotBe(new NumberNode("456".AsMemory()));
        new TextNode("123".AsMemory()).Should().NotBe(new TextNode("456".AsMemory()));
        new IdentifierNode("123".AsMemory()).Should().NotBe(new IdentifierNode("abc".AsMemory()));

        // Different types
        var original = new NumberNode("test".AsMemory());
        original.Should().NotBe(new SymbolNode(Symbol.abs, "test".AsMemory(), TokenType.Symbol));
        original.Should().NotBe(new TextNode("test".AsMemory()));
        original.Should().NotBe(new IdentifierNode("test".AsMemory()));
    }

    [Fact]
    public void IdenticalSequencesConsideredEqual()
    {
        var seq1 = new SequenceNode([
            new NumberNode("123".AsMemory()),
            new TextNode("456".AsMemory()),
            new IdentifierNode("test".AsMemory()),
        ]);
        var seq2 = new SequenceNode([
            new NumberNode("123".AsMemory()),
            new TextNode("456".AsMemory()),
            new IdentifierNode("test".AsMemory()),
        ]);

        seq1.Should().Be(seq2);
    }
    [Fact]
    public void EmptySequencesConsideredEqual()
    {
        var seq1 = new SequenceNode([]);
        var seq2 = new SequenceNode([]);

        seq1.Should().Be(seq2);
    }

    [Fact]
    public void DifferentSequencesConsideredUnequal_DifferentOrder()
    {
        var seq1 = new SequenceNode([
            new NumberNode("123".AsMemory()),
            new TextNode("456".AsMemory()),
            new IdentifierNode("test".AsMemory()),
        ]);
        var seq2 = new SequenceNode([
            new TextNode("456".AsMemory()),
            new NumberNode("123".AsMemory()),
            new IdentifierNode("test".AsMemory()),
        ]);

        seq1.Should().NotBe(seq2);
    }

    [Fact]
    public void DifferentSequencesConsideredUnequal_DifferentElements()
    {
        var seq1 = new SequenceNode([
            new TextNode("456".AsMemory()),
            new TextNode("123".AsMemory()),
            new IdentifierNode("test".AsMemory()),
        ]);
        var seq2 = new SequenceNode([
            new TextNode("456".AsMemory()),
            new NumberNode("123".AsMemory()),
            new IdentifierNode("test".AsMemory()),
        ]);

        seq1.Should().NotBe(seq2);
    }

    [Fact]
    public void ParenNodeEquality()
    {
        var lParen = new SymbolNode(Symbol.lparen, "(".AsMemory(), TokenType.LeftParen);
        var rParen = new SymbolNode(Symbol.rparen, ")".AsMemory(), TokenType.RightParen);
        var e1 = new TextNode("123".AsMemory());
        var e2 = new TextNode("123".AsMemory());
        var e3 = new NumberNode("123".AsMemory());

        // Same
        new ParenNode(lParen, e1, rParen).Should().Be(new ParenNode(lParen, e2, rParen));
        new ParenNode(null, e1, null).Should().Be(new ParenNode(null, e2, null));
        new ParenNode(lParen, e1, null).Should().Be(new ParenNode(lParen, e2, null));
        new ParenNode(null, e1, rParen).Should().Be(new ParenNode(null, e2, rParen));
        
        // Different
        new ParenNode(lParen, e1, rParen).Should().NotBe(new ParenNode(null, e2, null));
        new ParenNode(lParen, e1, rParen).Should().NotBe(new ParenNode(lParen, e2, null));
        new ParenNode(lParen, e1, rParen).Should().NotBe(new ParenNode(null, e2, rParen));
        new ParenNode(lParen, e1, rParen).Should().NotBe(new ParenNode(lParen, e3, rParen));
    }

    [Fact]
    public void GroupNodeEquality()
    {
        var lParen = new SymbolNode(Symbol.lparen, "(".AsMemory(), TokenType.LeftParen);
        var rParen = new SymbolNode(Symbol.rparen, ")".AsMemory(), TokenType.RightParen);
        var e1 = new TextNode("123".AsMemory());
        var e2 = new TextNode("123".AsMemory());
        var e3 = new NumberNode("123".AsMemory());

        // Same
        new GroupNode(lParen, e1, rParen).Should().Be(new GroupNode(lParen, e2, rParen));
        new GroupNode(null, e1, null).Should().Be(new GroupNode(null, e2, null));
        new GroupNode(lParen, e1, null).Should().Be(new GroupNode(lParen, e2, null));
        new GroupNode(null, e1, rParen).Should().Be(new GroupNode(null, e2, rParen));
        
        // Different
        new GroupNode(lParen, e1, rParen).Should().NotBe(new GroupNode(null, e2, null));
        new GroupNode(lParen, e1, rParen).Should().NotBe(new GroupNode(lParen, e2, null));
        new GroupNode(lParen, e1, rParen).Should().NotBe(new GroupNode(null, e2, rParen));
        new GroupNode(lParen, e1, rParen).Should().NotBe(new GroupNode(lParen, e3, rParen));
    }

    [Fact]
    public void SubSupNodeEquality()
    {
        var sym1 = new SymbolNode(Symbol.lparen, "(".AsMemory(), TokenType.LeftParen);
        var sym2 = new SymbolNode(Symbol.lparen, "(".AsMemory(), TokenType.LeftParen);
        var sub1 = new TextNode("123".AsMemory());
        var sub2 = new TextNode("123".AsMemory());
        var sup1 = new NumberNode("123".AsMemory());
        var sup2 = new NumberNode("123".AsMemory());

        var sym3 = new SymbolNode(Symbol.rparen, "-".AsMemory(), TokenType.LeftParen);

        // Same
        new SubSupNode(sym1, sub1, sup1).Should().Be(new SubSupNode(sym2, sub2, sup2));
        new SubSupNode(sym1, null, null).Should().Be(new SubSupNode(sym2, null, null));
        new SubSupNode(sym1, sub1, null).Should().Be(new SubSupNode(sym2, sub2, null));
        new SubSupNode(sym1, null, sup1).Should().Be(new SubSupNode(sym2, null, sup2));
        
        // Different
        new SubSupNode(sym1, sub1, sup1).Should().NotBe(new SubSupNode(sym2, sub2, sym3));
        new SubSupNode(sym1, sub1, sup1).Should().NotBe(new SubSupNode(sym2, sym3, sup2));
        new SubSupNode(sym1, sub1, sup1).Should().NotBe(new SubSupNode(sym3, sub2, sup2));
        new SubSupNode(sym1, sub1, null).Should().NotBe(new SubSupNode(sym2, sub2, sup2));
        new SubSupNode(sym1, null, sup1).Should().NotBe(new SubSupNode(sym2, sub2, sup2));
        new SubSupNode(sym1, sub1, sup1).Should().NotBe(new SubSupNode(sym2, null, sup2));
        new SubSupNode(sym1, sub1, sup1).Should().NotBe(new SubSupNode(sym2, sub2, null));
    }

    [Fact]
    public void BinaryNodeEquality()
    {
        var sym1 = new SymbolNode(Symbol.lparen, "(".AsMemory(), TokenType.Binary);
        var sym2 = new SymbolNode(Symbol.lparen, "(".AsMemory(), TokenType.Binary);
        var sub1 = new TextNode("123".AsMemory());
        var sub2 = new TextNode("123".AsMemory());
        var sup1 = new NumberNode("123".AsMemory());
        var sup2 = new NumberNode("123".AsMemory());

        var sym3 = new SymbolNode(Symbol.rparen, "-".AsMemory(), TokenType.Binary);

        // Same
        new BinaryOpNode(sym1, sub1, sup1).Should().Be(new BinaryOpNode(sym2, sub2, sup2));
        
        // Different
        new BinaryOpNode(sym1, sub1, sup1).Should().NotBe(new BinaryOpNode(sym2, sub2, sym3));
        new BinaryOpNode(sym1, sub1, sup1).Should().NotBe(new BinaryOpNode(sym2, sym3, sup2));
        new BinaryOpNode(sym1, sub1, sup1).Should().NotBe(new BinaryOpNode(sym3, sub2, sup2));
    }

    [Fact]
    public void InfixNodeEquality()
    {
        var sym1 = new SymbolNode(Symbol.lparen, "(".AsMemory(), TokenType.Infix);
        var sym2 = new SymbolNode(Symbol.lparen, "(".AsMemory(), TokenType.Infix);
        var sub1 = new TextNode("123".AsMemory());
        var sub2 = new TextNode("123".AsMemory());
        var sup1 = new NumberNode("123".AsMemory());
        var sup2 = new NumberNode("123".AsMemory());

        var sym3 = new SymbolNode(Symbol.rparen, "-".AsMemory(), TokenType.Infix);

        // Same
        new InfixOpNode(sym1, sub1, sup1).Should().Be(new InfixOpNode(sym2, sub2, sup2));
        
        // Different
        new InfixOpNode(sym1, sub1, sup1).Should().NotBe(new InfixOpNode(sym2, sub2, sym3));
        new InfixOpNode(sym1, sub1, sup1).Should().NotBe(new InfixOpNode(sym2, sym3, sup2));
        new InfixOpNode(sym1, sub1, sup1).Should().NotBe(new InfixOpNode(sym3, sub2, sup2));
    }
    
    [Fact]
    public void UnaryNodeEquality()
    {
        var sym1 = new SymbolNode(Symbol.lparen, "(".AsMemory(), TokenType.Unary);
        var sym2 = new SymbolNode(Symbol.lparen, "(".AsMemory(), TokenType.Unary);
        var operand1 = new TextNode("123".AsMemory());
        var operand2 = new TextNode("123".AsMemory());

        var sym3 = new SymbolNode(Symbol.rparen, "-".AsMemory(), TokenType.Unary);

        // Same
        new UnaryOpNode(sym1, operand1).Should().Be(new UnaryOpNode(sym2, operand2));
        
        // Different
        new UnaryOpNode(sym1, operand1).Should().NotBe(new UnaryOpNode(sym2, sym2));
        new UnaryOpNode(sym1, operand1).Should().NotBe(new UnaryOpNode(sym3, operand2));
    }
}