using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace AsciiMath;

internal class MathMlMarkupBuilder
{
    internal static MathMlMarkupBuilder Instance = new();

    private readonly StringBuilder _sb = new();
    private readonly RowMode _defaultRowMode;
    private readonly bool _escapeNonAscii;

    public MathMlMarkupBuilder(RowMode rowMode = RowMode.Avoid, bool escapeNonAscii = true)
    {
        _defaultRowMode = rowMode;
        _escapeNonAscii = escapeNonAscii;
    }

    protected static bool TryGetSymbol(Symbol symbol, [NotNullWhen(true)] out DisplayDetail? entry, bool fixPhi = true)
    {
        // https://github.com/asciidoctor/asciimath/issues/52
        entry = (symbol, fixPhi) switch
        {
            (Symbol.plus, _) => new("+", DisplaySymbolType.Operator),
            (Symbol.minus, _) => new("\u2212", DisplaySymbolType.Operator),
            (Symbol.cdot, _) => new("\u22C5", DisplaySymbolType.Operator),
            (Symbol.ast, _) => new("\u002A", DisplaySymbolType.Operator),
            (Symbol.star, _) => new("\u22C6", DisplaySymbolType.Operator),
            (Symbol.slash, _) => new("/", DisplaySymbolType.Operator),
            (Symbol.backslash, _) => new("\\", DisplaySymbolType.Operator),
            (Symbol.setminus, _) => new("\\", DisplaySymbolType.Operator),
            (Symbol.times, _) => new("\u00D7", DisplaySymbolType.Operator),
            (Symbol.ltimes, _) => new("\u22C9", DisplaySymbolType.Operator),
            (Symbol.rtimes, _) => new("\u22CA", DisplaySymbolType.Operator),
            (Symbol.bowtie, _) => new("\u22C8", DisplaySymbolType.Operator),
            (Symbol.div, _) => new("\u00F7", DisplaySymbolType.Operator),
            (Symbol.circ, _) => new("\u26AC", DisplaySymbolType.Operator),
            (Symbol.oplus, _) => new("\u2295", DisplaySymbolType.Operator),
            (Symbol.otimes, _) => new("\u2297", DisplaySymbolType.Operator),
            (Symbol.odot, _) => new("\u2299", DisplaySymbolType.Operator),
            (Symbol.sum, _) => new("\u2211", DisplaySymbolType.Operator, IsUnderOver: true),
            (Symbol.prod, _) => new("\u220F", DisplaySymbolType.Operator, IsUnderOver: true),
            (Symbol.wedge, _) => new("\u2227", DisplaySymbolType.Operator),
            (Symbol.bigwedge, _) => new("\u22C0", DisplaySymbolType.Operator, IsUnderOver: true),
            (Symbol.vee, _) => new("\u2228", DisplaySymbolType.Operator),
            (Symbol.bigvee, _) => new("\u22C1", DisplaySymbolType.Operator, IsUnderOver: true),
            (Symbol.cap, _) => new("\u2229", DisplaySymbolType.Operator),
            (Symbol.bigcap, _) => new("\u22C2", DisplaySymbolType.Operator, IsUnderOver: true),
            (Symbol.cup, _) => new("\u222A", DisplaySymbolType.Operator),
            (Symbol.bigcup, _) => new("\u22C3", DisplaySymbolType.Operator, IsUnderOver: true),

            // # Relation symbols
            (Symbol.eq, _) => new("=", DisplaySymbolType.Operator),
            (Symbol.ne, _) => new("\u2260", DisplaySymbolType.Operator),
            (Symbol.assign, _) => new("\u2254", DisplaySymbolType.Operator),
            (Symbol.lt, _) => new("\u003C", DisplaySymbolType.Operator),
            (Symbol.mlt, _) => new("\u226A", DisplaySymbolType.Operator),
            (Symbol.gt, _) => new("\u003E", DisplaySymbolType.Operator),
            (Symbol.mgt, _) => new("\u226B", DisplaySymbolType.Operator),
            (Symbol.le, _) => new("\u2264", DisplaySymbolType.Operator),
            (Symbol.ge, _) => new("\u2265", DisplaySymbolType.Operator),
            (Symbol.prec, _) => new("\u227A", DisplaySymbolType.Operator),
            (Symbol.succ, _) => new("\u227B", DisplaySymbolType.Operator),
            (Symbol.preceq, _) => new("\u2AAF", DisplaySymbolType.Operator),
            (Symbol.succeq, _) => new("\u2AB0", DisplaySymbolType.Operator),
            (Symbol.@in, _) => new("\u2208", DisplaySymbolType.Operator),
            (Symbol.notin, _) => new("\u2209", DisplaySymbolType.Operator),
            (Symbol.subset, _) => new("\u2282", DisplaySymbolType.Operator),
            (Symbol.supset, _) => new("\u2283", DisplaySymbolType.Operator),
            (Symbol.subseteq, _) => new("\u2286", DisplaySymbolType.Operator),
            (Symbol.supseteq, _) => new("\u2287", DisplaySymbolType.Operator),
            (Symbol.equiv, _) => new("\u2261", DisplaySymbolType.Operator),
            (Symbol.sim, _) => new("\u223C", DisplaySymbolType.Operator),
            (Symbol.cong, _) => new("\u2245", DisplaySymbolType.Operator),
            (Symbol.approx, _) => new("\u2248", DisplaySymbolType.Operator),
            (Symbol.propto, _) => new("\u221D", DisplaySymbolType.Operator),

            // # Logical symbols
            (Symbol.and, _) => new("and", DisplaySymbolType.Text),
            (Symbol.or, _) => new("or", DisplaySymbolType.Text),
            (Symbol.not, _) => new("\u00AC", DisplaySymbolType.Operator),
            (Symbol.implies, _) => new("\u21D2", DisplaySymbolType.Operator),
            (Symbol.@if, _) => new("if", DisplaySymbolType.Operator),
            (Symbol.iff, _) => new("\u21D4", DisplaySymbolType.Operator),
            (Symbol.forall, _) => new("\u2200", DisplaySymbolType.Operator),
            (Symbol.exists, _) => new("\u2203", DisplaySymbolType.Operator),
            (Symbol.bot, _) => new("\u22A5", DisplaySymbolType.Operator),
            (Symbol.top, _) => new("\u22A4", DisplaySymbolType.Operator),
            (Symbol.vdash, _) => new("\u22A2", DisplaySymbolType.Operator),
            (Symbol.models, _) => new("\u22A8", DisplaySymbolType.Operator),

            // # Grouping brackets
            (Symbol.lparen, _) => new("(", DisplaySymbolType.LeftParen),
            (Symbol.rparen, _) => new(")", DisplaySymbolType.RightParen),
            (Symbol.lbracket, _) => new("[", DisplaySymbolType.LeftParen),
            (Symbol.rbracket, _) => new("]", DisplaySymbolType.RightParen),
            (Symbol.lbrace, _) => new("{", DisplaySymbolType.LeftParen),
            (Symbol.rbrace, _) => new("}", DisplaySymbolType.RightParen),
            (Symbol.vbar, _) => new("|", DisplaySymbolType.LeftRightParen),
            (Symbol.langle, _) => new("\u2329", DisplaySymbolType.LeftParen),
            (Symbol.rangle, _) => new("\u232A", DisplaySymbolType.RightParen),
            (Symbol.parallel, _) => new( "\u2225", DisplaySymbolType.LeftRightParen),

            // # Miscellaneous symbols
            (Symbol.integral, _) => new("\u222B", DisplaySymbolType.Operator),
            (Symbol.dx, _) => new("dx", DisplaySymbolType.Identifier),
            (Symbol.dy, _) => new("dy", DisplaySymbolType.Identifier),
            (Symbol.dz, _) => new("dz", DisplaySymbolType.Identifier),
            (Symbol.dt, _) => new("dt", DisplaySymbolType.Identifier),
            (Symbol.contourintegral, _) => new("\u222E", DisplaySymbolType.Operator),
            (Symbol.partial, _) => new("\u2202", DisplaySymbolType.Operator),
            (Symbol.nabla, _) => new("\u2207", DisplaySymbolType.Operator),
            (Symbol.pm, _) => new("\u00B1", DisplaySymbolType.Operator),
            (Symbol.mp, _) => new("\u2213", DisplaySymbolType.Operator),
            (Symbol.emptyset, _) => new("\u2205", DisplaySymbolType.Operator),
            (Symbol.infty, _) => new("\u221E", DisplaySymbolType.Operator),
            (Symbol.aleph, _) => new("\u2135", DisplaySymbolType.Operator),
            (Symbol.ellipsis, _) => new("\u2026", DisplaySymbolType.Operator),
            (Symbol.therefore, _) => new("\u2234", DisplaySymbolType.Operator),
            (Symbol.because, _) => new("\u2235", DisplaySymbolType.Operator),
            (Symbol.angle, _) => new("\u2220", DisplaySymbolType.Operator),
            (Symbol.triangle, _) => new("\u25B3", DisplaySymbolType.Operator),
            (Symbol.prime, _) => new("\u2032", DisplaySymbolType.Operator),
            (Symbol.tilde, _) => new("~", DisplaySymbolType.Accent, Position: Position.Over),
            (Symbol.nbsp, _) => new("\u00A0", DisplaySymbolType.Operator),
            (Symbol.frown, _) => new("\u2322", DisplaySymbolType.Operator),
            (Symbol.quad, _) => new("\u00A0\u00A0", DisplaySymbolType.Operator),
            (Symbol.qquad, _) => new("\u00A0\u00A0\u00A0\u00A0", DisplaySymbolType.Operator),
            (Symbol.cdots, _) => new("\u22EF", DisplaySymbolType.Operator),
            (Symbol.vdots, _) => new("\u22EE", DisplaySymbolType.Operator),
            (Symbol.ddots, _) => new("\u22F1", DisplaySymbolType.Operator),
            (Symbol.diamond, _) => new("\u22C4", DisplaySymbolType.Operator),
            (Symbol.square, _) => new("\u25A1", DisplaySymbolType.Operator),
            (Symbol.lfloor, _) => new("\u230A", DisplaySymbolType.Operator),
            (Symbol.rfloor, _) => new("\u230B", DisplaySymbolType.Operator),
            (Symbol.lceiling, _) => new("\u2308", DisplaySymbolType.Operator),
            (Symbol.rceiling, _) => new("\u2309", DisplaySymbolType.Operator),
            (Symbol.dstruck_captial_c, _) => new("\u2102", DisplaySymbolType.Operator),
            (Symbol.dstruck_captial_n, _) => new("\u2115", DisplaySymbolType.Operator),
            (Symbol.dstruck_captial_q, _) => new("\u211A", DisplaySymbolType.Operator),
            (Symbol.dstruck_captial_r, _) => new("\u211D", DisplaySymbolType.Operator),
            (Symbol.dstruck_captial_z, _) => new("\u2124", DisplaySymbolType.Operator),
            (Symbol.f, _) => new("f", DisplaySymbolType.Identifier),
            (Symbol.g, _) => new("g", DisplaySymbolType.Identifier),

            // # Standard functions
            (Symbol.lim, _) => new("lim", DisplaySymbolType.Operator, IsUnderOver: true),
            (Symbol.Lim, _) => new("Lim", DisplaySymbolType.Operator, IsUnderOver: true),
            (Symbol.min, _) => new("min", DisplaySymbolType.Operator, IsUnderOver: true),
            (Symbol.max, _) => new("max", DisplaySymbolType.Operator, IsUnderOver: true),
            (Symbol.sin, _) => new("sin", DisplaySymbolType.Identifier),
            (Symbol.Sin, _) => new("Sin", DisplaySymbolType.Identifier),
            (Symbol.cos, _) => new("cos", DisplaySymbolType.Identifier),
            (Symbol.Cos, _) => new("Cos", DisplaySymbolType.Identifier),
            (Symbol.tan, _) => new("tan", DisplaySymbolType.Identifier),
            (Symbol.Tan, _) => new("Tan", DisplaySymbolType.Identifier),
            (Symbol.sinh, _) => new("sinh", DisplaySymbolType.Identifier),
            (Symbol.Sinh, _) => new("Sinh", DisplaySymbolType.Identifier),
            (Symbol.cosh, _) => new("cosh", DisplaySymbolType.Identifier),
            (Symbol.Cosh, _) => new("Cosh", DisplaySymbolType.Identifier),
            (Symbol.tanh, _) => new("tanh", DisplaySymbolType.Identifier),
            (Symbol.Tanh, _) => new("Tanh", DisplaySymbolType.Identifier),
            (Symbol.cot, _) => new("cot", DisplaySymbolType.Identifier),
            (Symbol.Cot, _) => new("Cot", DisplaySymbolType.Identifier),
            (Symbol.sec, _) => new("sec", DisplaySymbolType.Identifier),
            (Symbol.Sec, _) => new("Sec", DisplaySymbolType.Identifier),
            (Symbol.csc, _) => new("csc", DisplaySymbolType.Identifier),
            (Symbol.Csc, _) => new("Csc", DisplaySymbolType.Identifier),
            (Symbol.arcsin, _) => new("arcsin", DisplaySymbolType.Identifier),
            (Symbol.arccos, _) => new("arccos", DisplaySymbolType.Identifier),
            (Symbol.arctan, _) => new("arctan", DisplaySymbolType.Identifier),
            (Symbol.coth, _) => new("coth", DisplaySymbolType.Identifier),
            (Symbol.sech, _) => new("sech", DisplaySymbolType.Identifier),
            (Symbol.csch, _) => new("csch", DisplaySymbolType.Identifier),
            (Symbol.exp, _) => new("exp", DisplaySymbolType.Identifier),
            (Symbol.abs, _) => new("abs", DisplaySymbolType.Wrap, WrapLParen: "|", WrapRParen: "|"),
            (Symbol.norm, _) => new("norm", DisplaySymbolType.Wrap, WrapLParen: "\u2225", WrapRParen: "\u2225"),
            (Symbol.floor, _) => new("floor", DisplaySymbolType.Wrap, WrapLParen: "\u230A", WrapRParen: "\u230B"),
            (Symbol.ceil, _) => new("ceil", DisplaySymbolType.Wrap, WrapLParen: "\u2308", WrapRParen: "\u2309"),
            (Symbol.log, _) => new("log", DisplaySymbolType.Identifier),
            (Symbol.Log, _) => new("Log", DisplaySymbolType.Identifier),
            (Symbol.ln, _) => new("ln", DisplaySymbolType.Identifier),
            (Symbol.Ln, _) => new("Ln", DisplaySymbolType.Identifier),
            (Symbol.det, _) => new("det", DisplaySymbolType.Identifier),
            (Symbol.dim, _) => new("dim", DisplaySymbolType.Identifier),
            (Symbol.ker, _) => new("ker", DisplaySymbolType.Identifier),
            (Symbol.mod, _) => new("mod", DisplaySymbolType.Identifier),
            (Symbol.gcd, _) => new("gcd", DisplaySymbolType.Identifier),
            (Symbol.lcm, _) => new("lcm", DisplaySymbolType.Identifier),
            (Symbol.lub, _) => new("lub", DisplaySymbolType.Identifier),
            (Symbol.glb, _) => new("glb", DisplaySymbolType.Identifier),

            // # Arrows
            (Symbol.uparrow, _) => new("\u2191", DisplaySymbolType.Operator),
            (Symbol.downarrow, _) => new("\u2193", DisplaySymbolType.Operator),
            (Symbol.rightarrow, _) => new("\u2192", DisplaySymbolType.Operator),
            (Symbol.to, _) => new("\u2192", DisplaySymbolType.Operator),
            (Symbol.rightarrowtail, _) => new("\u21A3", DisplaySymbolType.Operator),
            (Symbol.twoheadrightarrow, _) => new("\u21A0", DisplaySymbolType.Operator),
            (Symbol.twoheadrightarrowtail, _) => new("\u2916", DisplaySymbolType.Operator),
            (Symbol.mapsto, _) => new("\u21A6", DisplaySymbolType.Operator),
            (Symbol.leftarrow, _) => new("\u2190", DisplaySymbolType.Operator),
            (Symbol.leftrightarrow, _) => new("\u2194", DisplaySymbolType.Operator),
            (Symbol.Rightarrow, _) => new("\u21D2", DisplaySymbolType.Operator),
            (Symbol.Leftarrow, _) => new("\u21D0", DisplaySymbolType.Operator),
            (Symbol.Leftrightarrow, _) => new("\u21D4", DisplaySymbolType.Operator),

            // # Unary tags
            (Symbol.sqrt, _) => new(null!, DisplaySymbolType.Sqrt),
            (Symbol.cancel, _) => new(null!, DisplaySymbolType.Cancel),

            // # Binary tags
            (Symbol.root, _) => new(null!, DisplaySymbolType.Root),
            (Symbol.frac, _) => new(null!, DisplaySymbolType.Frac),
            (Symbol.stackrel, _) => new(null!, DisplaySymbolType.Over),
            (Symbol.overset, _) => new(null!, DisplaySymbolType.Over),
            (Symbol.underset, _) => new(null!, DisplaySymbolType.Under),
            (Symbol.color, _) => new(null!, DisplaySymbolType.Color),

            (Symbol.sub, _) => new("_", DisplaySymbolType.Operator),
            (Symbol.sup, _) => new("^", DisplaySymbolType.Operator),
            (Symbol.hat, _) => new("\u005E", DisplaySymbolType.Accent, Position: Position.Over),
            (Symbol.overline, _) => new("\u00AF", DisplaySymbolType.Accent, Position: Position.Over),
            (Symbol.vec, _) => new("\u2192", DisplaySymbolType.Accent, Position: Position.Over),
            (Symbol.dot, _) => new(".", DisplaySymbolType.Accent, Position: Position.Over),
            (Symbol.ddot, _) => new("..", DisplaySymbolType.Accent, Position: Position.Over),
            (Symbol.overarc, _) => new("\u23DC", DisplaySymbolType.Accent, Position: Position.Over),
            (Symbol.underline, _) => new("_", DisplaySymbolType.Accent, Position: Position.Under),
            (Symbol.underbrace, _) => new("\u23DF", DisplaySymbolType.Accent, Position: Position.Under,
                IsUnderOver: true),
            (Symbol.overbrace, _) => new("\u23DE", DisplaySymbolType.Accent, Position: Position.Over,
                IsUnderOver: true),
            (Symbol.bold, _) => new("bold", DisplaySymbolType.Font),
            (Symbol.double_struck, _) => new("double-struck", DisplaySymbolType.Font),
            (Symbol.italic, _) => new("italic", DisplaySymbolType.Font),
            (Symbol.bold_italic, _) => new("bold-italic", DisplaySymbolType.Font),
            (Symbol.script, _) => new("script", DisplaySymbolType.Font),
            (Symbol.bold_script, _) => new("bold-script", DisplaySymbolType.Font),
            (Symbol.monospace, _) => new("monospace", DisplaySymbolType.Font),
            (Symbol.fraktur, _) => new("fraktur", DisplaySymbolType.Font),
            (Symbol.bold_fraktur, _) => new("bold-fraktur", DisplaySymbolType.Font),
            (Symbol.sans_serif, _) => new("sans-serif", DisplaySymbolType.Font),
            (Symbol.bold_sans_serif, _) => new("bold-sans-serif", DisplaySymbolType.Font),
            (Symbol.sans_serif_italic, _) => new("sans-serif-italic", DisplaySymbolType.Font),
            (Symbol.sans_serif_bold_italic, _) => new("sans-serif-bold-italic", DisplaySymbolType.Font),
            (Symbol.roman, _) => new("normal", DisplaySymbolType.Font),

            // # Greek letters
            (Symbol.alpha, _) => new("\u03b1", DisplaySymbolType.Identifier),
            (Symbol.Alpha, _) => new("\u0391", DisplaySymbolType.Identifier),
            (Symbol.beta, _) => new("\u03b2", DisplaySymbolType.Identifier),
            (Symbol.Beta, _) => new("\u0392", DisplaySymbolType.Identifier),
            (Symbol.gamma, _) => new("\u03b3", DisplaySymbolType.Identifier),
            (Symbol.Gamma, _) => new("\u0393", DisplaySymbolType.Operator),
            (Symbol.delta, _) => new("\u03b4", DisplaySymbolType.Identifier),
            (Symbol.Delta, _) => new("\u0394", DisplaySymbolType.Operator),
            (Symbol.epsilon, _) => new("\u03b5", DisplaySymbolType.Identifier),
            (Symbol.Epsilon, _) => new("\u0395", DisplaySymbolType.Identifier),
            (Symbol.varepsilon, _) => new("\u025b", DisplaySymbolType.Identifier),
            (Symbol.zeta, _) => new("\u03b6", DisplaySymbolType.Identifier),
            (Symbol.Zeta, _) => new("\u0396", DisplaySymbolType.Identifier),
            (Symbol.eta, _) => new("\u03b7", DisplaySymbolType.Identifier),
            (Symbol.Eta, _) => new("\u0397", DisplaySymbolType.Identifier),
            (Symbol.theta, _) => new("\u03b8", DisplaySymbolType.Identifier),
            (Symbol.Theta, _) => new("\u0398", DisplaySymbolType.Operator),
            (Symbol.vartheta, _) => new("\u03d1", DisplaySymbolType.Identifier),
            (Symbol.iota, _) => new("\u03b9", DisplaySymbolType.Identifier),
            (Symbol.Iota, _) => new("\u0399", DisplaySymbolType.Identifier),
            (Symbol.kappa, _) => new("\u03ba", DisplaySymbolType.Identifier),
            (Symbol.Kappa, _) => new("\u039a", DisplaySymbolType.Identifier),
            (Symbol.lambda, _) => new("\u03bb", DisplaySymbolType.Identifier),
            (Symbol.Lambda, _) => new("\u039b", DisplaySymbolType.Operator),
            (Symbol.mu, _) => new("\u03bc", DisplaySymbolType.Identifier),
            (Symbol.Mu, _) => new("\u039c", DisplaySymbolType.Identifier),
            (Symbol.nu, _) => new("\u03bd", DisplaySymbolType.Identifier),
            (Symbol.Nu, _) => new("\u039d", DisplaySymbolType.Identifier),
            (Symbol.xi, _) => new("\u03be", DisplaySymbolType.Identifier),
            (Symbol.Xi, _) => new("\u039e", DisplaySymbolType.Operator),
            (Symbol.omicron, _) => new("\u03bf", DisplaySymbolType.Identifier),
            (Symbol.Omicron, _) => new("\u039f", DisplaySymbolType.Identifier),
            (Symbol.pi, _) => new("\u03c0", DisplaySymbolType.Identifier),
            (Symbol.Pi, _) => new("\u03a0", DisplaySymbolType.Operator),
            (Symbol.rho, _) => new("\u03c1", DisplaySymbolType.Identifier),
            (Symbol.Rho, _) => new("\u03a1", DisplaySymbolType.Identifier),
            (Symbol.sigma, _) => new("\u03c3", DisplaySymbolType.Identifier),
            (Symbol.Sigma, _) => new("\u03a3", DisplaySymbolType.Operator),
            (Symbol.tau, _) => new("\u03c4", DisplaySymbolType.Identifier),
            (Symbol.Tau, _) => new("\u03a4", DisplaySymbolType.Identifier),
            (Symbol.upsilon, _) => new("\u03c5", DisplaySymbolType.Identifier),
            (Symbol.Upsilon, _) => new("\u03a5", DisplaySymbolType.Identifier),
            (Symbol.phi, true) => new("\u03d5", DisplaySymbolType.Identifier),
            (Symbol.varphi, true) => new("\u03c6", DisplaySymbolType.Identifier),
            (Symbol.phi, false) => new("\u03c6", DisplaySymbolType.Identifier),
            (Symbol.varphi, false) => new("\u03d5", DisplaySymbolType.Identifier),
            (Symbol.Phi, _) => new("\u03a6", DisplaySymbolType.Identifier),
            (Symbol.chi, _) => new("\u03c7", DisplaySymbolType.Identifier),
            (Symbol.Chi, _) => new("\u03a7", DisplaySymbolType.Identifier),
            (Symbol.psi, _) => new("\u03c8", DisplaySymbolType.Identifier),
            (Symbol.Psi, _) => new("\u03a8", DisplaySymbolType.Identifier),
            (Symbol.omega, _) => new("\u03c9", DisplaySymbolType.Identifier),
            (Symbol.Omega, _) => new("\u03a9", DisplaySymbolType.Operator),
            _ => null,
        };

        return entry is not null;
    }

    public string Serialize(Node? ast, MathMlOptions options)
    {
        if (ast is null)
        {
            return string.Empty;
        }

        _sb.Clear();
        var element = options.IsBlock switch
        {
            null => "<math>",
            true => """<math display="block">""",
            false => """<math display="inline">""",
        };

        _sb.Append(element);

        Append(ast, RowMode.Omit);

        _sb.Append("</math>");
        return _sb.ToString();
    }

    void Append(Node? ast, RowMode rowMode = RowMode.Avoid)
    {
        if (rowMode == RowMode.Force)
        {
            if (ast is SequenceNode seq)
            {
                AppendRow(seq);
            }
            else
            {
                AppendRow(ast);
            }

            return;
        }

        switch (ast)
        {
            case SequenceNode seq:
                if ((seq.Count <= 0 && rowMode == RowMode.Avoid) || rowMode == RowMode.Omit)
                {
                    foreach (var node in seq)
                    {
                        Append(node);
                    }
                }
                else
                {
                    AppendRow(seq);
                }

                break;
            case GroupNode n:
                Append(n.Expression);
                break;
            case TextNode n:
                AppendText(n.Value);
                break;
            case NumberNode n:
                AppendNumber(n);
                break;
            case IdentifierNode n:
                AppendIdentifierOrOperator(n.Value);
                break;
            case SymbolNode n:
                if (n.Value is not null && TryGetSymbol(n.Value.Value, out var symbolDetail))
                {
                    if (symbolDetail.Value.Type is DisplaySymbolType.Operator
                        or DisplaySymbolType.Accent or DisplaySymbolType.LeftParen
                        or DisplaySymbolType.RightParen or DisplaySymbolType.LeftRightParen)
                    {
                        AppendOperator(symbolDetail.Value.Text.AsMemory());
                    }
                    else if (symbolDetail.Value.Type is DisplaySymbolType.Text)
                    {
                        AppendText(symbolDetail.Value.Text.AsMemory());
                    }
                    else
                    {
                        AppendIdentifier(symbolDetail.Value.Text.AsMemory());
                    }
                }
                else
                {
                    AppendIdentifierOrOperator(n.Text);
                }
                break;
            case ParenNode n:
                AppendFenced(n.LParen, n.Expression, n.RParen);
                break;
            case SubSupNode n:
                if (IsUnderOver(n.BaseExpression))
                {
                    AppendUnderOver(n.BaseExpression, n.SubExpression, n.SupExpression);
                }
                else
                {
                    AppendSubSup(n.BaseExpression, n.SubExpression, n.SupExpression);
                }

                break;
            case UnaryOpNode n:
                if (TryResolveSymbol(n.Operator) is { } uSymbol)
                {
                    switch (uSymbol.Type)
                    {
                        case DisplaySymbolType.Identifier:
                            AppendIdentifierUnary(uSymbol.Text.AsMemory(), n.Operand);
                            break;
                        case DisplaySymbolType.Operator:
                            AppendOperatorUnary(uSymbol.Text.AsMemory(), n.Operand);
                            break;
                        case DisplaySymbolType.Wrap:
                            var lParen = uSymbol.WrapLParen is { } left ? left.AsMemory() : ReadOnlyMemory<char>.Empty;
                            var rParen = uSymbol.WrapRParen is { } right ? right.AsMemory() : ReadOnlyMemory<char>.Empty;
                            AppendFenced(lParen , n.Operand, rParen);
                            break;
                        case DisplaySymbolType.Accent:
                            if (uSymbol.Position == Position.Over)
                            {
                                AppendUnderOver(n.Operand, null, n.Operator);
                            }
                            else
                            {
                                AppendUnderOver(n.Operand, n.Operator, null);
                            }

                            break;
                        case DisplaySymbolType.Font:
                            AppendFont(uSymbol.Text.AsMemory(), n.Operand);
                            break;
                        case DisplaySymbolType.Cancel:
                            AppendCancel(n.Operand);
                            break;
                        case DisplaySymbolType.Sqrt:
                            AppendSqrt(n.Operand);
                            break;
                        default:
                            throw new InvalidOperationException("Unexpected symbol: " + uSymbol);
                    }
                }

                break;
            
            case BinaryOpNode n:
                if (TryResolveSymbol(n.Operator) is { } bSymbol)
                {
                    switch (bSymbol.Type)
                    {
                        case DisplaySymbolType.Over:
                            AppendUnderOver(n.Operand2, null, n.Operand1);
                            break;
                        case DisplaySymbolType.Under:
                            AppendUnderOver(n.Operand2, n.Operand1, null);
                            break;
                        case DisplaySymbolType.Root:
                            AppendRoot(n.Operand2, n.Operand1);
                            break;
                        case DisplaySymbolType.Color when n.Operand1 is ColorNode c:
                            AppendColor(c.ToHexRgb().AsMemory(), n.Operand2);
                            break;
                        case DisplaySymbolType.Frac:
                            AppendFraction(n.Operand1, n.Operand2);
                            break;
                        default:
                            throw new InvalidOperationException("Unexpected symbol: " + bSymbol);
                    }
                }

                break;
            case InfixOpNode n:
                if (TryResolveSymbol(n.Operator) is { } iSymbol)
                {
                    switch (iSymbol.Type)
                    {
                        case DisplaySymbolType.Frac:
                            AppendFraction(n.Operand1, n.Operand2);
                            break;
                        default:
                            throw new InvalidOperationException("Unexpected symbol: " + iSymbol);
                    }
                }

                break;
            case MatrixNode n:
                AppendMatrix(n.LParen, n, n.RParen);
                break;
        }
    }

    bool TryResolveParen(SymbolNode? paren, out ReadOnlyMemory<char> value)
    {
        if (paren?.Value is null)
        {
            value = ReadOnlyMemory<char>.Empty;
            return false;
        }

        if (TryGetSymbol(paren.Value.Value, out var symbolDetail))
        {
            value = symbolDetail.Value.Text.AsMemory();
            return true;
        }

        value = paren.Text;
        return true;
    }

    bool IsUnderOver(Node node)
    {
        var symbol = node is UnaryOpNode u ? u.Operator : node;
        return TryResolveSymbol(symbol)?.IsUnderOver ?? false;
    }

    bool IsAccent(Node? node)
    {
        var symbol = node is UnaryOpNode u ? u.Operator : node;
        return TryResolveSymbol(symbol)?.Type == DisplaySymbolType.Accent;
    }

    DisplayDetail? TryResolveSymbol(Node? node)
        => node switch
        {
            SymbolNode s => TryResolveSymbol(s.Value),
            _ => null
        };
    DisplayDetail? TryResolveSymbol(Symbol? symbol)
        => symbol switch
        {
            { } s when TryGetSymbol(s, out var detail) => detail,
            _ => null,
        };

    void AppendText(ReadOnlyMemory<char> value) => _sb.Append("<mtext>").AppendEscapedText(value, _escapeNonAscii).Append("</mtext>");
    void AppendNumber(NumberNode node) => _sb.Append("<mn>").AppendEscapedText(node.Value, _escapeNonAscii).Append("</mn>");
    void AppendIdentifier(ReadOnlyMemory<char> value) => _sb.Append("<mi>").AppendEscapedText(value, _escapeNonAscii).Append("</mi>");
    void AppendOperator(ReadOnlyMemory<char> value) => _sb.Append("<mo>").AppendEscapedText(value, _escapeNonAscii).Append("</mo>");

    void AppendMatrix(SymbolNode? lParen, MatrixNode node, SymbolNode? rParen)
    {
        TryResolveParen(lParen, out var left);
        TryResolveParen(rParen, out var right);

        if (!left.IsEmpty || !right.IsEmpty)
        {
            _sb.Append("<mrow>");
            if (!left.IsEmpty)
            {
                _sb.Append("<mo>").AppendEscapedText(left, _escapeNonAscii).Append("</mo>");
            }
        }

        _sb.Append("<mtable>");
        foreach (var row in node)
        {
            _sb.Append("<mtr>");
            foreach (var cell in row)
            {
                _sb.Append("<mtd>");
                Append(cell);
                _sb.Append("</mtd>");
            }
            _sb.Append("</mtr>");
        }

        _sb.Append("</mtable>");

        if (!left.IsEmpty || !right.IsEmpty)
        {
            if (!right.IsEmpty)
            {
                _sb.Append("<mo>").AppendEscapedText(right, _escapeNonAscii).Append("</mo>");
            }

            _sb.Append("</mrow>");
        }
    }

    void AppendRow(Node? node)
    {
        _sb.Append("<mrow>");
        if (node is SequenceNode seq)
        {
            foreach (var n in seq)
            {
                Append(n);
            }            
        }
        else
        {
            Append(node);
        }
        

        _sb.Append("</mrow>");
    }
    void AppendIdentifierUnary(ReadOnlyMemory<char> identifier, Node? op)
    {
        _sb.Append("<mrow>");
        AppendIdentifier(identifier);
        Append(op, rowMode: _defaultRowMode);
        _sb.Append("</mrow>");
    }

    void AppendOperatorUnary(ReadOnlyMemory<char> identifier, Node? op)
    {
        _sb.Append("<mrow>");
        AppendOperator(identifier);
        Append(op, rowMode: _defaultRowMode);
        _sb.Append("</mrow>");
    }

    void AppendFont(ReadOnlyMemory<char> style, Node? e)
    {
        _sb.Append("<mstyle mathvariant=\"")
            .Append(style)
            .Append("\">");
        Append(e);
        _sb.Append("</mstyle>");
    }

    void AppendColor(ReadOnlyMemory<char> color, Node? e)
    {
        _sb.Append("<mstyle mathcolor=\"")
            .Append(color)
            .Append("\">");
        Append(e);
        _sb.Append("</mstyle>");
    }

    void AppendCancel(Node? e)
    {
        _sb.Append("<menclose notation=\"updiagonalstrike\">");
        Append(e, RowMode.Omit);
        _sb.Append("</menclose>");
    }

    void AppendSqrt(Node? e)
    {
        _sb.Append("<msqrt>");
        Append(e, _defaultRowMode);
        _sb.Append("</msqrt>");
    }

    void AppendRoot(Node? @base, Node? index)
    {
        _sb.Append("<mroot>");
        Append(@base, _defaultRowMode);
        Append(index, _defaultRowMode);
        _sb.Append("</mroot>");
    }

    void AppendFraction(Node? numerator, Node? denominator)
    {
        _sb.Append("<mfrac>");
        Append(numerator, _defaultRowMode);
        Append(denominator, _defaultRowMode);
        _sb.Append("</mfrac>");
    }
    
    // https://github.com/asciidoctor/asciimath/issues/58
    void AppendIdentifierOrOperator(ReadOnlyMemory<char> value)
    {
        if (value.IsEmpty || char.IsLetterOrDigit(value.Span[0]))
        {
            AppendIdentifier(value);
        }
        else
        {
            AppendOperator(value);
        }
    }

    void AppendFenced(
        SymbolNode? leftParen,
        Node? node,
        SymbolNode? rightParen)
    {
        TryResolveParen(leftParen, out var left);
        TryResolveParen(rightParen, out var right);
        AppendFenced(left, node, right);
    }

    void AppendFenced(
        ReadOnlyMemory<char> left,
        Node? node,
        ReadOnlyMemory<char> right)
    {
        if (left.IsEmpty && right.IsEmpty)
        {
            Append(node);
            return;
        }

        _sb.Append("<mrow>");
        if (!left.IsEmpty)
        {
            _sb.Append("<mo>").AppendEscapedText(left, _escapeNonAscii).Append("</mo>");
        }
        Append(node);
        if (!right.IsEmpty)
        {
            _sb.Append("<mo>").AppendEscapedText(right, _escapeNonAscii).Append("</mo>");
        }

        _sb.Append("</mrow>");
    }

    void AppendUnderOver(Node baseExpression, Node? sub, Node? sup)
    {
        var accentUnder = false;
        var accent = false;
        var subRowMode = _defaultRowMode;
        var supRowMode = _defaultRowMode;

        if (IsAccent(sub))
        {
            accentUnder = true;
            subRowMode = RowMode.Avoid;
        }

        if (IsAccent(sup))
        {
            accent = true;
            supRowMode = RowMode.Avoid;
        }

        if (sub is not null && sup is not null)
        {
            _sb.Append("<munderover");
            if (accent)
            {
                _sb.Append(" accent=\"true\"");
            }
            if (accentUnder)
            {
                _sb.Append(" accentunder=\"true\"");
            }

            _sb.Append('>');
            Append(baseExpression, _defaultRowMode);
            Append(sub, subRowMode);
            Append(sup, supRowMode);
            _sb.Append("</munderover>");
        }
        else if (sub is not null)
        {
            _sb.Append("<munder");
            if (accentUnder)
            {
                _sb.Append(" accentunder=\"true\"");
            }

            _sb.Append('>');
            Append(baseExpression, _defaultRowMode);
            Append(sub, subRowMode);
            _sb.Append("</munder>");
        }
        else if (sup is not null)
        {
            _sb.Append("<mover");
            if (accent)
            {
                _sb.Append(" accent=\"true\"");
            }

            _sb.Append('>');
            Append(baseExpression, _defaultRowMode);
            Append(sup, supRowMode);
            _sb.Append("</mover>");
        }
        else
        {
            Append(baseExpression);
        }
    }

    void AppendSubSup(Node baseExpression, Node? sub, Node? sup)
    {
        if (sub is not null && sup is not null)
        {
            _sb.Append("<msubsup>");
            Append(baseExpression, _defaultRowMode);
            Append(sub, _defaultRowMode);
            Append(sup, _defaultRowMode);
            _sb.Append("</msubsup>");

        }
        else if (sub is not null)
        {
            _sb.Append("<msub>");
            Append(baseExpression, _defaultRowMode);
            Append(sub, _defaultRowMode);
            _sb.Append("</msub>");
        }
        else if (sup is not null)
        {
            _sb.Append("<msup>");
            Append(baseExpression, _defaultRowMode);
            Append(sup, _defaultRowMode);
            _sb.Append("</msup>");
        }
        else
        {
            Append(baseExpression);
        }
    }

    internal enum RowMode
    {
        Omit,
        Avoid,
        Force,
    }

    internal enum DisplaySymbolType
    {
        Operator,
        LeftParen,
        RightParen,
        LeftRightParen,
        Identifier,
        Accent,
        Font,
        Wrap,
        Cancel,
        Sqrt,
        Over,
        Under,
        Root,
        Color,
        Frac,
        Text,
    }

    internal enum Position
    {
        Over,
        Under,
    }
    
    internal readonly record struct DisplayDetail(string Text, DisplaySymbolType Type, bool IsUnderOver = false, Position Position = Position.Over, string? WrapLParen = null, string? WrapRParen = null);
}