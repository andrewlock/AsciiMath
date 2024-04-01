using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace AsciiMathParser;

internal class SymbolTable
{
    public static bool TryGetEntry(ReadOnlySpan<char> text, [NotNullWhen(true)]out SymbolDetail? entry)
    {
        // Probably would rather a dictionary here, but would need to convert the text to a string so meh
        // can almost certainly optimise this further, but again, meh
        // Operation symbols
        entry = text switch
        {
            "+" => new(Symbol.plus, TokenType.Symbol),
            "-" => new(Symbol.minus, TokenType.Symbol),
            "*" or "cdot" => new(Symbol.cdot, TokenType.Symbol),
            "**" or "ast" => new(Symbol.ast, TokenType.Symbol),
            "***" or "star" => new(Symbol.star, TokenType.Symbol),
            "//" => new(Symbol.slash, TokenType.Symbol),
            "\\\\" or "backslash" => new(Symbol.backslash, TokenType.Symbol),
            "setminus" => new(Symbol.setminus, TokenType.Symbol),
            "xx" or "times" => new(Symbol.times, TokenType.Symbol),
            "|><" or "ltimes" => new(Symbol.ltimes, TokenType.Symbol),
            "><|" or "rtimes" => new(Symbol.rtimes, TokenType.Symbol),
            "|><|" or "bowtie" => new(Symbol.bowtie, TokenType.Symbol),
            "-:" or "div" or "divide" => new(Symbol.div, TokenType.Symbol),
            "@" or "circ" => new(Symbol.circ, TokenType.Symbol),
            "o+" or "oplus" => new(Symbol.oplus, TokenType.Symbol),
            "ox" or "otimes" => new(Symbol.otimes, TokenType.Symbol),
            "o." or "odot" => new(Symbol.odot, TokenType.Symbol),
            "sum" => new(Symbol.sum, TokenType.Symbol),
            "prod" => new(Symbol.prod, TokenType.Symbol),
            "^^" or "wedge" => new(Symbol.wedge, TokenType.Symbol),
            "^^^" or "bigwedge" => new(Symbol.bigwedge, TokenType.Symbol),
            "vv" or "vee" => new(Symbol.vee, TokenType.Symbol),
            "vvv" or "bigvee" => new(Symbol.bigvee, TokenType.Symbol),
            "nn" or "cap" => new(Symbol.cap, TokenType.Symbol),
            "nnn" or "bigcap" => new(Symbol.bigcap, TokenType.Symbol),
            "uu" or "cup" => new(Symbol.cup, TokenType.Symbol),
            "uuu" or "bigcup" => new(Symbol.bigcup, TokenType.Symbol),

// Relation symbols
            "=" => new(Symbol.eq, TokenType.Symbol),
            "!=" or "ne" => new(Symbol.ne, TokenType.Symbol),
            ":=" => new(Symbol.assign, TokenType.Symbol),
            "<" or "lt" => new(Symbol.lt, TokenType.Symbol),
            "mlt" or "ll" => new(Symbol.mlt, TokenType.Symbol),
            ">" or "gt" => new(Symbol.gt, TokenType.Symbol),
            "mgt" or "gg" => new(Symbol.mgt, TokenType.Symbol),
            "<=" or "le" => new(Symbol.le, TokenType.Symbol),
            ">=" or "ge" => new(Symbol.ge, TokenType.Symbol),
            "-<" or "-lt" or "prec" => new(Symbol.prec, TokenType.Symbol),
            ">-" or "succ" => new(Symbol.succ, TokenType.Symbol),
            "-<=" or "preceq" => new(Symbol.preceq, TokenType.Symbol),
            ">-=" or "succeq" => new(Symbol.succeq, TokenType.Symbol),
            "in" => new(Symbol.@in, TokenType.Symbol),
            "!in" or "notin" => new(Symbol.notin, TokenType.Symbol),
            "sub" or "subset" => new(Symbol.subset, TokenType.Symbol),
            "sup" or "supset" => new(Symbol.supset, TokenType.Symbol),
            "sube" or "subseteq" => new(Symbol.subseteq, TokenType.Symbol),
            "supe" or "supseteq" => new(Symbol.supseteq, TokenType.Symbol),
            "-=" or "equiv" => new(Symbol.equiv, TokenType.Symbol),
            "~" or "sim" => new(Symbol.sim, TokenType.Symbol),
            "~=" or "cong" => new(Symbol.cong, TokenType.Symbol),
            "~~" or "approx" => new(Symbol.approx, TokenType.Symbol),
            "prop" or "propto" => new(Symbol.propto, TokenType.Symbol),

// Logical symbols
            "and" => new(Symbol.and, TokenType.Symbol),
            "or" => new(Symbol.or, TokenType.Symbol),
            "not" or "neg" => new(Symbol.not, TokenType.Symbol),
            "=>" or "implies" => new(Symbol.implies, TokenType.Symbol),
            "if" => new(Symbol.@if, TokenType.Symbol),
            "<=>" or "iff" => new(Symbol.iff, TokenType.Symbol),
            "AA" or "forall" => new(Symbol.forall, TokenType.Symbol),
            "EE" or "exists" => new(Symbol.exists, TokenType.Symbol),
            "_|_" or "bot" => new(Symbol.bot, TokenType.Symbol),
            "TT" or "top" => new(Symbol.top, TokenType.Symbol),
            "|--" or "vdash" => new(Symbol.vdash, TokenType.Symbol),
            "|==" or "models" => new(Symbol.models, TokenType.Symbol),

// Grouping brackets
            "(" or "left(" => new(Symbol.lparen, TokenType.LeftParen),
            ")" or "right)" => new(Symbol.rparen, TokenType.RightParen),
            "[" or "left[" => new(Symbol.lbracket, TokenType.LeftParen),
            "]" or "right]" => new(Symbol.rbracket, TokenType.RightParen),
            "{" => new(Symbol.lbrace, TokenType.LeftParen),
            "}" => new(Symbol.rbrace, TokenType.RightParen),
            "|" => new(Symbol.vbar, TokenType.LeftRightParen),
            ":|:" => new(Symbol.vbar, TokenType.Symbol),
            "|:" => new(Symbol.vbar, TokenType.LeftParen),
            ":|" => new(Symbol.vbar, TokenType.RightParen),
//{"dd('||" or "||" => new(Symbol.lrparen),
            "(:" or "<<" or "langle" => new(Symbol.langle, TokenType.LeftParen),
            ":)" or ">>" or "rangle" => new(Symbol.rangle, TokenType.RightParen),
            "{:" => new(null, TokenType.LeftParen),
            ":}" => new(null, TokenType.RightParen),

// Miscellaneous symbols
            "int" => new(Symbol.integral, TokenType.Symbol),
            "dx" => new(Symbol.dx, TokenType.Symbol),
            "dy" => new(Symbol.dy, TokenType.Symbol),
            "dz" => new(Symbol.dz, TokenType.Symbol),
            "dt" => new(Symbol.dt, TokenType.Symbol),
            "oint" => new(Symbol.contourintegral, TokenType.Symbol),
            "del" or "partial" => new(Symbol.partial, TokenType.Symbol),
            "grad" or "nabla" => new(Symbol.nabla, TokenType.Symbol),
            "+-" or "pm" => new(Symbol.pm, TokenType.Symbol),
            "-+" or "mp" => new(Symbol.mp, TokenType.Symbol),
            "O/" or "emptyset" => new(Symbol.emptyset, TokenType.Symbol),
            "oo" or "infty" => new(Symbol.infty, TokenType.Symbol),
            "aleph" => new(Symbol.aleph, TokenType.Symbol),
            "..." or "ldots" => new(Symbol.ellipsis, TokenType.Symbol),
            ":." or "therefore" => new(Symbol.therefore, TokenType.Symbol),
            ":\'" or "because" => new(Symbol.because, TokenType.Symbol),
            "/_" or "angle" => new(Symbol.angle, TokenType.Symbol),
            "/_\\" or "triangle" => new(Symbol.triangle, TokenType.Symbol),
            "\'" or "prime" => new(Symbol.prime, TokenType.Symbol),
            "tilde" => new(Symbol.tilde, TokenType.Unary),
            "\\ " => new(Symbol.nbsp, TokenType.Symbol),
            "frown" => new(Symbol.frown, TokenType.Symbol),
            "quad" => new(Symbol.quad, TokenType.Symbol),
            "qquad" => new(Symbol.qquad, TokenType.Symbol),
            "cdots" => new(Symbol.cdots, TokenType.Symbol),
            "vdots" => new(Symbol.vdots, TokenType.Symbol),
            "ddots" => new(Symbol.ddots, TokenType.Symbol),
            "diamond" => new(Symbol.diamond, TokenType.Symbol),
            "square" => new(Symbol.square, TokenType.Symbol),
            "|__" or "lfloor" => new(Symbol.lfloor, TokenType.Symbol),
            "__|" or "rfloor" => new(Symbol.rfloor, TokenType.Symbol),
            "|~" or "lceiling" => new(Symbol.lceiling, TokenType.Symbol),
            "~|" or "rceiling" => new(Symbol.rceiling, TokenType.Symbol),
            "CC" => new(Symbol.dstruck_captial_c, TokenType.Symbol),
            "NN" => new(Symbol.dstruck_captial_n, TokenType.Symbol),
            "QQ" => new(Symbol.dstruck_captial_q, TokenType.Symbol),
            "RR" => new(Symbol.dstruck_captial_r, TokenType.Symbol),
            "ZZ" => new(Symbol.dstruck_captial_z, TokenType.Symbol),
            "f" => new(Symbol.f, TokenType.Symbol),
            "g" => new(Symbol.g, TokenType.Symbol),


// Standard functions
            "lim" => new(Symbol.lim, TokenType.Symbol),
            "Lim" => new(Symbol.Lim, TokenType.Symbol),
            "min" => new(Symbol.min, TokenType.Symbol),
            "max" => new(Symbol.max, TokenType.Symbol),
            "sin" => new(Symbol.sin, TokenType.Symbol),
            "Sin" => new(Symbol.Sin, TokenType.Symbol),
            "cos" => new(Symbol.cos, TokenType.Symbol),
            "Cos" => new(Symbol.Cos, TokenType.Symbol),
            "tan" => new(Symbol.tan, TokenType.Symbol),
            "Tan" => new(Symbol.Tan, TokenType.Symbol),
            "sinh" => new(Symbol.sinh, TokenType.Symbol),
            "Sinh" => new(Symbol.Sinh, TokenType.Symbol),
            "cosh" => new(Symbol.cosh, TokenType.Symbol),
            "Cosh" => new(Symbol.Cosh, TokenType.Symbol),
            "tanh" => new(Symbol.tanh, TokenType.Symbol),
            "Tanh" => new(Symbol.Tanh, TokenType.Symbol),
            "cot" => new(Symbol.cot, TokenType.Symbol),
            "Cot" => new(Symbol.Cot, TokenType.Symbol),
            "sec" => new(Symbol.sec, TokenType.Symbol),
            "Sec" => new(Symbol.Sec, TokenType.Symbol),
            "csc" => new(Symbol.csc, TokenType.Symbol),
            "Csc" => new(Symbol.Csc, TokenType.Symbol),
            "arcsin" => new(Symbol.arcsin, TokenType.Symbol),
            "arccos" => new(Symbol.arccos, TokenType.Symbol),
            "arctan" => new(Symbol.arctan, TokenType.Symbol),
            "coth" => new(Symbol.coth, TokenType.Symbol),
            "sech" => new(Symbol.sech, TokenType.Symbol),
            "csch" => new(Symbol.csch, TokenType.Symbol),
            "exp" => new(Symbol.exp, TokenType.Symbol),
            "abs" => new(Symbol.abs, TokenType.Unary),
            "Abs" => new(Symbol.abs, TokenType.Unary),
            "norm" => new(Symbol.norm, TokenType.Unary),
            "floor" => new(Symbol.floor, TokenType.Unary),
            "ceil" => new(Symbol.ceil, TokenType.Unary),
            "log" => new(Symbol.log, TokenType.Symbol),
            "Log" => new(Symbol.Log, TokenType.Symbol),
            "ln" => new(Symbol.ln, TokenType.Symbol),
            "Ln" => new(Symbol.Ln, TokenType.Symbol),
            "det" => new(Symbol.det, TokenType.Symbol),
            "dim" => new(Symbol.dim, TokenType.Symbol),
            "ker" => new(Symbol.ker, TokenType.Symbol),
            "mod" => new(Symbol.mod, TokenType.Symbol),
            "gcd" => new(Symbol.gcd, TokenType.Symbol),
            "lcm" => new(Symbol.lcm, TokenType.Symbol),
            "lub" => new(Symbol.lub, TokenType.Symbol),
            "glb" => new(Symbol.glb, TokenType.Symbol),

// Arrows
            "uarr" or "uparrow" => new(Symbol.uparrow, TokenType.Symbol),
            "darr" or "downarrow" => new(Symbol.downarrow, TokenType.Symbol),
            "rarr" or "rightarrow" => new(Symbol.rightarrow, TokenType.Symbol),
            "->" or "to" => new(Symbol.to, TokenType.Symbol),
            ">->" or "rightarrowtail" => new(Symbol.rightarrowtail, TokenType.Symbol),
            "->>" or "twoheadrightarrow" => new(Symbol.twoheadrightarrow, TokenType.Symbol),
            ">->>" or "twoheadrightarrowtail" => new(Symbol.twoheadrightarrowtail, TokenType.Symbol),
            "|->" or "mapsto" => new(Symbol.mapsto, TokenType.Symbol),
            "larr" or "leftarrow" => new(Symbol.leftarrow, TokenType.Symbol),
            "harr" or "leftrightarrow" => new(Symbol.leftrightarrow, TokenType.Symbol),
            "rArr" or "Rightarrow" => new(Symbol.Rightarrow, TokenType.Symbol),
            "lArr" or "Leftarrow" => new(Symbol.Leftarrow, TokenType.Symbol),
            "hArr" or "Leftrightarrow" => new(Symbol.Leftrightarrow, TokenType.Symbol),

// Other
            "sqrt" => new(Symbol.sqrt, TokenType.Unary),
            "root" => new(Symbol.root, TokenType.Binary),
            "frac" => new(Symbol.frac, TokenType.Binary),
            "/" => new(Symbol.frac, TokenType.Infix),
            "stackrel" => new(Symbol.stackrel, TokenType.Binary),
            "overset" => new(Symbol.overset, TokenType.Binary),
            "underset" => new(Symbol.underset, TokenType.Binary),
            "color" => new(Symbol.color, TokenType.Binary, new Converter(convertBinary1: ConvertToColor)),
            "_" => new(Symbol.sub, TokenType.Infix),
            "^" => new(Symbol.sup, TokenType.Infix),
            "hat" => new(Symbol.hat, TokenType.Unary),
            "bar" => new(Symbol.overline, TokenType.Unary),
            "vec" => new(Symbol.vec, TokenType.Unary),
            "dot" => new(Symbol.dot, TokenType.Unary),
            "ddot" => new(Symbol.ddot, TokenType.Unary),
            "overarc" or "overparen" => new(Symbol.overarc, TokenType.Unary),
            "ul" or "underline" => new(Symbol.underline, TokenType.Unary),
            "ubrace" or "underbrace" => new(Symbol.underbrace, TokenType.Unary),
            "obrace" or "overbrace" => new(Symbol.overbrace, TokenType.Unary),
            "cancel" => new(Symbol.cancel, TokenType.Unary),
            "bb" or "mathbf" => new(Symbol.bold, TokenType.Unary),
            "bbb" or "mathbb" => new(Symbol.double_struck, TokenType.Unary),
            "ii" => new(Symbol.italic, TokenType.Unary),
            "bii" => new(Symbol.bold_italic, TokenType.Unary),
            "cc" or "mathcal" => new(Symbol.script, TokenType.Unary),
            "bcc" => new(Symbol.bold_script, TokenType.Unary),
            "tt" or "mathtt" => new(Symbol.monospace, TokenType.Unary),
            "fr" or "mathfrak" => new(Symbol.fraktur, TokenType.Unary),
            "bfr" => new(Symbol.bold_fraktur, TokenType.Unary),
            "sf" or "mathsf" => new(Symbol.sans_serif, TokenType.Unary),
            "bsf" => new(Symbol.bold_sans_serif, TokenType.Unary),
            "sfi" => new(Symbol.sans_serif_italic, TokenType.Unary),
            "sfbi" => new(Symbol.sans_serif_bold_italic, TokenType.Unary),
            "rm" => new(Symbol.roman, TokenType.Unary),

// Greek letters
            "alpha" => new(Symbol.alpha, TokenType.Symbol),
            "Alpha" => new(Symbol.Alpha, TokenType.Symbol),
            "beta" => new(Symbol.beta, TokenType.Symbol),
            "Beta" => new(Symbol.Beta, TokenType.Symbol),
            "gamma" => new(Symbol.gamma, TokenType.Symbol),
            "Gamma" => new(Symbol.Gamma, TokenType.Symbol),
            "delta" => new(Symbol.delta, TokenType.Symbol),
            "Delta" => new(Symbol.Delta, TokenType.Symbol),
            "epsi" or "epsilon" => new(Symbol.epsilon, TokenType.Symbol),
            "Epsilon" => new(Symbol.Epsilon, TokenType.Symbol),
            "varepsilon" => new(Symbol.varepsilon, TokenType.Symbol),
            "zeta" => new(Symbol.zeta, TokenType.Symbol),
            "Zeta" => new(Symbol.Zeta, TokenType.Symbol),
            "eta" => new(Symbol.eta, TokenType.Symbol),
            "Eta" => new(Symbol.Eta, TokenType.Symbol),
            "theta" => new(Symbol.theta, TokenType.Symbol),
            "Theta" => new(Symbol.Theta, TokenType.Symbol),
            "vartheta" => new(Symbol.vartheta, TokenType.Symbol),
            "iota" => new(Symbol.iota, TokenType.Symbol),
            "Iota" => new(Symbol.Iota, TokenType.Symbol),
            "kappa" => new(Symbol.kappa, TokenType.Symbol),
            "Kappa" => new(Symbol.Kappa, TokenType.Symbol),
            "lambda" => new(Symbol.lambda, TokenType.Symbol),
            "Lambda" => new(Symbol.Lambda, TokenType.Symbol),
            "mu" => new(Symbol.mu, TokenType.Symbol),
            "Mu" => new(Symbol.Mu, TokenType.Symbol),
            "nu" => new(Symbol.nu, TokenType.Symbol),
            "Nu" => new(Symbol.Nu, TokenType.Symbol),
            "xi" => new(Symbol.xi, TokenType.Symbol),
            "Xi" => new(Symbol.Xi, TokenType.Symbol),
            "omicron" => new(Symbol.omicron, TokenType.Symbol),
            "Omicron" => new(Symbol.Omicron, TokenType.Symbol),
            "pi" => new(Symbol.pi, TokenType.Symbol),
            "Pi" => new(Symbol.Pi, TokenType.Symbol),
            "rho" => new(Symbol.rho, TokenType.Symbol),
            "Rho" => new(Symbol.Rho, TokenType.Symbol),
            "sigma" => new(Symbol.sigma, TokenType.Symbol),
            "Sigma" => new(Symbol.Sigma, TokenType.Symbol),
            "tau" => new(Symbol.tau, TokenType.Symbol),
            "Tau" => new(Symbol.Tau, TokenType.Symbol),
            "upsilon" => new(Symbol.upsilon, TokenType.Symbol),
            "Upsilon" => new(Symbol.Upsilon, TokenType.Symbol),
            "phi" => new(Symbol.phi, TokenType.Symbol),
            "Phi" => new(Symbol.Phi, TokenType.Symbol),
            "varphi" => new(Symbol.varphi, TokenType.Symbol),
            "chi" => new(Symbol.chi, TokenType.Symbol),
            "Chi" => new(Symbol.Chi, TokenType.Symbol),
            "psi" => new(Symbol.psi, TokenType.Symbol),
            "Psi" => new(Symbol.Psi, TokenType.Symbol),
            "omega" => new(Symbol.omega, TokenType.Symbol),
            "Omega" => new(Symbol.Omega, TokenType.Symbol),
            _ => null,
        };

        return entry is not null;
    }

    public static readonly Lazy<FrozenDictionary<string, SymbolDetail>> AllSymbols = new (() => new Dictionary<string, SymbolDetail>()
    {
        // Operation symbols
        { "+", new(Symbol.plus, TokenType.Symbol) },
        { "-", new(Symbol.minus, TokenType.Symbol) },
        { "*", new(Symbol.cdot, TokenType.Symbol) },
        { "cdot", new(Symbol.cdot, TokenType.Symbol) },
        { "**", new(Symbol.ast, TokenType.Symbol) },
        { "ast", new(Symbol.ast, TokenType.Symbol) },
        { "***", new(Symbol.star, TokenType.Symbol) },
        { "star", new(Symbol.star, TokenType.Symbol) },
        { "//", new(Symbol.slash, TokenType.Symbol) },
        { "\\\\", new(Symbol.backslash, TokenType.Symbol) },
        { "backslash", new(Symbol.backslash, TokenType.Symbol) },
        { "setminus", new(Symbol.setminus, TokenType.Symbol) },
        { "xx", new(Symbol.times, TokenType.Symbol) },
        { "times", new(Symbol.times, TokenType.Symbol) },
        { "|><", new(Symbol.ltimes, TokenType.Symbol) },
        { "ltimes", new(Symbol.ltimes, TokenType.Symbol) },
        { "><|", new(Symbol.rtimes, TokenType.Symbol) },
        { "rtimes", new(Symbol.rtimes, TokenType.Symbol) },
        { "|><|", new(Symbol.bowtie, TokenType.Symbol) },
        { "bowtie", new(Symbol.bowtie, TokenType.Symbol) },
        { "-:", new(Symbol.div, TokenType.Symbol) },
        { "divide", new(Symbol.div, TokenType.Symbol) },
        { "div", new(Symbol.div, TokenType.Symbol) },
        { "@", new(Symbol.circ, TokenType.Symbol) },
        { "circ", new(Symbol.circ, TokenType.Symbol) },
        { "o+", new(Symbol.oplus, TokenType.Symbol) },
        { "oplus", new(Symbol.oplus, TokenType.Symbol) },
        { "ox", new(Symbol.otimes, TokenType.Symbol) },
        { "otimes", new(Symbol.otimes, TokenType.Symbol) },
        { "o.", new(Symbol.odot, TokenType.Symbol) },
        { "odot", new(Symbol.odot, TokenType.Symbol) },
        { "sum", new(Symbol.sum, TokenType.Symbol) },
        { "prod", new(Symbol.prod, TokenType.Symbol) },
        { "^^", new(Symbol.wedge, TokenType.Symbol) },
        { "wedge", new(Symbol.wedge, TokenType.Symbol) },
        { "^^^", new(Symbol.bigwedge, TokenType.Symbol) },
        { "bigwedge", new(Symbol.bigwedge, TokenType.Symbol) },
        { "vv", new(Symbol.vee, TokenType.Symbol) },
        { "vee", new(Symbol.vee, TokenType.Symbol) },
        { "vvv", new(Symbol.bigvee, TokenType.Symbol) },
        { "bigvee", new(Symbol.bigvee, TokenType.Symbol) },
        { "nn", new(Symbol.cap, TokenType.Symbol) },
        { "cap", new(Symbol.cap, TokenType.Symbol) },
        { "nnn", new(Symbol.bigcap, TokenType.Symbol) },
        { "bigcap", new(Symbol.bigcap, TokenType.Symbol) },
        { "uu", new(Symbol.cup, TokenType.Symbol) },
        { "cup", new(Symbol.cup, TokenType.Symbol) },
        { "uuu", new(Symbol.bigcup, TokenType.Symbol) },
        { "bigcup", new(Symbol.bigcup, TokenType.Symbol) },

        // Relation symbols
        { "=", new(Symbol.eq, TokenType.Symbol) },
        { "!=", new(Symbol.ne, TokenType.Symbol) },
        { "ne", new(Symbol.ne, TokenType.Symbol) },
        { ":=", new(Symbol.assign, TokenType.Symbol) },
        { "<", new(Symbol.lt, TokenType.Symbol) },
        { "lt", new(Symbol.lt, TokenType.Symbol) },
        { "mlt", new(Symbol.mlt, TokenType.Symbol) },
        { "ll", new(Symbol.mlt, TokenType.Symbol) },
        { ">", new(Symbol.gt, TokenType.Symbol) },
        { "gt", new(Symbol.gt, TokenType.Symbol) },
        { "mgt", new(Symbol.mgt, TokenType.Symbol) },
        { "gg", new(Symbol.mgt, TokenType.Symbol) },
        { "<=", new(Symbol.le, TokenType.Symbol) },
        { "le", new(Symbol.le, TokenType.Symbol) },
        { ">=", new(Symbol.ge, TokenType.Symbol) },
        { "ge", new(Symbol.ge, TokenType.Symbol) },
        { "-<", new(Symbol.prec, TokenType.Symbol) },
        { "-lt", new(Symbol.prec, TokenType.Symbol) },
        { "prec", new(Symbol.prec, TokenType.Symbol) },
        { ">-", new(Symbol.succ, TokenType.Symbol) },
        { "succ", new(Symbol.succ, TokenType.Symbol) },
        { "-<=", new(Symbol.preceq, TokenType.Symbol) },
        { "preceq", new(Symbol.preceq, TokenType.Symbol) },
        { ">-=", new(Symbol.succeq, TokenType.Symbol) },
        { "succeq", new(Symbol.succeq, TokenType.Symbol) },
        { "in", new(Symbol.@in, TokenType.Symbol) },
        { "!in", new(Symbol.notin, TokenType.Symbol) },
        { "notin", new(Symbol.notin, TokenType.Symbol) },
        { "sub", new(Symbol.subset, TokenType.Symbol) },
        { "subset", new(Symbol.subset, TokenType.Symbol) },
        { "sup", new(Symbol.supset, TokenType.Symbol) },
        { "supset", new(Symbol.supset, TokenType.Symbol) },
        { "sube", new(Symbol.subseteq, TokenType.Symbol) },
        { "subseteq", new(Symbol.subseteq, TokenType.Symbol) },
        { "supe", new(Symbol.supseteq, TokenType.Symbol) },
        { "supseteq", new(Symbol.supseteq, TokenType.Symbol) },
        { "-=", new(Symbol.equiv, TokenType.Symbol) },
        { "equiv", new(Symbol.equiv, TokenType.Symbol) },
        { "~", new(Symbol.sim, TokenType.Symbol) },
        { "sim", new(Symbol.sim, TokenType.Symbol) },
        { "~=", new(Symbol.cong, TokenType.Symbol) },
        { "cong", new(Symbol.cong, TokenType.Symbol) },
        { "~~", new(Symbol.approx, TokenType.Symbol) },
        { "approx", new(Symbol.approx, TokenType.Symbol) },
        { "prop", new(Symbol.propto, TokenType.Symbol) },
        { "propto", new(Symbol.propto, TokenType.Symbol) },

        // Logical symbols
        { "and", new(Symbol.and, TokenType.Symbol) },
        { "or", new(Symbol.or, TokenType.Symbol) },
        { "not", new(Symbol.not, TokenType.Symbol) },
        { "neg", new(Symbol.not, TokenType.Symbol) },
        { "=>", new(Symbol.implies, TokenType.Symbol) },
        { "implies", new(Symbol.implies, TokenType.Symbol) },
        { "if", new(Symbol.@if, TokenType.Symbol) },
        { "<=>", new(Symbol.iff, TokenType.Symbol) },
        { "iff", new(Symbol.iff, TokenType.Symbol) },
        { "AA", new(Symbol.forall, TokenType.Symbol) },
        { "forall", new(Symbol.forall, TokenType.Symbol) },
        { "EE", new(Symbol.exists, TokenType.Symbol) },
        { "exists", new(Symbol.exists, TokenType.Symbol) },
        { "_|_", new(Symbol.bot, TokenType.Symbol) },
        { "bot", new(Symbol.bot, TokenType.Symbol) },
        { "TT", new(Symbol.top, TokenType.Symbol) },
        { "top", new(Symbol.top, TokenType.Symbol) },
        { "|--", new(Symbol.vdash, TokenType.Symbol) },
        { "vdash", new(Symbol.vdash, TokenType.Symbol) },
        { "|==", new(Symbol.models, TokenType.Symbol) },
        { "models", new(Symbol.models, TokenType.Symbol) },

        // Grouping brackets
        { "(", new(Symbol.lparen, TokenType.LeftParen) },
        { "left(", new(Symbol.lparen, TokenType.LeftParen) },
        { ")", new(Symbol.rparen, TokenType.RightParen) },
        { "right)", new(Symbol.rparen, TokenType.RightParen) },
        { "[", new(Symbol.lbracket, TokenType.LeftParen) },
        { "left[", new(Symbol.lbracket, TokenType.LeftParen) },
        { "]", new(Symbol.rbracket, TokenType.RightParen) },
        { "right]", new(Symbol.rbracket, TokenType.RightParen) },
        { "{", new(Symbol.lbrace, TokenType.LeftParen) },
        { "}", new(Symbol.rbrace, TokenType.RightParen) },
        { "|", new(Symbol.vbar, TokenType.LeftRightParen) },
        { ":|:", new(Symbol.vbar, TokenType.Symbol) },
        { "|:", new(Symbol.vbar, TokenType.LeftParen) },
        { ":|", new(Symbol.vbar, TokenType.RightParen) },
        // {"'||", "||", new(Symbol.lrparen)},
        { "(:", new(Symbol.langle, TokenType.LeftParen) },
        { "<<", new(Symbol.langle, TokenType.LeftParen) },
        { "langle", new(Symbol.langle, TokenType.LeftParen) },
        { ":)", new(Symbol.rangle, TokenType.RightParen) },
        { ">>", new(Symbol.rangle, TokenType.RightParen) },
        { "rangle", new(Symbol.rangle, TokenType.RightParen) },
        { "{:", new(null, TokenType.LeftParen) },
        { ":}", new(null, TokenType.RightParen) },

        // Miscellaneous symbols
        { "int", new(Symbol.integral, TokenType.Symbol) },
        { "dx", new(Symbol.dx, TokenType.Symbol) },
        { "dy", new(Symbol.dy, TokenType.Symbol) },
        { "dz", new(Symbol.dz, TokenType.Symbol) },
        { "dt", new(Symbol.dt, TokenType.Symbol) },
        { "oint", new(Symbol.contourintegral, TokenType.Symbol) },
        { "del", new(Symbol.partial, TokenType.Symbol) },
        { "partial", new(Symbol.partial, TokenType.Symbol) },
        { "grad", new(Symbol.nabla, TokenType.Symbol) },
        { "nabla", new(Symbol.nabla, TokenType.Symbol) },
        { "+-", new(Symbol.pm, TokenType.Symbol) },
        { "pm", new(Symbol.pm, TokenType.Symbol) },
        { "-+", new(Symbol.mp, TokenType.Symbol) },
        { "mp", new(Symbol.mp, TokenType.Symbol) },
        { "O/", new(Symbol.emptyset, TokenType.Symbol) },
        { "emptyset", new(Symbol.emptyset, TokenType.Symbol) },
        { "oo", new(Symbol.infty, TokenType.Symbol) },
        { "infty", new(Symbol.infty, TokenType.Symbol) },
        { "aleph", new(Symbol.aleph, TokenType.Symbol) },
        { "...", new(Symbol.ellipsis, TokenType.Symbol) },
        { "ldots", new(Symbol.ellipsis, TokenType.Symbol) },
        { ":.", new(Symbol.therefore, TokenType.Symbol) },
        { "therefore", new(Symbol.therefore, TokenType.Symbol) },
        { ":\'", new(Symbol.because, TokenType.Symbol) },
        { "because", new(Symbol.because, TokenType.Symbol) },
        { "/_", new(Symbol.angle, TokenType.Symbol) },
        { "angle", new(Symbol.angle, TokenType.Symbol) },
        { "/_\\", new(Symbol.triangle, TokenType.Symbol) },
        { "triangle", new(Symbol.triangle, TokenType.Symbol) },
        { "\'", new(Symbol.prime, TokenType.Symbol) },
        { "prime", new(Symbol.prime, TokenType.Symbol) },
        { "tilde", new(Symbol.tilde, TokenType.Unary) },
        { "\\ ", new(Symbol.nbsp, TokenType.Symbol) },
        { "frown", new(Symbol.frown, TokenType.Symbol) },
        { "quad", new(Symbol.quad, TokenType.Symbol) },
        { "qquad", new(Symbol.qquad, TokenType.Symbol) },
        { "cdots", new(Symbol.cdots, TokenType.Symbol) },
        { "vdots", new(Symbol.vdots, TokenType.Symbol) },
        { "ddots", new(Symbol.ddots, TokenType.Symbol) },
        { "diamond", new(Symbol.diamond, TokenType.Symbol) },
        { "square", new(Symbol.square, TokenType.Symbol) },
        { "|__", new(Symbol.lfloor, TokenType.Symbol) },
        { "lfloor", new(Symbol.lfloor, TokenType.Symbol) },
        { "__|", new(Symbol.rfloor, TokenType.Symbol) },
        { "rfloor", new(Symbol.rfloor, TokenType.Symbol) },
        { "|~", new(Symbol.lceiling, TokenType.Symbol) },
        { "lceiling", new(Symbol.lceiling, TokenType.Symbol) },
        { "~|", new(Symbol.rceiling, TokenType.Symbol) },
        { "rceiling", new(Symbol.rceiling, TokenType.Symbol) },
        { "CC", new(Symbol.dstruck_captial_c, TokenType.Symbol) },
        { "NN", new(Symbol.dstruck_captial_n, TokenType.Symbol) },
        { "QQ", new(Symbol.dstruck_captial_q, TokenType.Symbol) },
        { "RR", new(Symbol.dstruck_captial_r, TokenType.Symbol) },
        { "ZZ", new(Symbol.dstruck_captial_z, TokenType.Symbol) },
        { "f", new(Symbol.f, TokenType.Symbol) },
        { "g", new(Symbol.g, TokenType.Symbol) },


        // Standard functions
        { "lim", new(Symbol.lim, TokenType.Symbol) },
        { "Lim", new(Symbol.Lim, TokenType.Symbol) },
        { "min", new(Symbol.min, TokenType.Symbol) },
        { "max", new(Symbol.max, TokenType.Symbol) },
        { "sin", new(Symbol.sin, TokenType.Symbol) },
        { "Sin", new(Symbol.Sin, TokenType.Symbol) },
        { "cos", new(Symbol.cos, TokenType.Symbol) },
        { "Cos", new(Symbol.Cos, TokenType.Symbol) },
        { "tan", new(Symbol.tan, TokenType.Symbol) },
        { "Tan", new(Symbol.Tan, TokenType.Symbol) },
        { "sinh", new(Symbol.sinh, TokenType.Symbol) },
        { "Sinh", new(Symbol.Sinh, TokenType.Symbol) },
        { "cosh", new(Symbol.cosh, TokenType.Symbol) },
        { "Cosh", new(Symbol.Cosh, TokenType.Symbol) },
        { "tanh", new(Symbol.tanh, TokenType.Symbol) },
        { "Tanh", new(Symbol.Tanh, TokenType.Symbol) },
        { "cot", new(Symbol.cot, TokenType.Symbol) },
        { "Cot", new(Symbol.Cot, TokenType.Symbol) },
        { "sec", new(Symbol.sec, TokenType.Symbol) },
        { "Sec", new(Symbol.Sec, TokenType.Symbol) },
        { "csc", new(Symbol.csc, TokenType.Symbol) },
        { "Csc", new(Symbol.Csc, TokenType.Symbol) },
        { "arcsin", new(Symbol.arcsin, TokenType.Symbol) },
        { "arccos", new(Symbol.arccos, TokenType.Symbol) },
        { "arctan", new(Symbol.arctan, TokenType.Symbol) },
        { "coth", new(Symbol.coth, TokenType.Symbol) },
        { "sech", new(Symbol.sech, TokenType.Symbol) },
        { "csch", new(Symbol.csch, TokenType.Symbol) },
        { "exp", new(Symbol.exp, TokenType.Symbol) },
        { "abs", new(Symbol.abs, TokenType.Unary) },
        { "Abs", new(Symbol.abs, TokenType.Unary) },
        { "norm", new(Symbol.norm, TokenType.Unary) },
        { "floor", new(Symbol.floor, TokenType.Unary) },
        { "ceil", new(Symbol.ceil, TokenType.Unary) },
        { "log", new(Symbol.log, TokenType.Symbol) },
        { "Log", new(Symbol.Log, TokenType.Symbol) },
        { "ln", new(Symbol.ln, TokenType.Symbol) },
        { "Ln", new(Symbol.Ln, TokenType.Symbol) },
        { "det", new(Symbol.det, TokenType.Symbol) },
        { "dim", new(Symbol.dim, TokenType.Symbol) },
        { "ker", new(Symbol.ker, TokenType.Symbol) },
        { "mod", new(Symbol.mod, TokenType.Symbol) },
        { "gcd", new(Symbol.gcd, TokenType.Symbol) },
        { "lcm", new(Symbol.lcm, TokenType.Symbol) },
        { "lub", new(Symbol.lub, TokenType.Symbol) },
        { "glb", new(Symbol.glb, TokenType.Symbol) },

        // Arrows
        { "uarr", new(Symbol.uparrow, TokenType.Symbol) },
        { "uparrow", new(Symbol.uparrow, TokenType.Symbol) },
        { "darr", new(Symbol.downarrow, TokenType.Symbol) },
        { "downarrow", new(Symbol.downarrow, TokenType.Symbol) },
        { "rarr", new(Symbol.rightarrow, TokenType.Symbol) },
        { "rightarrow", new(Symbol.rightarrow, TokenType.Symbol) },
        { "->", new(Symbol.to, TokenType.Symbol) },
        { "to", new(Symbol.to, TokenType.Symbol) },
        { ">->", new(Symbol.rightarrowtail, TokenType.Symbol) },
        { "rightarrowtail", new(Symbol.rightarrowtail, TokenType.Symbol) },
        { "->>", new(Symbol.twoheadrightarrow, TokenType.Symbol) },
        { "twoheadrightarrow", new(Symbol.twoheadrightarrow, TokenType.Symbol) },
        { ">->>", new(Symbol.twoheadrightarrowtail, TokenType.Symbol) },
        { "twoheadrightarrowtail", new(Symbol.twoheadrightarrowtail, TokenType.Symbol) },
        { "|->", new(Symbol.mapsto, TokenType.Symbol) },
        { "mapsto", new(Symbol.mapsto, TokenType.Symbol) },
        { "larr", new(Symbol.leftarrow, TokenType.Symbol) },
        { "leftarrow", new(Symbol.leftarrow, TokenType.Symbol) },
        { "harr", new(Symbol.leftrightarrow, TokenType.Symbol) },
        { "leftrightarrow", new(Symbol.leftrightarrow, TokenType.Symbol) },
        { "rArr", new(Symbol.Rightarrow, TokenType.Symbol) },
        { "Rightarrow", new(Symbol.Rightarrow, TokenType.Symbol) },
        { "lArr", new(Symbol.Leftarrow, TokenType.Symbol) },
        { "Leftarrow", new(Symbol.Leftarrow, TokenType.Symbol) },
        { "hArr", new(Symbol.Leftrightarrow, TokenType.Symbol) },
        { "Leftrightarrow", new(Symbol.Leftrightarrow, TokenType.Symbol) },

        // Other
        { "sqrt", new(Symbol.sqrt, TokenType.Unary) },
        { "root", new(Symbol.root, TokenType.Binary) },
        { "frac", new(Symbol.frac, TokenType.Binary) },
        { "/", new(Symbol.frac, TokenType.Infix) },
        { "stackrel", new(Symbol.stackrel, TokenType.Binary) },
        { "overset", new(Symbol.overset, TokenType.Binary) },
        { "underset", new(Symbol.underset, TokenType.Binary) },
        {"color", new(Symbol.color, TokenType.Binary, new Converter(convertBinary1: ConvertToColor)) },
        { "_", new(Symbol.sub, TokenType.Infix) },
        { "^", new(Symbol.sup, TokenType.Infix) },
        { "hat", new(Symbol.hat, TokenType.Unary) },
        { "bar", new(Symbol.overline, TokenType.Unary) },
        { "vec", new(Symbol.vec, TokenType.Unary) },
        { "dot", new(Symbol.dot, TokenType.Unary) },
        { "ddot", new(Symbol.ddot, TokenType.Unary) },
        { "overarc", new(Symbol.overarc, TokenType.Unary) },
        { "overparen", new(Symbol.overarc, TokenType.Unary) },
        { "ul", new(Symbol.underline, TokenType.Unary) },
        { "underline", new(Symbol.underline, TokenType.Unary) },
        { "ubrace", new(Symbol.underbrace, TokenType.Unary) },
        { "underbrace", new(Symbol.underbrace, TokenType.Unary) },
        { "obrace", new(Symbol.overbrace, TokenType.Unary) },
        { "overbrace", new(Symbol.overbrace, TokenType.Unary) },
        { "cancel", new(Symbol.cancel, TokenType.Unary) },
        { "bb", new(Symbol.bold, TokenType.Unary) },
        { "mathbf", new(Symbol.bold, TokenType.Unary) },
        { "bbb", new(Symbol.double_struck, TokenType.Unary) },
        { "mathbb", new(Symbol.double_struck, TokenType.Unary) },
        { "ii", new(Symbol.italic, TokenType.Unary) },
        { "bii", new(Symbol.bold_italic, TokenType.Unary) },
        { "cc", new(Symbol.script, TokenType.Unary) },
        { "mathcal", new(Symbol.script, TokenType.Unary) },
        { "bcc", new(Symbol.bold_script, TokenType.Unary) },
        { "tt", new(Symbol.monospace, TokenType.Unary) },
        { "mathtt", new(Symbol.monospace, TokenType.Unary) },
        { "fr", new(Symbol.fraktur, TokenType.Unary) },
        { "mathfrak", new(Symbol.fraktur, TokenType.Unary) },
        { "bfr", new(Symbol.bold_fraktur, TokenType.Unary) },
        { "sf", new(Symbol.sans_serif, TokenType.Unary) },
        { "mathsf", new(Symbol.sans_serif, TokenType.Unary) },
        { "bsf", new(Symbol.bold_sans_serif, TokenType.Unary) },
        { "sfi", new(Symbol.sans_serif_italic, TokenType.Unary) },
        { "sfbi", new(Symbol.sans_serif_bold_italic, TokenType.Unary) },
        { "rm", new(Symbol.roman, TokenType.Unary) },

        // Greek letters
        { "alpha", new(Symbol.alpha, TokenType.Symbol) },
        { "Alpha", new(Symbol.Alpha, TokenType.Symbol) },
        { "beta", new(Symbol.beta, TokenType.Symbol) },
        { "Beta", new(Symbol.Beta, TokenType.Symbol) },
        { "gamma", new(Symbol.gamma, TokenType.Symbol) },
        { "Gamma", new(Symbol.Gamma, TokenType.Symbol) },
        { "delta", new(Symbol.delta, TokenType.Symbol) },
        { "Delta", new(Symbol.Delta, TokenType.Symbol) },
        { "epsi", new(Symbol.epsilon, TokenType.Symbol) },
        { "epsilon", new(Symbol.epsilon, TokenType.Symbol) },
        { "Epsilon", new(Symbol.Epsilon, TokenType.Symbol) },
        { "varepsilon", new(Symbol.varepsilon, TokenType.Symbol) },
        { "zeta", new(Symbol.zeta, TokenType.Symbol) },
        { "Zeta", new(Symbol.Zeta, TokenType.Symbol) },
        { "eta", new(Symbol.eta, TokenType.Symbol) },
        { "Eta", new(Symbol.Eta, TokenType.Symbol) },
        { "theta", new(Symbol.theta, TokenType.Symbol) },
        { "Theta", new(Symbol.Theta, TokenType.Symbol) },
        { "vartheta", new(Symbol.vartheta, TokenType.Symbol) },
        { "iota", new(Symbol.iota, TokenType.Symbol) },
        { "Iota", new(Symbol.Iota, TokenType.Symbol) },
        { "kappa", new(Symbol.kappa, TokenType.Symbol) },
        { "Kappa", new(Symbol.Kappa, TokenType.Symbol) },
        { "lambda", new(Symbol.lambda, TokenType.Symbol) },
        { "Lambda", new(Symbol.Lambda, TokenType.Symbol) },
        { "mu", new(Symbol.mu, TokenType.Symbol) },
        { "Mu", new(Symbol.Mu, TokenType.Symbol) },
        { "nu", new(Symbol.nu, TokenType.Symbol) },
        { "Nu", new(Symbol.Nu, TokenType.Symbol) },
        { "xi", new(Symbol.xi, TokenType.Symbol) },
        { "Xi", new(Symbol.Xi, TokenType.Symbol) },
        { "omicron", new(Symbol.omicron, TokenType.Symbol) },
        { "Omicron", new(Symbol.Omicron, TokenType.Symbol) },
        { "pi", new(Symbol.pi, TokenType.Symbol) },
        { "Pi", new(Symbol.Pi, TokenType.Symbol) },
        { "rho", new(Symbol.rho, TokenType.Symbol) },
        { "Rho", new(Symbol.Rho, TokenType.Symbol) },
        { "sigma", new(Symbol.sigma, TokenType.Symbol) },
        { "Sigma", new(Symbol.Sigma, TokenType.Symbol) },
        { "tau", new(Symbol.tau, TokenType.Symbol) },
        { "Tau", new(Symbol.Tau, TokenType.Symbol) },
        { "upsilon", new(Symbol.upsilon, TokenType.Symbol) },
        { "Upsilon", new(Symbol.Upsilon, TokenType.Symbol) },
        { "phi", new(Symbol.phi, TokenType.Symbol) },
        { "Phi", new(Symbol.Phi, TokenType.Symbol) },
        { "varphi", new(Symbol.varphi, TokenType.Symbol) },
        { "chi", new(Symbol.chi, TokenType.Symbol) },
        { "Chi", new(Symbol.Chi, TokenType.Symbol) },
        { "psi", new(Symbol.psi, TokenType.Symbol) },
        { "Psi", new(Symbol.Psi, TokenType.Symbol) },
        { "omega", new(Symbol.omega, TokenType.Symbol) },
        { "Omega", new(Symbol.Omega, TokenType.Symbol) },
    }.ToFrozenDictionary());

    public const int MaxKeyLength = 21;


    public static (int r, int g, int b)? TryGetColor(ReadOnlySpan<char> text)
    {
        if (text.Equals("aqua", StringComparison.OrdinalIgnoreCase))
            return (0, 255, 255);
        if (text.Equals("black", StringComparison.OrdinalIgnoreCase))
            return (0, 0, 0);
        if (text.Equals("blue", StringComparison.OrdinalIgnoreCase))
            return (0, 0, 255);
        if (text.Equals("fuchsia", StringComparison.OrdinalIgnoreCase))
            return (255, 0, 255);
        if (text.Equals("gray", StringComparison.OrdinalIgnoreCase))
            return (128, 128, 128);
        if (text.Equals("green", StringComparison.OrdinalIgnoreCase))
            return (0, 128, 0);
        if (text.Equals("lime", StringComparison.OrdinalIgnoreCase))
            return (0, 255, 0);
        if (text.Equals("maroon", StringComparison.OrdinalIgnoreCase))
            return (128, 0, 0);
        if (text.Equals("navy", StringComparison.OrdinalIgnoreCase))
            return (0, 0, 128);
        if (text.Equals("olive", StringComparison.OrdinalIgnoreCase))
            return (128, 128, 0);
        if (text.Equals("purple", StringComparison.OrdinalIgnoreCase))
            return (128, 0, 128);
        if (text.Equals("red", StringComparison.OrdinalIgnoreCase))
            return (255, 0, 0);
        if (text.Equals("silver", StringComparison.OrdinalIgnoreCase))
            return (192, 192, 192);
        if (text.Equals("teal", StringComparison.OrdinalIgnoreCase))
            return (0, 128, 128);
        if (text.Equals("white", StringComparison.OrdinalIgnoreCase))
            return (255, 255, 255);
        if (text.Equals("yellow", StringComparison.OrdinalIgnoreCase))
            return (255, 255, 0);
        return null;
    }

    private static Node ConvertToColor(Node node)
    {
        // set the capacity to be large enough that everything fits
        // in one chunk
        var sb = new StringBuilder(capacity: 8);
        AppendColorText(sb, node);

        int r; 
        int g; 
        int b; 
        if (sb.Length == 4)
        {
            foreach (var chunk in sb.GetChunks())
            {
                if(chunk.Span[0] == '#'
                   && char.IsAsciiHexDigit(chunk.Span[1])
                   && char.IsAsciiHexDigit(chunk.Span[2])
                   && char.IsAsciiHexDigit(chunk.Span[3]))
                {
                    var c = int.Parse(chunk.Span.Slice(1, 1), NumberStyles.HexNumber);
                    r = (c << 4) + c;
                    c = int.Parse(chunk.Span.Slice(2, 1), NumberStyles.HexNumber);
                    g = (c << 4) + c;
                    c = int.Parse(chunk.Span.Slice(3, 1), NumberStyles.HexNumber);
                    b = (c << 4) + c;

                    return new ColorNode(sb.ToString(), r, g, b);
                }
            }
        }
        else if (sb.Length == 7)
        {
            foreach (var chunk in sb.GetChunks())
            {
                if(chunk.Span[0] == '#'
                   && char.IsAsciiHexDigit(chunk.Span[1])
                   && char.IsAsciiHexDigit(chunk.Span[2])
                   && char.IsAsciiHexDigit(chunk.Span[3])
                   && char.IsAsciiHexDigit(chunk.Span[4])
                   && char.IsAsciiHexDigit(chunk.Span[5])
                   && char.IsAsciiHexDigit(chunk.Span[6]))
                {
                    r = int.Parse(chunk.Span.Slice(1, 2), NumberStyles.HexNumber);
                    g = int.Parse(chunk.Span.Slice(3, 2), NumberStyles.HexNumber);
                    b = int.Parse(chunk.Span.Slice(5, 2), NumberStyles.HexNumber);

                    return new ColorNode(sb.ToString(), r, g, b);
                }
            }
        }

        var str = sb.ToString();
        (r, g, b) = TryGetColor(str) ?? (0, 0, 0);

        return new ColorNode(str, r, g, b);

        static void AppendColorText(StringBuilder sb, Node? node)
        {
            switch (node)
            {
                case SequenceNode s:
                    foreach (var n in s)
                    {
                        AppendColorText(sb, n);
                    }

                    break;
                case NumberNode n:
                    sb.Append(n.Value);
                    break;
                case TextNode n:
                    sb.Append(n.Value);
                    break;
                case IdentifierNode n:
                    sb.Append(n.Value);
                    break;
                case SymbolNode n:
                    sb.Append(n.Value);
                    break;
                case GroupNode n:
                    AppendColorText(sb, n.Expression);
                    break;
                case ParenNode n:
                    AppendColorText(sb, n.LParen);
                    AppendColorText(sb, n.Expression);
                    AppendColorText(sb, n.RParen);
                    break;
                case SubSupNode n:
                    AppendColorText(sb, n.BaseExpression);
                    AppendColorText(sb, n.SubExpression);
                    AppendColorText(sb, n.SupExpression);
                    break;
                case UnaryOpNode n:
                    AppendColorText(sb, n.Operator);
                    AppendColorText(sb, n.Operand);
                    break;
                case BinaryOpNode n:
                    AppendColorText(sb, n.Operator);
                    AppendColorText(sb, n.Operand1);
                    AppendColorText(sb, n.Operand2);
                    break;
                case InfixOpNode n:
                    AppendColorText(sb, n.Operand1);
                    AppendColorText(sb, n.Operator);
                    AppendColorText(sb, n.Operand2);
                    break;
            }
        }
    }
}