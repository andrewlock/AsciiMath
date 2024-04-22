namespace AsciiMath.Tests;
using static TestAst;

internal record TestSpec(string asciiMath, Node ast = null, string mathml = null, string mathml_word = null, string html = null, string latex =null)
{
    public static IEnumerable<object[]> AstTests() =>
        Specs.Values.Where(x => x.ast is not null).Select(x => new object[] { x.asciiMath });

    public static IEnumerable<object[]> MathMlTests() =>
        Specs.Values.Where(x => x.mathml is not null).Select(x => new object[] { x.asciiMath });

    public static Dictionary<string, TestSpec> Specs = new TestSpec[]
    {
        new TestSpec("""underset(_)(hat A) = hat A exp j vartheta_0""",
          ast: seq(
              binary(
                  symbol("underset"),
                  group(symbol("_")),
                  group(unary(symbol("hat"), "A"))
              ),
              symbol("="),
              unary(symbol("hat"), "A"),
              symbol("exp"),
              'j',
              sub(symbol("vartheta"), "0")
          ),
          // mathml: """<math><munder><mover accent="true"><mi>A</mi><mo>^</mo></mover><mo>_</mo></munder><mo>=</mo><mover accent="true"><mi>A</mi><mo>^</mo></mover><mi>exp</mi><mi>j</mi><msub><mi>&#x3D1;</mi><mn>0</mn></msub></math>""",
          mathml: """<math><munder><mover accent="true"><mi>A</mi><mo>^</mo></mover><mo>_</mo></munder><mo>=</mo><mover accent="true"><mi>A</mi><mo>^</mo></mover><mo>exp</mo><mi>j</mi><msub><mi>&#x3D1;</mi><mn>0</mn></msub></math>""",
          mathml_word: """<math><munder><mrow><mover accent="true"><mrow><mi>A</mi></mrow><mo>^</mo></mover></mrow><mrow><mo>_</mo></mrow></munder><mo>=</mo><mover accent="true"><mrow><mi>A</mi></mrow><mo>^</mo></mover><mi>exp</mi><mi>j</mi><msub><mrow><mi>&#x3D1;</mi></mrow><mrow><mn>0</mn></mrow></msub></math>""",
          latex: """\\underset{\\text{–}}{\\hat{A}} = \\hat{A} \\exp j \\vartheta_0'"""
      ),
      
      new TestSpec("""x+b/(2a)<+-sqrt((b^2)/(4a^2)-c/a)""",
          ast: seq(
              'x',
              symbol("+"),
              infix('b', symbol("/"), grseq('2', "a")),
              symbol("<"),
              symbol("+-"),
              unary(
                  symbol("sqrt"),
                  grseq(
                      infix(
                          group(sup('b', "2")),
                          symbol("/"),
                          grseq('4', sup('a', "2"))
                      ),
                      symbol("-"),
                      infix('c', symbol("/"), "a")
                  )
              )
          ),
          mathml: """<math><mi>x</mi><mo>+</mo><mfrac><mi>b</mi><mrow><mn>2</mn><mi>a</mi></mrow></mfrac><mo>&lt;</mo><mo>&#xB1;</mo><msqrt><mrow><mfrac><msup><mi>b</mi><mn>2</mn></msup><mrow><mn>4</mn><msup><mi>a</mi><mn>2</mn></msup></mrow></mfrac><mo>&#x2212;</mo><mfrac><mi>c</mi><mi>a</mi></mfrac></mrow></msqrt></math>""",
          latex: """x + \\frac{b}{2 a} < \\pm \\sqrt{\\frac{b^2}{4 a^2} - \\frac{c}{a}}"""
      ),
      
      new TestSpec("""a^2 + b^2 = c^2""",
          ast: seq(
              sup('a', "2"),
              symbol("+"),
              sup('b', "2"),
              symbol("="),
              sup('c', "2")
          ),
          mathml: """<math><msup><mi>a</mi><mn>2</mn></msup><mo>+</mo><msup><mi>b</mi><mn>2</mn></msup><mo>=</mo><msup><mi>c</mi><mn>2</mn></msup></math>""",
          html: """<span class="math-inline"><span class="math-identifier">a</span><span class="math-subsup"><span class="math-smaller"><span class="math-number">2</span></span><span class="math-smaller">&#x200D;</span></span><span class="math-operator">+</span><span class="math-identifier">b</span><span class="math-subsup"><span class="math-smaller"><span class="math-number">2</span></span><span class="math-smaller">&#x200D;</span></span><span class="math-operator">=</span><span class="math-identifier">c</span><span class="math-subsup"><span class="math-smaller"><span class="math-number">2</span></span><span class="math-smaller">&#x200D;</span></span></span>""",
          latex: """a^2 + b^2 = c^2"""
      ),
      
      new TestSpec("""x = (-b+-sqrt(b^2-4ac))/(2a)""",
          ast: seq(
              'x',
              symbol("="),
              infix(
                  grseq(
                      symbol("-"), "b",
                      symbol("+-"),
                      unary(symbol("sqrt"), grseq(sup('b', "2"), symbol("-"), "4", "a", "c"))
                  ),
                  symbol("/"),
                  grseq('2', "a")
              )
          ),
          mathml: """<math><mi>x</mi><mo>=</mo><mfrac><mrow><mo>&#x2212;</mo><mi>b</mi><mo>&#xB1;</mo><msqrt><mrow><msup><mi>b</mi><mn>2</mn></msup><mo>&#x2212;</mo><mn>4</mn><mi>a</mi><mi>c</mi></mrow></msqrt></mrow><mrow><mn>2</mn><mi>a</mi></mrow></mfrac></math>""",
          latex: """x = \\frac{- b \\pm \\sqrt{b^2 - 4 a c}}{2 a}"""
      ),
      
      new TestSpec("""m = (y_2 - y_1)/(x_2 - x_1) = (Deltay)/(Deltax)""",
          ast: seq(
              'm',
              symbol("="),
              infix(grseq(sub('y', "2"), symbol("-"), sub('y', "1")), symbol("/"), grseq(sub('x', "2"), symbol("-"), sub('x', "1"))),
              symbol("="),
              infix(grseq(symbol("Delta"), "y"), symbol("/"), grseq(symbol("Delta"), "x"))
          ),
          mathml: """<math><mi>m</mi><mo>=</mo><mfrac><mrow><msub><mi>y</mi><mn>2</mn></msub><mo>&#x2212;</mo><msub><mi>y</mi><mn>1</mn></msub></mrow><mrow><msub><mi>x</mi><mn>2</mn></msub><mo>&#x2212;</mo><msub><mi>x</mi><mn>1</mn></msub></mrow></mfrac><mo>=</mo><mfrac><mrow><mo>&#x394;</mo><mi>y</mi></mrow><mrow><mo>&#x394;</mo><mi>x</mi></mrow></mfrac></math>""",
          html: """<span class="math-inline"><span class="math-identifier">m</span><span class="math-operator">=</span><span class="math-blank">&#x200D;</span><span class="math-fraction"><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-row"><span class="math-identifier">y</span><span class="math-subsup"><span class="math-smaller">&#x200D;</span><span class="math-smaller"><span class="math-number">2</span></span></span><span class="math-operator">&#x2212;</span><span class="math-identifier">y</span><span class="math-subsup"><span class="math-smaller">&#x200D;</span><span class="math-smaller"><span class="math-number">1</span></span></span></span></span></span></span></span><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-row"><span class="math-identifier">x</span><span class="math-subsup"><span class="math-smaller">&#x200D;</span><span class="math-smaller"><span class="math-number">2</span></span></span><span class="math-operator">&#x2212;</span><span class="math-identifier">x</span><span class="math-subsup"><span class="math-smaller">&#x200D;</span><span class="math-smaller"><span class="math-number">1</span></span></span></span></span></span></span></span></span><span class="math-operator">=</span><span class="math-blank">&#x200D;</span><span class="math-fraction"><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-row"><span class="math-operator">&#x394;</span><span class="math-identifier">y</span></span></span></span></span></span><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-row"><span class="math-operator">&#x394;</span><span class="math-identifier">x</span></span></span></span></span></span></span></span>""",
          latex: """m = \\frac{y_2 - y_1}{x_2 - x_1} = \\frac{\\Delta y}{\\Delta x}"""
      ),
      
      new TestSpec("""f'(x) = lim_(Deltax->0)(f(x+Deltax)-f(x))/(Deltax)""",
          ast: seq(
              symbol("f"),
              symbol("\'"),
              paren("x"),
              symbol("="),
              sub(
                  symbol("lim"),
                  grseq(symbol("Delta"), "x", symbol("->"), "0")
              ),
              infix(
                  grseq(symbol("f"), paren(seq('x', symbol("+"), symbol("Delta"), "x")), symbol("-"), symbol("f"), paren("x")),
                  symbol("/"),
                  grseq(symbol("Delta"), "x")
              )
          ),
          mathml: """<math><mi>f</mi><mo>&#x2032;</mo><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow><mo>=</mo><munder><mo>lim</mo><mrow><mo>&#x394;</mo><mi>x</mi><mo>&#x2192;</mo><mn>0</mn></mrow></munder><mfrac><mrow><mi>f</mi><mrow><mo>(</mo><mrow><mi>x</mi><mo>+</mo><mo>&#x394;</mo><mi>x</mi></mrow><mo>)</mo></mrow><mo>&#x2212;</mo><mi>f</mi><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow></mrow><mrow><mo>&#x394;</mo><mi>x</mi></mrow></mfrac></math>""",
          html: """<span class="math-inline"><span class="math-identifier">f</span><span class="math-operator">&#x2032;</span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">x</span><span class="math-brace">)</span></span><span class="math-operator">=</span><span class="math-blank">&#x200D;</span><span class="math-underover"><span class="math-smaller"><span class="math-blank">&#x200D;</span></span><span class="math-operator">lim</span><span class="math-smaller"><span class="math-row"><span class="math-operator">&#x394;</span><span class="math-identifier">x</span><span class="math-operator">&#x2192;</span><span class="math-number">0</span></span></span></span><span class="math-blank">&#x200D;</span><span class="math-fraction"><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-row"><span class="math-identifier">f</span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">x</span><span class="math-operator">+</span><span class="math-operator">&#x394;</span><span class="math-identifier">x</span><span class="math-brace">)</span></span><span class="math-operator">&#x2212;</span><span class="math-identifier">f</span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">x</span><span class="math-brace">)</span></span></span></span></span></span></span><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-row"><span class="math-operator">&#x394;</span><span class="math-identifier">x</span></span></span></span></span></span></span></span>""",
          latex: """f \' ( x ) = \\lim_{\\Delta x \\rightarrow 0} \\frac{f \\left ( x + \\Delta x \\right ) - f ( x )}{\\Delta x}"""
      ),
      
      new TestSpec("""d/dx [x^n] = nx^(n - 1)""",
          ast: seq(
              infix('d', symbol("/"), symbol("dx")),
              paren(symbol("["), sup('x', "n"), symbol("]")),
              symbol("="),
              'n',
              sup('x', grseq('n', symbol("-"), "1"))
          ),
          mathml: """<math><mfrac><mi>d</mi><mi>dx</mi></mfrac><mrow><mo>[</mo><msup><mi>x</mi><mi>n</mi></msup><mo>]</mo></mrow><mo>=</mo><mi>n</mi><msup><mi>x</mi><mrow><mi>n</mi><mo>&#x2212;</mo><mn>1</mn></mrow></msup></math>""",
          html: """<span class="math-inline"><span class="math-blank">&#x200D;</span><span class="math-fraction"><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-identifier">d</span></span></span></span></span><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-identifier">dx</span></span></span></span></span></span><span class="math-row"><span class="math-brace">[</span><span class="math-identifier">x</span><span class="math-subsup"><span class="math-smaller"><span class="math-identifier">n</span></span><span class="math-smaller">&#x200D;</span></span><span class="math-brace">]</span></span><span class="math-operator">=</span><span class="math-identifier">n</span><span class="math-identifier">x</span><span class="math-subsup"><span class="math-smaller"><span class="math-row"><span class="math-identifier">n</span><span class="math-operator">&#x2212;</span><span class="math-number">1</span></span></span><span class="math-smaller">&#x200D;</span></span></span>""",
          latex: """\\frac{d}{dx} [ x^n ] = n x^{n - 1}"""
      ),
      
      new TestSpec("""int_a^b f(x) dx = [F(x)]_a^b = F(b) - F(a)""",
          ast: seq(
              subsup(symbol("int"), "a", "b"),
              symbol("f"),
              paren("x"),
              symbol("dx"),
              symbol("="),
              subsup(paren(symbol("["), seq("F", paren("x")), symbol("]")), "a", "b"),
              symbol("="),
              'F', paren('b'),
              symbol("-"),
              'F', paren('a')
          ),
          mathml: """<math><msubsup><mo>&#x222B;</mo><mi>a</mi><mi>b</mi></msubsup><mi>f</mi><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow><mi>dx</mi><mo>=</mo><msubsup><mrow><mo>[</mo><mrow><mi>F</mi><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow></mrow><mo>]</mo></mrow><mi>a</mi><mi>b</mi></msubsup><mo>=</mo><mi>F</mi><mrow><mo>(</mo><mi>b</mi><mo>)</mo></mrow><mo>&#x2212;</mo><mi>F</mi><mrow><mo>(</mo><mi>a</mi><mo>)</mo></mrow></math>""",
          html: """<span class="math-inline"><span class="math-operator">&#x222B;</span><span class="math-subsup"><span class="math-smaller"><span class="math-identifier">b</span></span><span class="math-smaller"><span class="math-identifier">a</span></span></span><span class="math-identifier">f</span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">x</span><span class="math-brace">)</span></span><span class="math-identifier">dx</span><span class="math-operator">=</span><span class="math-row"><span class="math-brace">[</span><span class="math-identifier">F</span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">x</span><span class="math-brace">)</span></span><span class="math-brace">]</span></span><span class="math-subsup"><span class="math-smaller"><span class="math-identifier">b</span></span><span class="math-smaller"><span class="math-identifier">a</span></span></span><span class="math-operator">=</span><span class="math-identifier">F</span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">b</span><span class="math-brace">)</span></span><span class="math-operator">&#x2212;</span><span class="math-identifier">F</span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">a</span><span class="math-brace">)</span></span></span>""",
          latex: """\\int_a^b f ( x ) dx = {\\left [ F ( x ) \\right ]}_a^b = F ( b ) - F ( a )"""
      ),
      
      new TestSpec("""int_a^b f(x) dx = f(c)(b - a)""",
          ast: seq(
              subsup(symbol("int"), 'a', "b"),
              symbol("f"),
              paren('x'),
              symbol("dx"),
              symbol("="),
              symbol("f"),
              paren('c'),
              paren(seq('b', symbol("-"), "a"))
          ),
          mathml: """<math><msubsup><mo>&#x222B;</mo><mi>a</mi><mi>b</mi></msubsup><mi>f</mi><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow><mi>dx</mi><mo>=</mo><mi>f</mi><mrow><mo>(</mo><mi>c</mi><mo>)</mo></mrow><mrow><mo>(</mo><mrow><mi>b</mi><mo>&#x2212;</mo><mi>a</mi></mrow><mo>)</mo></mrow></math>""",
          html: """<span class="math-inline"><span class="math-operator">&#x222B;</span><span class="math-subsup"><span class="math-smaller"><span class="math-identifier">b</span></span><span class="math-smaller"><span class="math-identifier">a</span></span></span><span class="math-identifier">f</span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">x</span><span class="math-brace">)</span></span><span class="math-identifier">dx</span><span class="math-operator">=</span><span class="math-identifier">f</span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">c</span><span class="math-brace">)</span></span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">b</span><span class="math-operator">&#x2212;</span><span class="math-identifier">a</span><span class="math-brace">)</span></span></span>""",
          latex: """\\int_a^b f ( x ) dx = f ( c ) ( b - a )"""
      ),
      
      new TestSpec("""ax^2 + bx + c = 0""",
          ast: seq(
              'a',
              sup('x', "2"),
              symbol("+"),
              'b',
              'x',
              symbol("+"),
              'c',
              symbol("="),
              '0'
          ),
          mathml: """<math><mi>a</mi><msup><mi>x</mi><mn>2</mn></msup><mo>+</mo><mi>b</mi><mi>x</mi><mo>+</mo><mi>c</mi><mo>=</mo><mn>0</mn></math>""",
          html: """<span class="math-inline"><span class="math-identifier">a</span><span class="math-identifier">x</span><span class="math-subsup"><span class="math-smaller"><span class="math-number">2</span></span><span class="math-smaller">&#x200D;</span></span><span class="math-operator">+</span><span class="math-identifier">b</span><span class="math-identifier">x</span><span class="math-operator">+</span><span class="math-identifier">c</span><span class="math-operator">=</span><span class="math-number">0</span></span>""",
          latex: """a x^2 + b x + c = 0"""
      ),
      
      new TestSpec("\"average value\"=1/(b-a) int_a^b f(x) dx",
          ast: seq(
              "average value",
              symbol("="),
              infix('1', symbol("/"), grseq('b', symbol("-"), "a")),
              subsup(symbol("int"), 'a', "b"),
              symbol("f"),
              paren('x'),
              symbol("dx")
          ),
          mathml: """<math><mtext>average value</mtext><mo>=</mo><mfrac><mn>1</mn><mrow><mi>b</mi><mo>&#x2212;</mo><mi>a</mi></mrow></mfrac><msubsup><mo>&#x222B;</mo><mi>a</mi><mi>b</mi></msubsup><mi>f</mi><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow><mi>dx</mi></math>""",
          html: """<span class="math-inline"><span class="math-text">average value</span><span class="math-operator">=</span><span class="math-blank">&#x200D;</span><span class="math-fraction"><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-number">1</span></span></span></span></span><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-row"><span class="math-identifier">b</span><span class="math-operator">&#x2212;</span><span class="math-identifier">a</span></span></span></span></span></span></span><span class="math-operator">&#x222B;</span><span class="math-subsup"><span class="math-smaller"><span class="math-identifier">b</span></span><span class="math-smaller"><span class="math-identifier">a</span></span></span><span class="math-identifier">f</span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">x</span><span class="math-brace">)</span></span><span class="math-identifier">dx</span></span>""",
          latex: """\\text{average value} = \\frac{1}{b - a} \\int_a^b f ( x ) dx"""
      ),
      
      new TestSpec("""d/dx[int_a^x f(t) dt] = f(x)""",
          ast: seq(
              infix('d', symbol("/"), symbol("dx")),
              paren(
                  symbol("["),
                  seq(subsup(symbol("int"), 'a', "x"), symbol("f"), paren('t'), symbol("dt")),
                  symbol("]")
              ),
              symbol("="),
              symbol("f"),
              paren('x')
          ),
          mathml: """<math><mfrac><mi>d</mi><mi>dx</mi></mfrac><mrow><mo>[</mo><mrow><msubsup><mo>&#x222B;</mo><mi>a</mi><mi>x</mi></msubsup><mi>f</mi><mrow><mo>(</mo><mi>t</mi><mo>)</mo></mrow><mi>dt</mi></mrow><mo>]</mo></mrow><mo>=</mo><mi>f</mi><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow></math>""",
          html: """<span class="math-inline"><span class="math-blank">&#x200D;</span><span class="math-fraction"><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-identifier">d</span></span></span></span></span><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-identifier">dx</span></span></span></span></span></span><span class="math-row"><span class="math-brace">[</span><span class="math-operator">&#x222B;</span><span class="math-subsup"><span class="math-smaller"><span class="math-identifier">x</span></span><span class="math-smaller"><span class="math-identifier">a</span></span></span><span class="math-identifier">f</span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">t</span><span class="math-brace">)</span></span><span class="math-identifier">dt</span><span class="math-brace">]</span></span><span class="math-operator">=</span><span class="math-identifier">f</span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">x</span><span class="math-brace">)</span></span></span>""",
          latex: """\\frac{d}{dx} \\left [ \\int_a^x f ( t ) dt \\right ] = f ( x )"""
      ),
      
      new TestSpec("""hat(ab) bar(xy) ul(A) vec(v)""",
          ast: seq(
              unary(symbol("hat"), grseq('a', "b")),
              unary(symbol("bar"), grseq('x', "y")),
              unary(symbol("ul"), group('A')),
              unary(symbol("vec"), group('v'))
          ),
          mathml: """<math><mover accent="true"><mrow><mi>a</mi><mi>b</mi></mrow><mo>^</mo></mover><mover accent="true"><mrow><mi>x</mi><mi>y</mi></mrow><mo>&#xAF;</mo></mover><munder accentunder="true"><mi>A</mi><mo>_</mo></munder><mover accent="true"><mi>v</mi><mo>&#x2192;</mo></mover></math>""",
          html: """<span class="math-inline"><span class="math-blank">&#x200D;</span><span class="math-underover"><span class="math-smaller"><span class="math-operator">^</span></span><span class="math-row"><span class="math-identifier">a</span><span class="math-identifier">b</span></span><span class="math-smaller"><span class="math-blank">&#x200D;</span></span></span><span class="math-blank">&#x200D;</span><span class="math-underover"><span class="math-smaller"><span class="math-operator">&#xAF;</span></span><span class="math-row"><span class="math-identifier">x</span><span class="math-identifier">y</span></span><span class="math-smaller"><span class="math-blank">&#x200D;</span></span></span><span class="math-blank">&#x200D;</span><span class="math-underover"><span class="math-smaller"><span class="math-blank">&#x200D;</span></span><span class="math-identifier">A</span><span class="math-smaller"><span class="math-operator">_</span></span></span><span class="math-blank">&#x200D;</span><span class="math-underover"><span class="math-smaller"><span class="math-operator">&#x2192;</span></span><span class="math-identifier">v</span><span class="math-smaller"><span class="math-blank">&#x200D;</span></span></span></span>""",
          latex: """\\hat{a b} \\overline{x y} \\underline{A} \\vec{v}"""
      ),
      
      new TestSpec("""z_12^34""",
          ast: subsup('z', "12", "34"),
          mathml: """<math><msubsup><mi>z</mi><mn>12</mn><mn>34</mn></msubsup></math>""",
          html: """<span class="math-inline"><span class="math-identifier">z</span><span class="math-subsup"><span class="math-smaller"><span class="math-number">34</span></span><span class="math-smaller"><span class="math-number">12</span></span></span></span>""",
          latex: """z_{12}^{34}"""
      ),
      
      new TestSpec("""lim_(x->c)(f(x)-f(c))/(x-c)""",
          ast: seq(
              sub(symbol("lim"), grseq('x', symbol("->"), "c")),
              infix(
                  grseq(symbol("f"), paren('x'), symbol("-"), symbol("f"), paren('c')),
                  symbol("/"),
                  grseq('x', symbol("-"), "c")
              )
          ),
          mathml: """<math><munder><mo>lim</mo><mrow><mi>x</mi><mo>&#x2192;</mo><mi>c</mi></mrow></munder><mfrac><mrow><mi>f</mi><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow><mo>&#x2212;</mo><mi>f</mi><mrow><mo>(</mo><mi>c</mi><mo>)</mo></mrow></mrow><mrow><mi>x</mi><mo>&#x2212;</mo><mi>c</mi></mrow></mfrac></math>""",
          html: """<span class="math-inline"><span class="math-blank">&#x200D;</span><span class="math-underover"><span class="math-smaller"><span class="math-blank">&#x200D;</span></span><span class="math-operator">lim</span><span class="math-smaller"><span class="math-row"><span class="math-identifier">x</span><span class="math-operator">&#x2192;</span><span class="math-identifier">c</span></span></span></span><span class="math-blank">&#x200D;</span><span class="math-fraction"><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-row"><span class="math-identifier">f</span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">x</span><span class="math-brace">)</span></span><span class="math-operator">&#x2212;</span><span class="math-identifier">f</span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">c</span><span class="math-brace">)</span></span></span></span></span></span></span><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-row"><span class="math-identifier">x</span><span class="math-operator">&#x2212;</span><span class="math-identifier">c</span></span></span></span></span></span></span></span>""",
          latex: """\\lim_{x \\rightarrow c} \\frac{f ( x ) - f ( c )}{x - c}"""
      ),
      
      new TestSpec("""int_0^(pi/2) g(x) dx""",
          ast: seq(
              subsup(symbol("int"), '0', group(infix(symbol("pi"), symbol("/"), "2"))),
              symbol("g"), paren('x'),
              symbol("dx")
          ),
          mathml: """<math><msubsup><mo>&#x222B;</mo><mn>0</mn><mfrac><mi>&#x3C0;</mi><mn>2</mn></mfrac></msubsup><mi>g</mi><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow><mi>dx</mi></math>""",
          html: """<span class="math-inline"><span class="math-operator">&#x222B;</span><span class="math-subsup"><span class="math-smaller"><span class="math-blank">&#x200D;</span><span class="math-fraction"><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-identifier">&#x3C0;</span></span></span></span></span><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-number">2</span></span></span></span></span></span></span><span class="math-smaller"><span class="math-number">0</span></span></span><span class="math-identifier">g</span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">x</span><span class="math-brace">)</span></span><span class="math-identifier">dx</span></span>""",
          latex: """\\int_0^{\\frac{\\pi}{2}} g ( x ) dx"""
      ),
      
      new TestSpec("""sum_(n=0)^oo a_n""",
          ast: seq(
              subsup(symbol("sum"), grseq('n', symbol("="), "0"), symbol("oo")),
              sub('a', "n")
          ),
          mathml: """<math><munderover><mo>&#x2211;</mo><mrow><mi>n</mi><mo>=</mo><mn>0</mn></mrow><mo>&#x221E;</mo></munderover><msub><mi>a</mi><mi>n</mi></msub></math>""",
          html: """<span class="math-inline"><span class="math-blank">&#x200D;</span><span class="math-underover"><span class="math-smaller"><span class="math-operator">&#x221E;</span></span><span class="math-operator">&#x2211;</span><span class="math-smaller"><span class="math-row"><span class="math-identifier">n</span><span class="math-operator">=</span><span class="math-number">0</span></span></span></span><span class="math-identifier">a</span><span class="math-subsup"><span class="math-smaller">&#x200D;</span><span class="math-smaller"><span class="math-identifier">n</span></span></span></span>""",
          latex: """\\sum_{n = 0}^\\infty a_n"""
      ),
      
      new TestSpec("""((1),(42))""",
          ast: matrix([["1"], ["42"]]),
          mathml: """<math><mrow><mo>(</mo><mtable><mtr><mtd><mn>1</mn></mtd></mtr><mtr><mtd><mn>42</mn></mtd></mtr></mtable><mo>)</mo></mrow></math>""",
          html: """<span class="math-inline"><span class="math-row"><span class="math-brace" style="font-size: 200%;">(</span><span class="math-matrix" style="grid-template-columns:repeat(1,1fr);grid-template-rows:repeat(2,1fr);"><span class="math-row"><span class="math-number">1</span></span><span class="math-row"><span class="math-number">42</span></span></span><span class="math-brace" style="font-size: 200%;">)</span></span></span>""",
          latex: """\\left ( \\begin{matrix} 1 \\\\ 42 \\end{matrix} \\right )"""
      ),
      
      new TestSpec("""((1,42))""",
        ast: matrix([["1", "42"]]),
        mathml: """<math><mrow><mo>(</mo><mtable><mtr><mtd><mn>1</mn></mtd><mtd><mn>42</mn></mtd></mtr></mtable><mo>)</mo></mrow></math>""",
        html: """<span class="math-inline"><span class="math-row"><span class="math-brace" style="font-size: 100%;">(</span><span class="math-matrix" style="grid-template-columns:repeat(2,1fr);grid-template-rows:repeat(1,1fr);"><span class="math-row"><span class="math-number">1</span></span><span class="math-row"><span class="math-number">42</span></span></span><span class="math-brace" style="font-size: 100%;">)</span></span></span>""",
        latex: """\\left ( \\begin{matrix} 1 & 42 \\end{matrix} \\right )"""
      ),
      
      new TestSpec("""((1,2,3),(4,5,6),(7,8,9))""",
          ast: matrix([["1", "2", "3"], ["4", "5", "6",], ["7", "8", "9"]]),
          mathml: """<math><mrow><mo>(</mo><mtable><mtr><mtd><mn>1</mn></mtd><mtd><mn>2</mn></mtd><mtd><mn>3</mn></mtd></mtr><mtr><mtd><mn>4</mn></mtd><mtd><mn>5</mn></mtd><mtd><mn>6</mn></mtd></mtr><mtr><mtd><mn>7</mn></mtd><mtd><mn>8</mn></mtd><mtd><mn>9</mn></mtd></mtr></mtable><mo>)</mo></mrow></math>""",
          html: """<span class="math-inline"><span class="math-row"><span class="math-brace" style="font-size: 300%;">(</span><span class="math-matrix" style="grid-template-columns:repeat(3,1fr);grid-template-rows:repeat(3,1fr);"><span class="math-row"><span class="math-number">1</span></span><span class="math-row"><span class="math-number">2</span></span><span class="math-row"><span class="math-number">3</span></span><span class="math-row"><span class="math-number">4</span></span><span class="math-row"><span class="math-number">5</span></span><span class="math-row"><span class="math-number">6</span></span><span class="math-row"><span class="math-number">7</span></span><span class="math-row"><span class="math-number">8</span></span><span class="math-row"><span class="math-number">9</span></span></span><span class="math-brace" style="font-size: 300%;">)</span></span></span>""",
          latex: """\\left ( \\begin{matrix} 1 & 2 & 3 \\\\ 4 & 5 & 6 \\\\ 7 & 8 & 9 \\end{matrix} \\right )"""
      ),

      new TestSpec("""|(a,b),(c,d)|=ad-bc""",
          ast: seq(
              matrix(symbol("|"), [["a", "b"], ["c", "d"]], symbol("|")),
              symbol("="),
              'a', 'd',
              symbol("-"),
              'b', 'c'
          ),
          mathml: """<math><mrow><mo>|</mo><mtable><mtr><mtd><mi>a</mi></mtd><mtd><mi>b</mi></mtd></mtr><mtr><mtd><mi>c</mi></mtd><mtd><mi>d</mi></mtd></mtr></mtable><mo>|</mo></mrow><mo>=</mo><mi>a</mi><mi>d</mi><mo>&#x2212;</mo><mi>b</mi><mi>c</mi></math>""",
          mathml_word: """<math><mfenced open="|" close="|"><mtable><mtr><mtd><mi>a</mi></mtd><mtd><mi>b</mi></mtd></mtr><mtr><mtd><mi>c</mi></mtd><mtd><mi>d</mi></mtd></mtr></mtable></mfenced><mo>=</mo><mi>a</mi><mi>d</mi><mo>&#x2212;</mo><mi>b</mi><mi>c</mi></math>""",
          html: """<span class="math-inline"><span class="math-row"><span class="math-brace" style="font-size: 200%;">|</span><span class="math-matrix" style="grid-template-columns:repeat(2,1fr);grid-template-rows:repeat(2,1fr);"><span class="math-row"><span class="math-identifier">a</span></span><span class="math-row"><span class="math-identifier">b</span></span><span class="math-row"><span class="math-identifier">c</span></span><span class="math-row"><span class="math-identifier">d</span></span></span><span class="math-brace" style="font-size: 200%;">|</span></span><span class="math-operator">=</span><span class="math-identifier">a</span><span class="math-identifier">d</span><span class="math-operator">&#x2212;</span><span class="math-identifier">b</span><span class="math-identifier">c</span></span>""",
          latex: """\\left | \\begin{matrix} a & b \\\\ c & d \\end{matrix} \\right | = a d - b c"""
      ),
      
      new TestSpec("""((a_(11), cdots , a_(1n)),(vdots, ddots, vdots),(a_(m1), cdots , a_(mn)))""",
          ast: matrix([
                             [sub('a', group("11")), symbol("cdots"), sub('a', grseq('1', "n"))],
                             [symbol("vdots"), symbol("ddots"), symbol("vdots")],
                             [sub('a', grseq('m', "1")), symbol("cdots"), sub('a', grseq('m', "n"))]
                         ]),
          mathml: """<math><mrow><mo>(</mo><mtable><mtr><mtd><msub><mi>a</mi><mn>11</mn></msub></mtd><mtd><mo>&#x22EF;</mo></mtd><mtd><msub><mi>a</mi><mrow><mn>1</mn><mi>n</mi></mrow></msub></mtd></mtr><mtr><mtd><mo>&#x22EE;</mo></mtd><mtd><mo>&#x22F1;</mo></mtd><mtd><mo>&#x22EE;</mo></mtd></mtr><mtr><mtd><msub><mi>a</mi><mrow><mi>m</mi><mn>1</mn></mrow></msub></mtd><mtd><mo>&#x22EF;</mo></mtd><mtd><msub><mi>a</mi><mrow><mi>m</mi><mi>n</mi></mrow></msub></mtd></mtr></mtable><mo>)</mo></mrow></math>""",
          html: """<span class="math-inline"><span class="math-row"><span class="math-brace" style="font-size: 300%;">(</span><span class="math-matrix" style="grid-template-columns:repeat(3,1fr);grid-template-rows:repeat(3,1fr);"><span class="math-row"><span class="math-identifier">a</span><span class="math-subsup"><span class="math-smaller">&#x200D;</span><span class="math-smaller"><span class="math-number">11</span></span></span></span><span class="math-row"><span class="math-operator">&#x22EF;</span></span><span class="math-row"><span class="math-identifier">a</span><span class="math-subsup"><span class="math-smaller">&#x200D;</span><span class="math-smaller"><span class="math-row"><span class="math-number">1</span><span class="math-identifier">n</span></span></span></span></span><span class="math-row"><span class="math-operator">&#x22EE;</span></span><span class="math-row"><span class="math-operator">&#x22F1;</span></span><span class="math-row"><span class="math-operator">&#x22EE;</span></span><span class="math-row"><span class="math-identifier">a</span><span class="math-subsup"><span class="math-smaller">&#x200D;</span><span class="math-smaller"><span class="math-row"><span class="math-identifier">m</span><span class="math-number">1</span></span></span></span></span><span class="math-row"><span class="math-operator">&#x22EF;</span></span><span class="math-row"><span class="math-identifier">a</span><span class="math-subsup"><span class="math-smaller">&#x200D;</span><span class="math-smaller"><span class="math-row"><span class="math-identifier">m</span><span class="math-identifier">n</span></span></span></span></span></span><span class="math-brace" style="font-size: 300%;">)</span></span></span>""",
          latex: """\\left ( \\begin{matrix} a_{11} & \\cdots & a_{1 n} \\\\ \\vdots & \\ddots & \\vdots \\\\ a_{m 1} & \\cdots & a_{m n} \\end{matrix} \\right )"""
      ),
      
      new TestSpec("""sum_(k=1)^n k = 1+2+ cdots +n=(n(n+1))/2""",
          ast: seq(
              subsup(symbol("sum"), grseq('k', symbol("="), "1"), "n"),
              'k',
              symbol("="),
              '1', symbol("+"), '2', symbol("+"), symbol("cdots"), symbol("+"), 'n',
              symbol("="),
              infix(
                  grseq('n', paren(seq('n', symbol("+"), "1"))),
                  symbol("/"),
                  '2'
              )
          ),
          mathml: """<math><munderover><mo>&#x2211;</mo><mrow><mi>k</mi><mo>=</mo><mn>1</mn></mrow><mi>n</mi></munderover><mi>k</mi><mo>=</mo><mn>1</mn><mo>+</mo><mn>2</mn><mo>+</mo><mo>&#x22EF;</mo><mo>+</mo><mi>n</mi><mo>=</mo><mfrac><mrow><mi>n</mi><mrow><mo>(</mo><mrow><mi>n</mi><mo>+</mo><mn>1</mn></mrow><mo>)</mo></mrow></mrow><mn>2</mn></mfrac></math>""",
          html: """<span class="math-inline"><span class="math-blank">&#x200D;</span><span class="math-underover"><span class="math-smaller"><span class="math-identifier">n</span></span><span class="math-operator">&#x2211;</span><span class="math-smaller"><span class="math-row"><span class="math-identifier">k</span><span class="math-operator">=</span><span class="math-number">1</span></span></span></span><span class="math-identifier">k</span><span class="math-operator">=</span><span class="math-number">1</span><span class="math-operator">+</span><span class="math-number">2</span><span class="math-operator">+</span><span class="math-operator">&#x22EF;</span><span class="math-operator">+</span><span class="math-identifier">n</span><span class="math-operator">=</span><span class="math-blank">&#x200D;</span><span class="math-fraction"><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-row"><span class="math-identifier">n</span><span class="math-row"><span class="math-brace">(</span><span class="math-identifier">n</span><span class="math-operator">+</span><span class="math-number">1</span><span class="math-brace">)</span></span></span></span></span></span></span><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-number">2</span></span></span></span></span></span></span>""",
          latex: """\\sum_{k = 1}^n k = 1 + 2 + \\cdots + n = \\frac{n ( n + 1 )}{2}"""
      ),
      
      new TestSpec("\"Скорость\"=(\"Расстояние\")/(\"Время\")",
          ast: seq(
              "Скорость",
              symbol("="),
              infix(group("Расстояние"), symbol("/"), group("Время"))
          ),
          mathml: """<math><mtext>&#x421;&#x43A;&#x43E;&#x440;&#x43E;&#x441;&#x442;&#x44C;</mtext><mo>=</mo><mfrac><mtext>&#x420;&#x430;&#x441;&#x441;&#x442;&#x43E;&#x44F;&#x43D;&#x438;&#x435;</mtext><mtext>&#x412;&#x440;&#x435;&#x43C;&#x44F;</mtext></mfrac></math>""",
          html: """<span class="math-inline"><span class="math-text">&#x421;&#x43A;&#x43E;&#x440;&#x43E;&#x441;&#x442;&#x44C;</span><span class="math-operator">=</span><span class="math-blank">&#x200D;</span><span class="math-fraction"><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-text">&#x420;&#x430;&#x441;&#x441;&#x442;&#x43E;&#x44F;&#x43D;&#x438;&#x435;</span></span></span></span></span><span class="math-fraction_row"><span class="math-fraction_cell"><span class="math-smaller"><span class="math-row"><span class="math-text">&#x412;&#x440;&#x435;&#x43C;&#x44F;</span></span></span></span></span></span></span>""",
          latex: """\\text{Скорость} = \\frac{\\text{Расстояние}}{\\text{Время}}"""
      ),
      
      new TestSpec("""bb (a + b) + cc c = fr (d^n)""",
          ast: seq(
              unary(symbol("bb"), grseq('a', symbol("+"), "b")),
              symbol("+"),
              unary(symbol("cc"), "c"),
              symbol("="),
              unary(symbol("fr"), group(sup('d', "n")))
          ),
          mathml: """<math><mstyle mathvariant="bold"><mrow><mi>a</mi><mo>+</mo><mi>b</mi></mrow></mstyle><mo>+</mo><mstyle mathvariant="script"><mi>c</mi></mstyle><mo>=</mo><mstyle mathvariant="fraktur"><msup><mi>d</mi><mi>n</mi></msup></mstyle></math>""",
          latex: """\\mathbf{a + b} + \\mathscr{c} = \\mathfrak{d^n}"""
      ),
      
      new TestSpec("""max()""",
          ast: seq(symbol("max"), paren(null)),
          mathml: """<math><mo>max</mo><mrow><mo>(</mo><mo>)</mo></mrow></math>""",
          html: """<span class="math-inline"><span class="math-operator">max</span><span class="math-row"><span class="math-brace">(</span><span class="math-brace">)</span></span></span>""",
          latex: """\\max (  )"""
      ),
      
      new TestSpec("""text("foo")""",
          ast: text("\"foo\""),
          mathml: """<math><mtext>"foo"</mtext></math>""",
          html: """<span class="math-inline"><span class="math-text">"foo"</span></span>""",
          latex: """\\text{"foo"}"""
      ),
      
      new TestSpec("""ubrace(1 + 2) obrace(3 + 4""",
          ast: seq(
              unary(symbol("ubrace"), grseq('1', symbol("+"), "2")),
              unary(symbol("obrace"), group(symbol("("), seq('3', symbol("+"), "4"), null))
          ),
          mathml: """<math><munder accentunder="true"><mrow><mn>1</mn><mo>+</mo><mn>2</mn></mrow><mo>&#x23DF;</mo></munder><mover accent="true"><mrow><mn>3</mn><mo>+</mo><mn>4</mn></mrow><mo>&#x23DE;</mo></mover></math>""",
          latex: """\\underbrace{1 + 2} \\overbrace{3 + 4}"""
      ),
      
      new TestSpec("""s'_i = {(- 1, if s_i > s_(i + 1)),( + 1, if s_i <= s_(i + 1)):}""",
          ast: seq(
              's',
              sub(symbol("\'"), "i"),
              symbol("="),
              matrix(
                  symbol("{"),
                  [
                      [seq(symbol("-"), "1"), seq(symbol("if"), sub('s', "i"), symbol(">"), sub('s', grseq('i', symbol("+"), "1")))],
                      [seq(symbol("+"), "1"), seq(symbol("if"), sub('s', "i"), symbol("<="), sub('s', grseq('i', symbol("+"), "1")))]
                  ],
                  symbol(":}")
              )
          ),
          mathml: """<math><mi>s</mi><msub><mo>&#x2032;</mo><mi>i</mi></msub><mo>=</mo><mrow><mo>{</mo><mtable><mtr><mtd><mrow><mo>&#x2212;</mo><mn>1</mn></mrow></mtd><mtd><mrow><mo>if</mo><msub><mi>s</mi><mi>i</mi></msub><mo>&gt;</mo><msub><mi>s</mi><mrow><mi>i</mi><mo>+</mo><mn>1</mn></mrow></msub></mrow></mtd></mtr><mtr><mtd><mrow><mo>+</mo><mn>1</mn></mrow></mtd><mtd><mrow><mo>if</mo><msub><mi>s</mi><mi>i</mi></msub><mo>&#x2264;</mo><msub><mi>s</mi><mrow><mi>i</mi><mo>+</mo><mn>1</mn></mrow></msub></mrow></mtd></mtr></mtable></mrow></math>""",
          latex: """s \'_i = \\left \\{ \\begin{matrix} - 1 & \\operatorname{if} s_i > s_{i + 1} \\\\ + 1 & \\operatorname{if} s_i \\le s_{i + 1} \\end{matrix} \\right ."""
      ),
      
      new TestSpec("""s'_i = {(, if s_i > s_(i + 1)),( + 1,):}""",
          ast: seq(
              's',
              sub(symbol("\'"), "i"),
              symbol("="),
              matrix(
                  symbol("{"),
                  [
                      [new EmptyNode(), new object[]{symbol("if"), sub('s', "i"), symbol(">"), sub('s', grseq('i', symbol("+"), "1"))}],
                      [new object[]{symbol("+"), "1"}, new EmptyNode()]
                  ],
                  symbol(":}")
              )
          ),
          mathml: """<math><mi>s</mi><msub><mo>&#x2032;</mo><mi>i</mi></msub><mo>=</mo><mrow><mo>{</mo><mtable><mtr><mtd></mtd><mtd><mrow><mo>if</mo><msub><mi>s</mi><mi>i</mi></msub><mo>&gt;</mo><msub><mi>s</mi><mrow><mi>i</mi><mo>+</mo><mn>1</mn></mrow></msub></mrow></mtd></mtr><mtr><mtd><mrow><mo>+</mo><mn>1</mn></mrow></mtd><mtd></mtd></mtr></mtable></mrow></math>""",
          latex: """s \'_i = \\left \\{ \\begin{matrix}  & \\operatorname{if} s_i > s_{i + 1} \\\\ + 1 &  \\end{matrix} \\right ."""
      ),
      
      new TestSpec("""{:(a,b),(c,d):}""",
          ast: matrix(
              symbol("{:"),
              [["a", "b"], ["c", "d"]],
              symbol(":}")
          ),
          mathml: """<math><mtable><mtr><mtd><mi>a</mi></mtd><mtd><mi>b</mi></mtd></mtr><mtr><mtd><mi>c</mi></mtd><mtd><mi>d</mi></mtd></mtr></mtable></math>""",
          latex: """\\begin{matrix} a & b \\\\ c & d \\end{matrix}"""
      ),
      
      new TestSpec("""overset (a + b) (c + d)""",
          ast: binary(
              symbol("overset"),
              grseq('a', symbol("+"), "b"),
              grseq('c', symbol("+"), "d")
          ),
          mathml: """<math><mover><mrow><mi>c</mi><mo>+</mo><mi>d</mi></mrow><mrow><mi>a</mi><mo>+</mo><mi>b</mi></mrow></mover></math>""",
          html: """<span class="math-inline"><span class="math-blank">&#x200D;</span><span class="math-underover"><span class="math-smaller"><span class="math-row"><span class="math-identifier">a</span><span class="math-operator">+</span><span class="math-identifier">b</span></span></span><span class="math-row"><span class="math-identifier">c</span><span class="math-operator">+</span><span class="math-identifier">d</span></span><span class="math-smaller"><span class="math-blank">&#x200D;</span></span></span></span>""",
          latex: """\\overset{a + b}{c + d}"""
      ),
      
      new TestSpec("""underset a b""",
          ast: binary(
              symbol("underset"),
              'a',
              'b'
          ),
          mathml: """<math><munder><mi>b</mi><mi>a</mi></munder></math>""",
          html: """<span class="math-inline"><span class="math-blank">&#x200D;</span><span class="math-underover"><span class="math-smaller"><span class="math-blank">&#x200D;</span></span><span class="math-identifier">b</span><span class="math-smaller"><span class="math-identifier">a</span></span></span></span>""",
          latex: """\\underset{a}{b}"""
      ),
      
      new TestSpec("""sin a_c^b""",
          ast: seq(
              symbol("sin"),
              subsup('a', 'c', "b")
          ),
          // mathml: """<math><mi>sin</mi><msubsup><mi>a</mi><mi>c</mi><mi>b</mi></msubsup></math>""",
          mathml: """<math><mo>sin</mo><msubsup><mi>a</mi><mi>c</mi><mi>b</mi></msubsup></math>""",
          html: """<span class="math-inline"><span class="math-identifier">sin</span><span class="math-identifier">a</span><span class="math-subsup"><span class="math-smaller"><span class="math-identifier">b</span></span><span class="math-smaller"><span class="math-identifier">c</span></span></span></span>""",
          latex: """\\sin a_c^b"""
      ),
      
      new TestSpec("""max a_c^b""",
          ast: seq(
              symbol("max"),
              subsup('a', 'c', "b")
          ),
          mathml: """<math><mo>max</mo><msubsup><mi>a</mi><mi>c</mi><mi>b</mi></msubsup></math>""",
          html: """<span class="math-inline"><span class="math-operator">max</span><span class="math-identifier">a</span><span class="math-subsup"><span class="math-smaller"><span class="math-identifier">b</span></span><span class="math-smaller"><span class="math-identifier">c</span></span></span></span>""",
          latex: """\\max a_c^b"""
      ),
      
      new TestSpec("""norm a_c^b""",
          ast: subsup(unary(symbol("norm"), "a"), 'c', "b"),
          mathml: """<math><msubsup><mrow><mo>&#x2225;</mo><mi>a</mi><mo>&#x2225;</mo></mrow><mi>c</mi><mi>b</mi></msubsup></math>""",
          html: """<span class="math-inline"><span class="math-row"><span class="math-brace">&#x2225;</span><span class="math-identifier">a</span><span class="math-brace">&#x2225;</span></span><span class="math-subsup"><span class="math-smaller"><span class="math-identifier">b</span></span><span class="math-smaller"><span class="math-identifier">c</span></span></span></span>""",
          latex: """{\\lVert a \\rVert}_c^b"""
      ),
      
      new TestSpec("""overarc a_b^c""",
          ast: subsup(unary(symbol("overarc"), "a"), 'b', "c"),
          mathml: """<math><msubsup><mover accent="true"><mi>a</mi><mo>&#x23DC;</mo></mover><mi>b</mi><mi>c</mi></msubsup></math>""",
          latex: """{\\overset{\\frown}{a}}_b^c'"""
      ),
      
      new TestSpec("""frown a_b^c""",
          ast: seq(symbol("frown"), subsup('a', 'b', "c")),
          mathml: """<math><mo>&#x2322;</mo><msubsup><mi>a</mi><mi>b</mi><mi>c</mi></msubsup></math>""",
          latex: """\\frown a_b^c"""
      ),
      
      new TestSpec("""sin(a_c^b)""",
          ast: seq(symbol("sin"), paren(subsup('a', 'c', "b"))),
          // mathml: """<math><mi>sin</mi><mrow><mo>(</mo><msubsup><mi>a</mi><mi>c</mi><mi>b</mi></msubsup><mo>)</mo></mrow></math>""",
          mathml: """<math><mo>sin</mo><mrow><mo>(</mo><msubsup><mi>a</mi><mi>c</mi><mi>b</mi></msubsup><mo>)</mo></mrow></math>""",
          latex: """\\sin ( a_c^b )"""
      ),
      
      new TestSpec("""text(a)a2)""",
          ast: seq(text("a"), identifier("a"), number("2"), symbol(")")),
          mathml: """<math><mtext>a</mtext><mi>a</mi><mn>2</mn><mo>)</mo></math>""",
          html: """<span class="math-inline"><span class="math-text">a</span><span class="math-identifier">a</span><span class="math-number">2</span><span class="math-operator">)</span></span>""",
          latex: """\\text{a} a 2 )"""
      ),
      
      new TestSpec("""cancel(a_b^c) cancel a_b^c""",
          ast: seq(
              unary(symbol("cancel"), group(subsup('a', "b", "c"))),
              subsup(unary(symbol("cancel"), "a"), "b", "c")
          ),
          mathml: """<math><menclose notation="updiagonalstrike"><msubsup><mi>a</mi><mi>b</mi><mi>c</mi></msubsup></menclose><msubsup><menclose notation="updiagonalstrike"><mi>a</mi></menclose><mi>b</mi><mi>c</mi></msubsup></math>""",
          latex: """\\cancel{a_b^c} {\\cancel{a}}_b^c"""
      ),
      
      new TestSpec("""color(red)(x) color(#123)(y) color(#1234ab)(z) colortext(blue)(a_b^c)""",
          ast: seq(
              binary(symbol("color"), color(255, 0, 0, "red"), group('x')),
              binary(symbol("color"), color(17, 34, 51, "#123"), group("y")),
              binary(symbol("color"), color(18, 52, 171, "#1234ab"), group("z")),
              binary(symbol("color"), color(0, 0, 255, "blue"), group(subsup('a', "b", "c")))
          ),
          mathml: """<math><mstyle mathcolor="#ff0000"><mi>x</mi></mstyle><mstyle mathcolor="#112233"><mi>y</mi></mstyle><mstyle mathcolor="#1234ab"><mi>z</mi></mstyle><mstyle mathcolor="#0000ff"><msubsup><mi>a</mi><mi>b</mi><mi>c</mi></msubsup></mstyle></math>""",
          latex: """{\\color{red} x} {\\color[RGB]{17,34,51} y} {\\color[RGB]{18,52,171} z} {\\color{blue} a_b^c}"""
      ),

      new TestSpec("""{ x\ : \ x in A ^^ x in B }""",
          ast: paren(
              symbol("{"),
              seq('x', symbol(@"\ "), ":", symbol(@"\ "), "x", symbol("in"), "A", symbol("^^"), "x", symbol("in"), "B"),
              symbol("}")
          ),
          mathml: """<math><mrow><mo>{</mo><mrow><mi>x</mi><mo>&#xA0;</mo><mo>:</mo><mo>&#xA0;</mo><mi>x</mi><mo>&#x2208;</mo><mi>A</mi><mo>&#x2227;</mo><mi>x</mi><mo>&#x2208;</mo><mi>B</mi></mrow><mo>}</mo></mrow></math>""",
          latex: """\\left \\{ x \\; : \\; x \\in A \\wedge x \\in B \\right \\}"""
      ),

      new TestSpec("""ii""",
          ast: unary(symbol("ii"), identifier("")),
          mathml: """<math><mstyle mathvariant="italic"><mi></mi></mstyle></math>"""
      ),
      
      new TestSpec("""rm(ms)""",
          ast: unary(symbol("rm"), grseq(identifier("m"), identifier("s"))),
          mathml: """<math><mstyle mathvariant="normal"><mrow><mi>m</mi><mi>s</mi></mrow></mstyle></math>"""
      ),
      
      new TestSpec("""hat""",
          ast: unary(symbol("hat"), identifier("")),
          mathml: """<math><mover accent="true"><mi></mi><mo>^</mo></mover></math>"""
      ),
      
      new TestSpec("""40% * 3!""",
        ast: seq("40", "%", symbol("*"), "3", "!"),
        mathml: """<math><mn>40</mn><mo>%</mo><mo>&#x22C5;</mo><mn>3</mn><mo>!</mo></math>""",
        html: """<span class="math-inline"><span class="math-number">40</span><span class="math-operator">%</span><span class="math-operator">&#x22C5;</span><span class="math-number">3</span><span class="math-operator">!</span></span>""",
        latex: """40 \% \cdot 3 !"""
      ),
      
      new TestSpec("""R(alpha_(K+1)|x)""",
        ast: seq('R', paren(symbol("("), seq(sub("alpha", grseq('K', symbol("+"), "1")), symbol("|"), "x"), symbol(")"))),
        mathml: """<math><mi>R</mi><mrow><mo>(</mo><mrow><msub><mi>&#x3B1;</mi><mrow><mi>K</mi><mo>+</mo><mn>1</mn></mrow></msub><mo>|</mo><mi>x</mi></mrow><mo>)</mo></mrow></math>""",
        latex: """R \\left ( \\alpha_{K + 1} | x \\right )"""
      ),
      
      new TestSpec("""|(a),(b)|""",
        ast: matrix(symbol("|"), [["a"], ["b"]], symbol("|"))
      ),
      
      new TestSpec("""|a+b|""",
        ast: paren(symbol("|"), seq('a', "+", "b"), symbol("|"))
      ),
      
      new TestSpec("""|a+b|/c""",
        ast: infix(paren(symbol("|"), seq('a', "+", "b"), symbol("|")), symbol("/"), "c")
      ),
      
      new TestSpec("""[[a,b,|,c],[d,e,|,f]]""",
        ast: matrix(symbol("["), [['a', 'b', symbol("|"), 'c'], ['d', 'e', symbol("|"), 'f']], symbol("]"))
      ),
      
      new TestSpec("""~a mlt b mgt -+c""",
        ast: seq(symbol("~"), "a", symbol("mlt"), "b", symbol("mgt"), symbol("-+"), "c"),
        latex: """\\sim a \\ll b \\gg \\mp c""",
        mathml: """<math><mo>&#x223C;</mo><mi>a</mi><mo>&#x226A;</mo><mi>b</mi><mo>&#x226B;</mo><mo>&#x2213;</mo><mi>c</mi></math>"""
        ),
      
      new TestSpec("""a+b+...+c""",
        ast: seq('a', symbol("+"), "b", symbol("+"), symbol("..."), symbol("+"), "c"),
        latex: """a + b + \ldots + c""",
        mathml: """<math><mi>a</mi><mo>+</mo><mi>b</mi><mo>+</mo><mo>&#x2026;</mo><mo>+</mo><mi>c</mi></math>"""
        ),
      
      new TestSpec("""frac{a}{b}""",
        ast: binary(symbol("frac"), group(symbol("{"), "a", symbol("}")), group(symbol("{"), "b", symbol("}"))),
        latex: """\frac{a}{b}""",
        mathml: """<math><mfrac><mi>a</mi><mi>b</mi></mfrac></math>"""
      ),
      
      new TestSpec("""ubrace(((1, 0),(0, 1)))_("Adjustment to texture space")""",
        ast: subsup(unary(symbol("ubrace"), group(matrix([["1", "0"], ["0", "1"]]))), group("Adjustment to texture space"), new EmptyNode()),
        latex: """\underbrace{\left ( \begin{matrix} 1 & 0 \\\\ 0 & 1 \end{matrix} \right )}_{\text{Adjustment to texture space}}""",
        mathml: """<math><munder><munder accentunder="true"><mrow><mo>(</mo><mtable><mtr><mtd><mn>1</mn></mtd><mtd><mn>0</mn></mtd></mtr><mtr><mtd><mn>0</mn></mtd><mtd><mn>1</mn></mtd></mtr></mtable><mo>)</mo></mrow><mo>&#x23DF;</mo></munder><mtext>Adjustment to texture space</mtext></munder></math>"""
      ),
      
      new TestSpec("""tilde x""",
        ast: unary(symbol("tilde"), "x"),
        latex: """\\tilde{x}'"""
        ),
    }.ToDictionary(x => x.asciiMath, x => x);
}

internal record AsciiMathTestSpec(string input, string output)
{
    public static IEnumerable<object[]> AllTests() => UnitTests.Select(x => new[] { x.input, x.output });
    
    // based on https://github.com/asciimath/asciimathml/blob/master/test/unittests.js
    private static readonly AsciiMathTestSpec[] UnitTests  = [
        //single symbol output
        new(input: "!=", output:"<mo>≠</mo>"),
        new(input: "!in", output:"<mo>∉</mo>"),
        new(input: "'", output:"<mo>′</mo>"),
        new(input: "(", output:"<mrow><mo>(</mo><mo></mo></mrow>"),
        new(input: "(:", output:"<mrow><mo>〈</mo><mo></mo></mrow>"),
        new(input: ")", output:"<mo>)</mo>"),
        new(input: "*", output:"<mo>⋅</mo>"),
        new(input: "**", output:"<mo>∗</mo>"),
        new(input: "***", output:"<mo>⋆</mo>"),
        new(input: "+-", output:"<mo>±</mo>"),
        new(input: "-", output:"<mo>-</mo>"),
        new(input: "-:", output:"<mo>÷</mo>"),
        new(input: "-<", output:"<mo>≺</mo>"),
        new(input: "-<=", output:"<mo>⪯</mo>"),
        new(input: "-=", output:"<mo>≡</mo>"),
        new(input: "->", output:"<mo>→</mo>"),
        new(input: "->>", output:"<mo>↠</mo>"),
        new(input: "-lt", output:"<mo>≺</mo>"),
        new(input: "...", output:"<mo>...</mo>"),
        new(input: "/", output:"<mo>/</mo>"),
        new(input: "//", output:"<mo>/</mo>"),
        new(input: "/_", output:"<mo>∠</mo>"),
        new(input: "/_\\", output:"<mo>△</mo>"),
        new(input: ":)", output:"<mo>〉</mo>"),
        new(input: ":.", output:"<mo>∴</mo>"),
        new(input: ":=", output:"<mo>:=</mo>"),
        new(input: ":}", output:"<mo>:}</mo>"),
        new(input: "<<", output:"<mrow><mo>〈</mo><mo></mo></mrow>"),
        new(input: "<=", output:"<mo>≤</mo>"),
        new(input: "<=>", output:"<mo>⇔</mo>"),
        new(input: "=>", output:"<mo>⇒</mo>"),
        new(input: ">-", output:"<mo>≻</mo>"),
        new(input: ">-=", output:"<mo>⪰</mo>"),
        new(input: ">->", output:"<mo>↣</mo>"),
        new(input: ">->>", output:"<mo>⤖</mo>"),
        new(input: "><|", output:"<mo>⋊</mo>"),
        new(input: ">=", output:"<mo>≥</mo>"),
        new(input: ">>", output:"<mo>〉</mo>"),
        new(input: "@", output:"<mo>∘</mo>"),
        new(input: "AA", output:"<mo>∀</mo>"),
        new(input: "CC", output:"<mo>ℂ</mo>"),
        new(input: "Delta", output:"<mo>Δ</mo>"),
        new(input: "EE", output:"<mo>∃</mo>"),
        new(input: "Gamma", output:"<mo>Γ</mo>"),
        new(input: "Lambda", output:"<mo>Λ</mo>"),
        new(input: "Lamda", output:"<mo>Λ</mo>"),
        new(input: "Leftarrow", output:"<mo>⇐</mo>"),
        new(input: "Leftrightarrow", output:"<mo>⇔</mo>"),
        new(input: "Lim", output:"<mo>Lim</mo>"),
        new(input: "NN", output:"<mo>ℕ</mo>"),
        new(input: "O/", output:"<mo>∅</mo>"),
        new(input: "Omega", output:"<mo>Ω</mo>"),
        new(input: "Phi", output:"<mo>Φ</mo>"),
        new(input: "Pi", output:"<mo>Π</mo>"),
        new(input: "Psi", output:"<mi>Ψ</mi>"),
        new(input: "QQ", output:"<mo>ℚ</mo>"),
        new(input: "RR", output:"<mo>ℝ</mo>"),
        new(input: "Rightarrow", output:"<mo>⇒</mo>"),
        new(input: "Sigma", output:"<mo>Σ</mo>"),
        new(input: "TT", output:"<mo>⊤</mo>"),
        new(input: "Theta", output:"<mo>Θ</mo>"),
        new(input: "Xi", output:"<mo>Ξ</mo>"),
        new(input: "ZZ", output:"<mo>ℤ</mo>"),
        new(input: "[", output:"<mrow><mo>[</mo><mo></mo></mrow>"),
        new(input: "\\ ", output:"<mo>&nbsp;</mo>"),
        new(input: "\\\\", output:"<mo>\\</mo>"),
        new(input: "]", output:"<mo>]</mo>"),
        new(input: "^", output:"<mo>^</mo>"),
        new(input: "^^", output:"<mo>∧</mo>"),
        new(input: "^^^", output:"<mo>⋀</mo>"),
        new(input: "_", output:"<mo>_</mo>"),
        new(input: "__|", output:"<mo>⌋</mo>"),
        new(input: "_|_", output:"<mo>⊥</mo>"),
        new(input: "abs", output:"<mrow><mo>|</mo><mo></mo><mo>|</mo></mrow>"),
        new(input: "aleph", output:"<mo>ℵ</mo>"),
        new(input: "alpha", output:"<mi>α</mi>"),
        new(input: "and", output:"<mrow><mspace width=\"1ex\"></mspace><mtext>and</mtext><mspace width=\"1ex\"></mspace></mrow>"),
        new(input: "angle", output:"<mo>∠</mo>"),
        new(input: "approx", output:"<mo>≈</mo>"),
        new(input: "arccos", output:"<mrow><mo>arccos</mo><mo></mo></mrow>"),
        new(input: "arcsin", output:"<mrow><mo>arcsin</mo><mo></mo></mrow>"),
        new(input: "arctan", output:"<mrow><mo>arctan</mo><mo></mo></mrow>"),
        new(input: "ast", output:"<mo>∗</mo>"),
        new(input: "backslash", output:"<mo>\\</mo>"),
        new(input: "bar", output:"<mover><mo></mo><mo>¯</mo></mover>"),
        new(input: "bb", output:"<mstyle mathvariant=\"bold\"><mo></mo></mstyle>"),
        new(input: "bbb", output:"<mstyle mathvariant=\"double-struck\"><mo></mo></mstyle>"),
        new(input: "beta", output:"<mi>β</mi>"),
        new(input: "bigcap", output:"<mo>⋂</mo>"),
        new(input: "bigcup", output:"<mo>⋃</mo>"),
        new(input: "bigvee", output:"<mo>⋁</mo>"),
        new(input: "bigwedge", output:"<mo>⋀</mo>"),
        new(input: "bot", output:"<mo>⊥</mo>"),
        new(input: "bowtie", output:"<mo>⋈</mo>"),
        new(input: "cap", output:"<mo>∩</mo>"),
        new(input: "cdot", output:"<mo>⋅</mo>"),
        new(input: "cdots", output:"<mo>⋯</mo>"),
        new(input: "ceil", output:"<mrow><mo>⌈</mo><mo></mo><mo>⌉</mo></mrow>"),
        new(input: "chi", output:"<mi>χ</mi>"),
        new(input: "circ", output:"<mo>∘</mo>"),
        new(input: "cong", output:"<mo>≅</mo>"),
        new(input: "cos", output:"<mrow><mo>cos</mo><mo></mo></mrow>"),
        new(input: "cosh", output:"<mrow><mo>cosh</mo><mo></mo></mrow>"),
        new(input: "cot", output:"<mrow><mo>cot</mo><mo></mo></mrow>"),
        new(input: "coth", output:"<mrow><mo>coth</mo><mo></mo></mrow>"),
        new(input: "csc", output:"<mrow><mo>csc</mo><mo></mo></mrow>"),
        new(input: "csch", output:"<mrow><mo>csch</mo><mo></mo></mrow>"),
        new(input: "cup", output:"<mo>∪</mo>"),
        new(input: "darr", output:"<mo>↓</mo>"),
        new(input: "ddot", output:"<mover><mo></mo><mo>..</mo></mover>"),
        new(input: "ddots", output:"<mo>⋱</mo>"),
        new(input: "del", output:"<mo>∂</mo>"),
        new(input: "delta", output:"<mi>δ</mi>"),
        new(input: "det", output:"<mrow><mo>det</mo><mo></mo></mrow>"),
        new(input: "diamond", output:"<mo>⋄</mo>"),
        new(input: "dim", output:"<mo>dim</mo>"),
        new(input: "div", output:"<mo>÷</mo>"),
        new(input: "divide", output:"<mo>÷</mo>"),
        new(input: "dot", output:"<mover><mo></mo><mo>.</mo></mover>"),
        new(input: "downarrow", output:"<mo>↓</mo>"),
        new(input: "dt", output:"<mrow><mi>d</mi><mi>t</mi></mrow>"),
        new(input: "dx", output:"<mrow><mi>d</mi><mi>x</mi></mrow>"),
        new(input: "dy", output:"<mrow><mi>d</mi><mi>y</mi></mrow>"),
        new(input: "dz", output:"<mrow><mi>d</mi><mi>z</mi></mrow>"),
        new(input: "emptyset", output:"<mo>∅</mo>"),
        new(input: "epsi", output:"<mi>ε</mi>"),
        new(input: "epsilon", output:"<mi>ε</mi>"),
        new(input: "equiv", output:"<mo>≡</mo>"),
        new(input: "eta", output:"<mi>η</mi>"),
        new(input: "exists", output:"<mo>∃</mo>"),
        new(input: "exp", output:"<mrow><mo>exp</mo><mo></mo></mrow>"),
        new(input: "f", output:"<mi>f</mi>"),
        new(input: "floor", output:"<mrow><mo>⌊</mo><mo></mo><mo>⌋</mo></mrow>"),
        new(input: "forall", output:"<mo>∀</mo>"),
        new(input: "frown", output:"<mo>⌢</mo>"),
        new(input: "g", output:"<mi>g</mi>"),
        new(input: "gamma", output:"<mi>γ</mi>"),
        new(input: "gcd", output:"<mrow><mo>gcd</mo><mo></mo></mrow>"),
        new(input: "ge", output:"<mo>≥</mo>"),
        new(input: "geq", output:"<mo>≥</mo>"),
        new(input: "glb", output:"<mo>glb</mo>"),
        new(input: "grad", output:"<mo>∇</mo>"),
        new(input: "gt", output:"<mo>&gt;</mo>"),
        new(input: "mgt", output:"<mo>≫</mo>"),
        new(input: "gt=", output:"<mo>≥</mo>"),
        new(input: "hArr", output:"<mo>⇔</mo>"),
        new(input: "harr", output:"<mo>↔</mo>"),
        new(input: "hat", output:"<mover><mo></mo><mo>^</mo></mover>"),
        new(input: "if", output:"<mrow><mspace width=\"1ex\"></mspace><mo>if</mo><mspace width=\"1ex\"></mspace></mrow>"),
        new(input: "iff", output:"<mo>⇔</mo>"),
        new(input: "implies", output:"<mo>⇒</mo>"),
        new(input: "in", output:"<mo>∈</mo>"),
        new(input: "infty", output:"<mo>∞</mo>"),
        new(input: "int", output:"<mo>∫</mo>"),
        new(input: "iota", output:"<mi>ι</mi>"),
        new(input: "kappa", output:"<mi>κ</mi>"),
        new(input: "lArr", output:"<mo>⇐</mo>"),
        new(input: "lambda", output:"<mi>λ</mi>"),
        new(input: "lamda", output:"<mi>λ</mi>"),
        new(input: "langle", output:"<mrow><mo>〈</mo><mo></mo></mrow>"),
        new(input: "larr", output:"<mo>←</mo>"),
        new(input: "lceiling", output:"<mo>⌈</mo>"),
        new(input: "lcm", output:"<mrow><mo>lcm</mo><mo></mo></mrow>"),
        new(input: "ldots", output:"<mo>...</mo>"),
        new(input: "le", output:"<mo>≤</mo>"),
        new(input: "leftarrow", output:"<mo>←</mo>"),
        new(input: "leftrightarrow", output:"<mo>↔</mo>"),
        new(input: "leq", output:"<mo>≤</mo>"),
        new(input: "lfloor", output:"<mo>⌊</mo>"),
        new(input: "lim", output:"<mo>lim</mo>"),
        new(input: "ln", output:"<mrow><mo>ln</mo><mo></mo></mrow>"),
        new(input: "log", output:"<mrow><mo>log</mo><mo></mo></mrow>"),
        new(input: "lt", output:"<mo>&lt;</mo>"),
        new(input: "mlt", output:"<mo>≪</mo>"),
        new(input: "lt=", output:"<mo>≤</mo>"),
        new(input: "ltimes", output:"<mo>⋉</mo>"),
        new(input: "lub", output:"<mo>lub</mo>"),
        new(input: "mapsto", output:"<mo>↦</mo>"),
        new(input: "max", output:"<mo>max</mo>"),
        new(input: "min", output:"<mo>min</mo>"),
        new(input: "mod", output:"<mo>mod</mo>"),
        new(input: "models", output:"<mo>⊨</mo>"),
        new(input: "mu", output:"<mi>μ</mi>"),
        new(input: "nabla", output:"<mo>∇</mo>"),
        new(input: "ne", output:"<mo>≠</mo>"),
        new(input: "neg", output:"<mo>¬</mo>"),
        new(input: "nn", output:"<mo>∩</mo>"),
        new(input: "nnn", output:"<mo>⋂</mo>"),
        new(input: "norm", output:"<mrow><mo>∥</mo><mo></mo><mo>∥</mo></mrow>"),
        new(input: "not", output:"<mo>¬</mo>"),
        new(input: "notin", output:"<mo>∉</mo>"),
        new(input: "nu", output:"<mi>ν</mi>"),
        new(input: "o+", output:"<mo>⊕</mo>"),
        new(input: "o.", output:"<mo>⊙</mo>"),
        new(input: "obrace", output:"<mover><mo></mo><mo>⏞</mo></mover>"),
        new(input: "odot", output:"<mo>⊙</mo>"),
        new(input: "oint", output:"<mo>∮</mo>"),
        new(input: "omega", output:"<mi>ω</mi>"),
        new(input: "oo", output:"<mo>∞</mo>"),
        new(input: "oplus", output:"<mo>⊕</mo>"),
        new(input: "or", output:"<mrow><mspace width=\"1ex\"></mspace><mtext>or</mtext><mspace width=\"1ex\"></mspace></mrow>"),
        new(input: "otimes", output:"<mo>⊗</mo>"),
        new(input: "overbrace", output:"<mover><mo></mo><mo>⏞</mo></mover>"),
        new(input: "overline", output:"<mover><mo></mo><mo>¯</mo></mover>"),
        new(input: "ox", output:"<mo>⊗</mo>"),
        new(input: "partial", output:"<mo>∂</mo>"),
        new(input: "phi", output:"<mi>ϕ</mi>"),
        new(input: "pi", output:"<mi>π</mi>"),
        new(input: "pm", output:"<mo>±</mo>"),
        new(input: "prec", output:"<mo>≺</mo>"),
        new(input: "preceq", output:"<mo>⪯</mo>"),
        new(input: "prime", output:"<mo>′</mo>"),
        new(input: "prod", output:"<mo>∏</mo>"),
        new(input: "prop", output:"<mo>∝</mo>"),
        new(input: "propto", output:"<mo>∝</mo>"),
        new(input: "psi", output:"<mi>ψ</mi>"),
        new(input: "qquad", output:"<mo>&nbsp;&nbsp;&nbsp;&nbsp;</mo>"),
        new(input: "quad", output:"<mo>&nbsp;&nbsp;</mo>"),
        new(input: "rArr", output:"<mo>⇒</mo>"),
        new(input: "rangle", output:"<mo>〉</mo>"),
        new(input: "rarr", output:"<mo>→</mo>"),
        new(input: "rceiling", output:"<mo>⌉</mo>"),
        new(input: "rfloor", output:"<mo>⌋</mo>"),
        new(input: "rho", output:"<mi>ρ</mi>"),
        new(input: "rightarrow", output:"<mo>→</mo>"),
        new(input: "rightarrowtail", output:"<mo>↣</mo>"),
        new(input: "root", output:"<mroot><mo></mo><mo></mo></mroot>"),
        new(input: "rtimes", output:"<mo>⋊</mo>"),
        new(input: "sec", output:"<mrow><mo>sec</mo><mo></mo></mrow>"),
        new(input: "sech", output:"<mrow><mo>sech</mo><mo></mo></mrow>"),
        new(input: "setminus", output:"<mo>\\</mo>"),
        new(input: "sigma", output:"<mi>σ</mi>"),
        new(input: "sin", output:"<mrow><mo>sin</mo><mo></mo></mrow>"),
        new(input: "sinh", output:"<mrow><mo>sinh</mo><mo></mo></mrow>"),
        new(input: "sqrt", output:"<msqrt><mo></mo></msqrt>"),
        new(input: "square", output:"<mo>□</mo>"),
        new(input: "stackrel", output:"<mover><mo></mo><mo></mo></mover>"),
        new(input: "star", output:"<mo>⋆</mo>"),
        new(input: "sub", output:"<mo>⊂</mo>"),
        new(input: "sube", output:"<mo>⊆</mo>"),
        new(input: "subset", output:"<mo>⊂</mo>"),
        new(input: "subseteq", output:"<mo>⊆</mo>"),
        new(input: "succ", output:"<mo>≻</mo>"),
        new(input: "succeq", output:"<mo>⪰</mo>"),
        new(input: "sum", output:"<mo>∑</mo>"),
        new(input: "sup", output:"<mo>⊃</mo>"),
        new(input: "supe", output:"<mo>⊇</mo>"),
        new(input: "supset", output:"<mo>⊃</mo>"),
        new(input: "supseteq", output:"<mo>⊇</mo>"),
        new(input: "tan", output:"<mrow><mo>tan</mo><mo></mo></mrow>"),
        new(input: "tanh", output:"<mrow><mo>tanh</mo><mo></mo></mrow>"),
        new(input: "tau", output:"<mi>τ</mi>"),
        new(input: "therefore", output:"<mo>∴</mo>"),
        new(input: "theta", output:"<mi>θ</mi>"),
        new(input: "tilde", output:"<mover><mo></mo><mo>~</mo></mover>"),
        new(input: "times", output:"<mo>×</mo>"),
        new(input: "to", output:"<mo>→</mo>"),
        new(input: "top", output:"<mo>⊤</mo>"),
        new(input: "triangle", output:"<mo>△</mo>"),
        new(input: "twoheadrightarrow", output:"<mo>↠</mo>"),
        new(input: "twoheadrightarrowtail", output:"<mo>⤖</mo>"),
        new(input: "uarr", output:"<mo>↑</mo>"),
        new(input: "ubrace", output:"<munder><mo></mo><mo>⏟</mo></munder>"),
        new(input: "ul", output:"<munder><mo></mo><mo>̲</mo></munder>"),
        new(input: "underbrace", output:"<munder><mo></mo><mo>⏟</mo></munder>"),
        new(input: "underline", output:"<munder><mo></mo><mo>̲</mo></munder>"),
        new(input: "underset", output:"<munder><mo></mo><mo></mo></munder>"),
        new(input: "uparrow", output:"<mo>↑</mo>"),
        new(input: "upsilon", output:"<mi>υ</mi>"),
        new(input: "uu", output:"<mo>∪</mo>"),
        new(input: "uuu", output:"<mo>⋃</mo>"),
        new(input: "varepsilon", output:"<mi>ɛ</mi>"),
        new(input: "varphi", output:"<mi>φ</mi>"),
        new(input: "vartheta", output:"<mi>ϑ</mi>"),
        new(input: "vdash", output:"<mo>⊢</mo>"),
        new(input: "vdots", output:"<mo>⋮</mo>"),
        new(input: "vec", output:"<mover><mo></mo><mo>→</mo></mover>"),
        new(input: "vee", output:"<mo>∨</mo>"),
        new(input: "vv", output:"<mo>∨</mo>"),
        new(input: "vvv", output:"<mo>⋁</mo>"),
        new(input: "wedge", output:"<mo>∧</mo>"),
        new(input: "xi", output:"<mi>ξ</mi>"),
        new(input: "xx", output:"<mo>×</mo>"),
        new(input: "zeta", output:"<mi>ζ</mi>"),
        new(input: "{", output:"<mrow><mo>{</mo><mo></mo></mrow>"),
        new(input: "|", output:"<mrow><mo>∣</mo></mrow>"),
        new(input: "|--", output:"<mo>⊢</mo>"),
        new(input: "|->", output:"<mo>↦</mo>"),
        new(input: "|==", output:"<mo>⊨</mo>"),
        new(input: "|><", output:"<mo>⋉</mo>"),
        new(input: "|><|", output:"<mo>⋈</mo>"),
        new(input: "|__", output:"<mo>⌊</mo>"),
        new(input: "|~", output:"<mo>⌈</mo>"),
        new(input: "}", output:"<mo>}</mo>"),
        new(input: "~=", output:"<mo>≅</mo>"),
        new(input: "~|", output:"<mo>⌉</mo>"),
        new(input: "~~", output:"<mo>≈</mo>"),

        //unary, binary, and accents
        new(input: "f(x)/g(x)", output:"<mfrac><mrow><mi>f</mi><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow></mrow><mrow><mi>g</mi><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow></mrow></mfrac>"),
        new(input: "sin(x)/2", output:"<mfrac><mrow><mo>sin</mo><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow></mrow><mn>2</mn></mfrac>"),
        new(input: "cosx/2", output:"<mfrac><mrow><mo>cos</mo><mi>x</mi></mrow><mn>2</mn></mfrac>"),
        new(input: "absx", output:"<mrow><mo>|</mo><mi>x</mi><mo>|</mo></mrow>"),
        new(input: "norm x", output:"<mrow><mo>∥</mo><mi>x</mi><mo>∥</mo></mrow>"),
        new(input: "floor x/2", output:"<mfrac><mrow><mo>⌊</mo><mi>x</mi><mo>⌋</mo></mrow><mn>2</mn></mfrac>"),
        new(input: "ceil 5.2", output:"<mrow><mo>⌈</mo><mn>5.2</mn><mo>⌉</mo></mrow>"),
        new(input: "min_x 3", output:"<munder><mo>min</mo><mi>x</mi></munder><mn>3</mn>"),
        new(input: "sqrt4", output:"<msqrt><mn>4</mn></msqrt>"),
        new(input: "sqrt(x+1)", output:"<msqrt><mrow><mi>x</mi><mo>+</mo><mn>1</mn></mrow></msqrt>"),
        new(input: "root(3)(x)", output:"<mroot><mrow><mi>x</mi></mrow><mrow><mn>3</mn></mrow></mroot>"),
        new(input: "root3x", output:"<mroot><mi>x</mi><mn>3</mn></mroot>"),
        new(input: "stackrel3=", output:"<mover><mo>=</mo><mn>3</mn></mover>"),
        new(input: "stackrel(3)(=)", output:"<mover><mrow><mo>=</mo></mrow><mrow><mn>3</mn></mrow></mover>"),
        new(input: "overset(k)(=)", output:"<mover><mrow><mo>=</mo></mrow><mrow><mi>k</mi></mrow></mover>"),
        new(input: "underset(k)(=)", output:"<munder><mrow><mo>=</mo></mrow><mrow><mi>k</mi></mrow></munder>"),
        new(input: "tilde x", output:"<mover><mi>x</mi><mo>~</mo></mover>"),
        new(input: "hat x", output:"<mover><mi>x</mi><mo>^</mo></mover>"),
        new(input: "hat(xy)", output:"<mover><mrow><mi>x</mi><mi>y</mi></mrow><mo>^</mo></mover>"),
        new(input: "bar x", output:"<mover><mi>x</mi><mo>¯</mo></mover>"),
        new(input: "vec x", output:"<mover><mi>x</mi><mo stretchy=\"false\">→</mo></mover>"),
        new(input: "vec(xy)", output:"<mover><mrow><mi>x</mi><mi>y</mi></mrow><mo>→</mo></mover>"),
        new(input: "dot x", output:"<mover><mi>x</mi><mo>.</mo></mover>"),
        new(input: "ddot x", output:"<mover><mi>x</mi><mo>..</mo></mover>"),
        new(input: "ul x", output:"<munder><mi>x</mi><mo>̲</mo></munder>"),
        new(input: "ubrace(x+1)", output:"<munder><mrow><mi>x</mi><mo>+</mo><mn>1</mn></mrow><mo>⏟</mo></munder>"),
        new(input: "obrace(x+1)", output:"<mover><mrow><mi>x</mi><mo>+</mo><mn>1</mn></mrow><mo>⏞</mo></mover>"),
        new(input: "mbox(hi)", output:"<mrow><mtext>hi</mtext></mrow>"),
        new(input: "text(hi)", output:"<mrow><mtext>hi</mtext></mrow>"),
        new(input: "\"hi\"", output:"<mrow><mtext>hi</mtext></mrow>"),
        new(input: "cancel(x)", output:"<menclose notation=\"updiagonalstrike\"><mrow><mi>x</mi></mrow></menclose>"),

        //font and color
        new(input: "color(red)(x)", output:"<mstyle mathcolor=\"red\"><mrow><mi>x</mi></mrow></mstyle>"),
        new(input: "bb(x)", output:"<mstyle mathvariant=\"bold\"><mrow><mi>x</mi></mrow></mstyle>"),
        new(input: "sf(x)", output:"<mstyle mathvariant=\"sans-serif\"><mrow><mi>x</mi></mrow></mstyle>"),
        new(input: "bbb(x)", output:"<mstyle mathvariant=\"double-struck\"><mrow>𝕩</mrow></mstyle>"),
        new(input: "cc(x)", output:"<mstyle mathvariant=\"script\"><mrow>𝓍</mrow></mstyle>"),
        new(input: "tt(x)", output:"<mstyle mathvariant=\"monospace\"><mrow><mi>x</mi></mrow></mstyle>"),
        new(input: "fr(x)", output:"<mstyle mathvariant=\"fraktur\"><mrow>𝔵</mrow></mstyle>"),

        //basics
        new(input: "x", output:"<mi>x</mi>"),
        new(input: "2", output:"<mn>2</mn>"),
        new(input: "x^2", output:"<msup><mi>x</mi><mn>2</mn></msup>"),
        new(input: "x_2", output:"<msub><mi>x</mi><mn>2</mn></msub>"),
        new(input: "x_2^3", output:"<mrow><msubsup><mi>x</mi><mn>2</mn><mn>3</mn></msubsup></mrow>"),
        new(input: "2/3", output:"<mfrac><mn>2</mn><mn>3</mn></mfrac>"),
        new(input: "-2/3", output:"<mo>-</mo><mfrac><mn>2</mn><mn>3</mn></mfrac>"),
        new(input: "2-3", output:"<mn>2</mn><mo>-</mo><mn>3</mn>"),
        new(input: "(2+3)", output:"<mrow><mo>(</mo><mn>2</mn><mo>+</mo><mn>3</mn><mo>)</mo></mrow>"),

        //braces
        new(input: "2+(3/4+1)", output:"<mn>2</mn><mo>+</mo><mrow><mo>(</mo><mfrac><mn>3</mn><mn>4</mn></mfrac><mo>+</mo><mn>1</mn><mo>)</mo></mrow>"),
        new(input: "2+[3/4+1]", output:"<mn>2</mn><mo>+</mo><mrow><mo>[</mo><mfrac><mn>3</mn><mn>4</mn></mfrac><mo>+</mo><mn>1</mn><mo>]</mo></mrow>"),
        new(input: "2+|3/4+1|", output:"<mn>2</mn><mo>+</mo><mrow><mo>|</mo><mfrac><mn>3</mn><mn>4</mn></mfrac><mo>+</mo><mn>1</mn><mo>|</mo></mrow>"),
        new(input: "[2/3,4)", output:"<mrow><mo>[</mo><mfrac><mn>2</mn><mn>3</mn></mfrac><mo>,</mo><mn>4</mn><mo>)</mo></mrow>"),
        new(input: "{:2,3:}", output:"<mrow><mn>2</mn><mo>,</mo><mn>3</mn></mrow>"),
        new(input: "<<2,3>>", output:"<mrow><mo>〈</mo><mn>2</mn><mo>,</mo><mn>3</mn><mo>〉</mo></mrow>"),
        new(input: "(:2,3:)", output:"<mrow><mo>〈</mo><mn>2</mn><mo>,</mo><mn>3</mn><mo>〉</mo></mrow>"),

        //matrices and arrays
        new(input: "[(2,3),(4,5)]", output:"<mrow><mo>[</mo><mtable columnlines=\"none none\"><mtr><mtd><mn>2</mn></mtd><mtd><mn>3</mn></mtd></mtr><mtr><mtd><mn>4</mn></mtd><mtd><mn>5</mn></mtd></mtr></mtable><mo>]</mo></mrow>"),
        new(input: "[(2,3,4,5)]", output:"<mrow><mo>[</mo><mtable columnlines=\"none none none none\"><mtr><mtd><mn>2</mn></mtd><mtd><mn>3</mn></mtd><mtd><mn>4</mn></mtd><mtd><mn>5</mn></mtd></mtr></mtable><mo>]</mo></mrow>"),
        new(input: "((1),(2))", output:"<mrow><mo>(</mo><mtable columnlines=\"none\"><mtr><mtd><mn>1</mn></mtd></mtr><mtr><mtd><mn>2</mn></mtd></mtr></mtable><mo>)</mo></mrow>"),
        new(input: "{(1,if,x ge 3),(2,if,x gt 3):}", output:"<mrow><mo>{</mo><mtable columnlines=\"none none none\" columnalign=\"left\"><mtr><mtd><mn>1</mn></mtd><mtd><mrow><mspace width=\"1ex\"></mspace><mo>if</mo><mspace width=\"1ex\"></mspace></mrow></mtd><mtd><mi>x</mi><mo>≥</mo><mn>3</mn></mtd></mtr><mtr><mtd><mn>2</mn></mtd><mtd><mrow><mspace width=\"1ex\"></mspace><mo>if</mo><mspace width=\"1ex\"></mspace></mrow></mtd><mtd><mi>x</mi><mo>&gt;</mo><mn>3</mn></mtd></mtr></mtable></mrow>"),
        new(input: "[(1,2,|,3),(4,5,|,6)]", output:"<mrow><mo>[</mo><mtable columnlines=\"none solid none\"><mtr><mtd><mn>1</mn></mtd><mtd><mn>2</mn></mtd><mtd><mn>3</mn></mtd></mtr><mtr><mtd><mn>4</mn></mtd><mtd><mn>5</mn></mtd><mtd><mn>6</mn></mtd></mtr></mtable><mo>]</mo></mrow>"),

        //from the existing demos
        new(input: "int_2^3 3dx", output:"<mrow><msubsup><mo>∫</mo><mn>2</mn><mn>3</mn></msubsup></mrow><mn>3</mn><mrow><mi>d</mi><mi>x</mi></mrow>"),
        new(input: "sum_(n=1)^3 n", output:"<mrow><munderover><mo>∑</mo><mrow><mi>n</mi><mo>=</mo><mn>1</mn></mrow><mn>3</mn></munderover></mrow><mi>n</mi>"),
        new(input: "lim_(h->0)(f(x+h)-f(x))/h", output:"<munder><mo>lim</mo><mrow><mi>h</mi><mo>→</mo><mn>0</mn></mrow></munder><mfrac><mrow><mrow><mi>f</mi><mrow><mo>(</mo><mi>x</mi><mo>+</mo><mi>h</mi><mo>)</mo></mrow></mrow><mo>-</mo><mrow><mi>f</mi><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow></mrow></mrow><mi>h</mi></mfrac>"),
        new(input: "sin^-1(x)", output:"<mrow><msup><mo>sin</mo><mrow><mo>-</mo><mn>1</mn></mrow></msup><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow></mrow>"),
        new(input: "f(x)=sum_(n=0)^oo(f^((n))(a))/(n!)(x-a)^n", output:"<mrow><mi>f</mi><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow></mrow><mo>=</mo><mrow><munderover><mo>∑</mo><mrow><mi>n</mi><mo>=</mo><mn>0</mn></mrow><mo>∞</mo></munderover></mrow><mfrac><mrow><mrow><msup><mi>f</mi><mrow><mrow><mo>(</mo><mi>n</mi><mo>)</mo></mrow></mrow></msup><mrow><mo>(</mo><mi>a</mi><mo>)</mo></mrow></mrow></mrow><mrow><mi>n</mi><mo>!</mo></mrow></mfrac><msup><mrow><mo>(</mo><mi>x</mi><mo>-</mo><mi>a</mi><mo>)</mo></mrow><mi>n</mi></msup>"),
        new(input: "f(x)=\\sum_{n=0}^\\infty\\frac{f^{(n)}(a)}{n!}(x-a)^n", output:"<mrow><mi>f</mi><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow></mrow><mo>=</mo><mrow><munderover><mo>∑</mo><mrow><mi>n</mi><mo>=</mo><mn>0</mn></mrow><mo>∞</mo></munderover></mrow><mfrac><mrow><mrow><msup><mi>f</mi><mrow><mrow><mo>(</mo><mi>n</mi><mo>)</mo></mrow></mrow></msup><mrow><mo>(</mo><mi>a</mi><mo>)</mo></mrow></mrow></mrow><mrow><mi>n</mi><mo>!</mo></mrow></mfrac><msup><mrow><mo>(</mo><mi>x</mi><mo>-</mo><mi>a</mi><mo>)</mo></mrow><mi>n</mi></msup>"),
        new(input: "(a,b]={x in RR | a < x <= b}", output:"<mrow><mo>(</mo><mi>a</mi><mo>,</mo><mi>b</mi><mo>]</mo></mrow><mo>=</mo><mrow><mo>{</mo><mi>x</mi><mo>∈</mo><mo>ℝ</mo><mrow><mo>∣</mo></mrow><mi>a</mi><mo>&lt;</mo><mi>x</mi><mo>≤</mo><mi>b</mi><mo>}</mo></mrow>"),
        new(input: "abc-123.45^-1.1", output:"<mi>a</mi><mi>b</mi><mi>c</mi><mo>-</mo><msup><mn>123.45</mn><mrow><mo>-</mo><mn>1.1</mn></mrow></msup>"),
        new(input: "stackrel\"def\"= or \\stackrel{\\Delta}{=}", output:"<mover><mo>=</mo><mrow><mtext>def</mtext></mrow></mover><mrow><mspace width=\"1ex\"></mspace><mtext>or</mtext><mspace width=\"1ex\"></mspace></mrow><mover><mrow><mo>=</mo></mrow><mrow><mo>Δ</mo></mrow></mover>"),
        new(input: "{::}_(\\ 92)^238U", output:"<mrow><msubsup><mrow></mrow><mrow><mo>&nbsp;</mo><mn>92</mn></mrow><mn>238</mn></msubsup></mrow><mi>U</mi>"),
        new(input: "(cancel((x+1))(x-2))/(cancel((x+1))(x+3))", output:"<mfrac><mrow><menclose notation=\"updiagonalstrike\"><mrow><mrow><mo>(</mo><mi>x</mi><mo>+</mo><mn>1</mn><mo>)</mo></mrow></mrow></menclose><mrow><mo>(</mo><mi>x</mi><mo>-</mo><mn>2</mn><mo>)</mo></mrow></mrow><mrow><menclose notation=\"updiagonalstrike\"><mrow><mrow><mo>(</mo><mi>x</mi><mo>+</mo><mn>1</mn><mo>)</mo></mrow></mrow></menclose><mrow><mo>(</mo><mi>x</mi><mo>+</mo><mn>3</mn><mo>)</mo></mrow></mrow></mfrac>"),
        new(input: "a//b", output:"<mi>a</mi><mo>/</mo><mi>b</mi>"),
        new(input: "int_1^3 2x dx = x^2|_1^3", output:"<mrow><msubsup><mo>∫</mo><mn>1</mn><mn>3</mn></msubsup></mrow><mn>2</mn><mi>x</mi><mrow><mi>d</mi><mi>x</mi></mrow><mo>=</mo><msup><mi>x</mi><mn>2</mn></msup><mrow><msubsup><mrow><mo>∣</mo></mrow><mn>1</mn><mn>3</mn></msubsup></mrow>"),

        //from issue 15 tests
        new(input: "log_2(x)/5", output:"<mfrac><mrow><msub><mo>log</mo><mn>2</mn></msub><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow></mrow><mn>5</mn></mfrac>"),
        new(input: "log_2(x)+5", output:"<mrow><msub><mo>log</mo><mn>2</mn></msub><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow></mrow><mo>+</mo><mn>5</mn>"),
        new(input: "log_sqrt(5)3/5", output:"<mfrac><mrow><msub><mo>log</mo><msqrt><mrow><mn>5</mn></mrow></msqrt></msub><mn>3</mn></mrow><mn>5</mn></mfrac>"),
        new(input: "log_2^5(x)+5", output:"<mrow><mrow><msubsup><mo>log</mo><mn>2</mn><mn>5</mn></msubsup></mrow><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow></mrow><mo>+</mo><mn>5</mn>"),
        new(input: "2^f_2-3", output:"<msup><mn>2</mn><mi>f</mi></msup><mo>_</mo><mn>2</mn><mo>-</mo><mn>3</mn>"),
        new(input: "f_3(x)/5", output:"<mfrac><mrow><msub><mi>f</mi><mn>3</mn></msub><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow></mrow><mn>5</mn></mfrac>"),
        new(input: "2^(f_3(x)/5)", output:"<msup><mn>2</mn><mrow><mfrac><mrow><msub><mi>f</mi><mn>3</mn></msub><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow></mrow><mn>5</mn></mfrac></mrow></msup>"),
        new(input: "log_3x^2/5", output:"<mfrac><mrow><msub><mo>log</mo><mn>3</mn></msub><msup><mi>x</mi><mn>2</mn></msup></mrow><mn>5</mn></mfrac>"),
        new(input: "log_3x_0/5", output:"<mfrac><mrow><msub><mo>log</mo><mn>3</mn></msub><msub><mi>x</mi><mn>0</mn></msub></mrow><mn>5</mn></mfrac>"),
        new(input: "sin^2(x)/5", output:"<mfrac><mrow><msup><mo>sin</mo><mn>2</mn></msup><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow></mrow><mn>5</mn></mfrac>"),

        //spaces can be used to break tokens
        new(input: "3+ -4", output:"<mn>3</mn><mo>+</mo><mo>-</mo><mn>4</mn>"),
        new(input: "3+-4", output:"<mn>3</mn><mo>±</mo><mn>4</mn>"),

        //decimal place (if used as decimal separator) keeps token, but commas do not
        new(input: "3^5.234", output:"<msup><mn>3</mn><mn>5.234</mn></msup>"),
        new(input: "3^5,233", output:"<msup><mn>3</mn><mn>5</mn></msup><mo>,</mo><mn>233</mn>"),

        //check I/I grammar
        new(input: "(x+1)/4", output:"<mfrac><mrow><mi>x</mi><mo>+</mo><mn>1</mn></mrow><mn>4</mn></mfrac>"),
        new(input: "sqrtx/4", output:"<mfrac><msqrt><mi>x</mi></msqrt><mn>4</mn></mfrac>"),
        new(input: "root(3)(5)/4", output:"<mfrac><mroot><mrow><mn>5</mn></mrow><mrow><mn>3</mn></mrow></mroot><mn>4</mn></mfrac>"),
        new(input: "3^2/4^2", output:"<mfrac><msup><mn>3</mn><mn>2</mn></msup><msup><mn>4</mn><mn>2</mn></msup></mfrac>"),
        new(input: "3_2/4_2", output:"<mfrac><msub><mn>3</mn><mn>2</mn></msub><msub><mn>4</mn><mn>2</mn></msub></mfrac>"),
        new(input: "3^2/4^2", output:"<mfrac><msup><mn>3</mn><mn>2</mn></msup><msup><mn>4</mn><mn>2</mn></msup></mfrac>"),
        new(input: "3_2/4_2", output:"<mfrac><msub><mn>3</mn><mn>2</mn></msub><msub><mn>4</mn><mn>2</mn></msub></mfrac>"),
        new(input: "3_2^3/4_2", output:"<mfrac><mrow><msubsup><mn>3</mn><mn>2</mn><mn>3</mn></msubsup></mrow><msub><mn>4</mn><mn>2</mn></msub></mfrac>"),
        new(input: "vecx/hat3+vecx^2+(vec x)^2 + vec(x^2)", output:"<mfrac><mover><mi>x</mi><mo stretchy=\"false\">→</mo></mover><mover><mn>3</mn><mo>^</mo></mover></mfrac><mo>+</mo><msup><mover><mi>x</mi><mo stretchy=\"false\">→</mo></mover><mn>2</mn></msup><mo>+</mo><msup><mrow><mo>(</mo><mover><mi>x</mi><mo stretchy=\"false\">→</mo></mover><mo>)</mo></mrow><mn>2</mn></msup><mo>+</mo><mover><mrow><msup><mi>x</mi><mn>2</mn></msup></mrow><mo>→</mo></mover>"),
        //negative handling
        new(input: "-3-4", output:"<mo>-</mo><mn>3</mn><mo>-</mo><mn>4</mn>"),
        new(input: "(-3,-4)", output:"<mrow><mo>(</mo><mo>-</mo><mn>3</mn><mo>,</mo><mo>-</mo><mn>4</mn><mo>)</mo></mrow>"),
        new(input: "-(-2-4)-5", output:"<mo>-</mo><mrow><mo>(</mo><mo>-</mo><mn>2</mn><mo>-</mo><mn>4</mn><mo>)</mo></mrow><mo>-</mo><mn>5</mn>"),
        new(input: "2_-4^-5", output:"<mrow><msubsup><mn>2</mn><mrow><mo>-</mo><mn>4</mn></mrow><mrow><mo>-</mo><mn>5</mn></mrow></msubsup></mrow>"),
        new(input: "int_-sqrt(3)^4", output:"<mrow><msubsup><mo>∫</mo><mrow><mo>-</mo><msqrt><mrow><mn>3</mn></mrow></msqrt></mrow><mn>4</mn></msubsup></mrow>"),
        new(input: "-2/-3", output:"<mo>-</mo><mfrac><mn>2</mn><mrow><mo>-</mo><mn>3</mn></mrow></mfrac>"),
        new(input: "(-2)/-3", output:"<mfrac><mrow><mo>-</mo><mn>2</mn></mrow><mrow><mo>-</mo><mn>3</mn></mrow></mfrac>"),
        new(input: "-2/3-3/4", output:"<mo>-</mo><mfrac><mn>2</mn><mn>3</mn></mfrac><mo>-</mo><mfrac><mn>3</mn><mn>4</mn></mfrac>"),
        new(input: "-2^2", output:"<mo>-</mo><msup><mn>2</mn><mn>2</mn></msup>"),
        new(input: "-(x+1)/-(x+3)", output:"<mo>-</mo><mfrac><mrow><mi>x</mi><mo>+</mo><mn>1</mn></mrow><mrow><mo>-</mo><mrow><mo>(</mo><mi>x</mi><mo>+</mo><mn>3</mn><mo>)</mo></mrow></mrow></mfrac>"),

        //issue 40
        new(input: "{:{:x:}:}", output:"<mrow><mrow><mi>x</mi></mrow></mrow>"),
        new(input: "{:1+{:x:}+3:}", output:"<mrow><mn>1</mn><mo>+</mo><mrow><mi>x</mi></mrow><mo>+</mo><mn>3</mn></mrow>"),

        //issue37
        new(input: "(:2,3]", output:"<mrow><mo>〈</mo><mn>2</mn><mo>,</mo><mn>3</mn><mo>]</mo></mrow>"),
        new(input: "[2,3rangle", output:"<mrow><mo>[</mo><mn>2</mn><mo>,</mo><mn>3</mn><mo>〉</mo></mrow>"),
        new(input: "2,3)", output:"<mn>2</mn><mo>,</mo><mn>3</mn><mo>)</mo>"),
        new(input: "(2,3", output:"<mrow><mo>(</mo><mn>2</mn><mo>,</mo><mn>3</mn></mrow>"),

        //issue42
        new(input: "[(1,2,3,|,4),(5,6,7, |,8)]", output:"<mrow><mo>[</mo><mtable columnlines=\"none none solid none\"><mtr><mtd><mn>1</mn></mtd><mtd><mn>2</mn></mtd><mtd><mn>3</mn></mtd><mtd><mn>4</mn></mtd></mtr><mtr><mtd><mn>5</mn></mtd><mtd><mn>6</mn></mtd><mtd><mn>7</mn></mtd><mtd><mn>8</mn></mtd></mtr></mtable><mo>]</mo></mrow>"),
        new(input: "[(1,2,3, | ,4,5),(5,6,7, | ,8,9)]", output:"<mrow><mo>[</mo><mtable columnlines=\"none none solid none none\"><mtr><mtd><mn>1</mn></mtd><mtd><mn>2</mn></mtd><mtd><mn>3</mn></mtd><mtd><mn>4</mn></mtd><mtd><mn>5</mn></mtd></mtr><mtr><mtd><mn>5</mn></mtd><mtd><mn>6</mn></mtd><mtd><mn>7</mn></mtd><mtd><mn>8</mn></mtd><mtd><mn>9</mn></mtd></mtr></mtable><mo>]</mo></mrow>"),
        new(input: "[(1,|,2,3,4),(5,|,6,7,8)]", output:"<mrow><mo>[</mo><mtable columnlines=\"solid none none none\"><mtr><mtd><mn>1</mn></mtd><mtd><mn>2</mn></mtd><mtd><mn>3</mn></mtd><mtd><mn>4</mn></mtd></mtr><mtr><mtd><mn>5</mn></mtd><mtd><mn>6</mn></mtd><mtd><mn>7</mn></mtd><mtd><mn>8</mn></mtd></mtr></mtable><mo>]</mo></mrow>"),
        new(input: "[(1,|,3,|,4),(5,|,7,|,8)]", output:"<mrow><mo>[</mo><mtable columnlines=\"solid solid none\"><mtr><mtd><mn>1</mn></mtd><mtd><mn>3</mn></mtd><mtd><mn>4</mn></mtd></mtr><mtr><mtd><mn>5</mn></mtd><mtd><mn>7</mn></mtd><mtd><mn>8</mn></mtd></mtr></mtable><mo>]</mo></mrow>"),
        new(input: "[(2,|x|,5),(3,|y|,4)]", output:"<mrow><mo>[</mo><mtable columnlines=\"none none none\"><mtr><mtd><mn>2</mn></mtd><mtd><mrow><mo>|</mo><mi>x</mi><mo>|</mo></mrow></mtd><mtd><mn>5</mn></mtd></mtr><mtr><mtd><mn>3</mn></mtd><mtd><mrow><mo>|</mo><mi>y</mi><mo>|</mo></mrow></mtd><mtd><mn>4</mn></mtd></mtr></mtable><mo>]</mo></mrow>"),
        new(input: "[(1,|,2,|x|,5),(3,|,4,|y|,7)]", output:"<mrow><mo>[</mo><mtable columnlines=\"solid none none none\"><mtr><mtd><mn>1</mn></mtd><mtd><mn>2</mn></mtd><mtd><mrow><mo>|</mo><mi>x</mi><mo>|</mo></mrow></mtd><mtd><mn>5</mn></mtd></mtr><mtr><mtd><mn>3</mn></mtd><mtd><mn>4</mn></mtd><mtd><mrow><mo>|</mo><mi>y</mi><mo>|</mo></mrow></mtd><mtd><mn>7</mn></mtd></mtr></mtable><mo>]</mo></mrow>"),
        new(input: "[(1,2,3,|,4),(5,6,7,8,9)]", output:"<mrow><mo>[</mo><mtable columnlines=\"none none solid none\"><mtr><mtd><mn>1</mn></mtd><mtd><mn>2</mn></mtd><mtd><mn>3</mn></mtd><mtd><mn>4</mn></mtd></mtr><mtr><mtd><mn>5</mn></mtd><mtd><mn>6</mn></mtd><mtd><mn>7</mn></mtd><mtd><mn>8</mn></mtd><mtd><mn>9</mn></mtd></mtr></mtable><mo>]</mo></mrow>"),
        new(input: "[(1,2,3,|,4),(5,6,7,8)]", output:"<mrow><mo>[</mo><mrow><mo>(</mo><mn>1</mn><mo>,</mo><mn>2</mn><mo>,</mo><mn>3</mn><mo>,</mo><mrow><mo>∣</mo></mrow><mo>,</mo><mn>4</mn><mo>)</mo></mrow><mo>,</mo><mrow><mo>(</mo><mn>5</mn><mo>,</mo><mn>6</mn><mo>,</mo><mn>7</mn><mo>,</mo><mn>8</mn><mo>)</mo></mrow><mo>]</mo></mrow>"),
        new(input: "[(1,2,3,4,5),(5,6,7,|,9)]", output:"<mrow><mo>[</mo><mtable columnlines=\"none none none none none\"><mtr><mtd><mn>1</mn></mtd><mtd><mn>2</mn></mtd><mtd><mn>3</mn></mtd><mtd><mn>4</mn></mtd><mtd><mn>5</mn></mtd></mtr><mtr><mtd><mn>5</mn></mtd><mtd><mn>6</mn></mtd><mtd><mn>7</mn></mtd><mtd><mn>9</mn></mtd></mtr></mtable><mo>]</mo></mrow>"),
        new(input: "[(1,2,3,4),(5,6,7,|,9)]", output:"<mrow><mo>[</mo><mrow><mo>(</mo><mn>1</mn><mo>,</mo><mn>2</mn><mo>,</mo><mn>3</mn><mo>,</mo><mn>4</mn><mo>)</mo></mrow><mo>,</mo><mrow><mo>(</mo><mn>5</mn><mo>,</mo><mn>6</mn><mo>,</mo><mn>7</mn><mo>,</mo><mrow><mo>∣</mo></mrow><mo>,</mo><mn>9</mn><mo>)</mo></mrow><mo>]</mo></mrow>"),
        new(input: "[(1,2,3,|),(5,6,7,|)]", output:"<mrow><mo>[</mo><mtable columnlines=\"none none solid none\"><mtr><mtd><mn>1</mn></mtd><mtd><mn>2</mn></mtd><mtd><mn>3</mn></mtd><mtd></mtd></mtr><mtr><mtd><mn>5</mn></mtd><mtd><mn>6</mn></mtd><mtd><mn>7</mn></mtd><mtd></mtd></mtr></mtable><mo>]</mo></mrow>"),
        new(input: "|x/2+3|,|x-4/5|", output:"<mrow><mo>|</mo><mfrac><mi>x</mi><mn>2</mn></mfrac><mo>+</mo><mn>3</mn><mo>|</mo></mrow><mo>,</mo><mrow><mo>|</mo><mi>x</mi><mo>-</mo><mfrac><mn>4</mn><mn>5</mn></mfrac><mo>|</mo></mrow>"),
        new(input: "int_2^4 2x dx = x^2|_2^4", output:"<mrow><msubsup><mo>∫</mo><mn>2</mn><mn>4</mn></msubsup></mrow><mn>2</mn><mi>x</mi><mrow><mi>d</mi><mi>x</mi></mrow><mo>=</mo><msup><mi>x</mi><mn>2</mn></msup><mrow><msubsup><mrow><mo>∣</mo></mrow><mn>2</mn><mn>4</mn></msubsup></mrow>"),

        //issue74
        new(input: "3+sin(x)/5-2Sin(x)", output:"<mn>3</mn><mo>+</mo><mfrac><mrow><mo>sin</mo><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow></mrow><mn>5</mn></mfrac><mo>-</mo><mn>2</mn><mrow><mo>Sin</mo><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow></mrow>"),
        new(input: "5+sin(x)+Sin(x)+\"test Since\"", output:"<mn>5</mn><mo>+</mo><mrow><mo>sin</mo><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow></mrow><mo>+</mo><mrow><mo>Sin</mo><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow></mrow><mo>+</mo><mrow><mtext>test Since</mtext></mrow>"),
        new(input: "Log(x)/3 +log(x)/3", output:"<mfrac><mrow><mo>Log</mo><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow></mrow><mn>3</mn></mfrac><mo>+</mo><mfrac><mrow><mo>log</mo><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow></mrow><mn>3</mn></mfrac>"),
        new(input: "Abs(3) + abs(3)", output:"<mrow><mo>|</mo><mrow><mn>3</mn></mrow><mo>|</mo></mrow><mo>+</mo><mrow><mo>|</mo><mrow><mn>3</mn></mrow><mo>|</mo></mrow>"),

        //issue86
        new(input: "3 + id(hi)(x^2)+class(red)(4)", output:"<mn>3</mn><mo>+</mo><mrow id=\"hi\"><mrow><msup><mi>x</mi><mn>2</mn></msup></mrow></mrow><mo>+</mo><mrow class=\"red\"><mrow><mn>4</mn></mrow></mrow>"),

        //issue 94
        new(input: "f^2(x)/5", output:"<mfrac><mrow><msup><mi>f</mi><mn>2</mn></msup><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow></mrow><mn>5</mn></mfrac>"),
        new(input: "f^2x/5", output:"<msup><mi>f</mi><mn>2</mn></msup><mfrac><mi>x</mi><mn>5</mn></mfrac>"),
        new(input: "1/f^2x^2", output:"<mfrac><mn>1</mn><msup><mi>f</mi><mn>2</mn></msup></mfrac><msup><mi>x</mi><mn>2</mn></msup>"),
        new(input: "1/fx^2", output:"<mfrac><mn>1</mn><mi>f</mi></mfrac><msup><mi>x</mi><mn>2</mn></msup>"),
        new(input: "f'(x)/5", output:"<mi>f</mi><mo>′</mo><mfrac><mrow><mi>x</mi></mrow><mn>5</mn></mfrac>"),

        // issue 113
        new(input: "[[1,2]]/4", output:"<mfrac><mrow><mo>[</mo><mtable columnlines=\"none none\"><mtr><mtd><mn>1</mn></mtd><mtd><mn>2</mn></mtd></mtr></mtable><mo>]</mo></mrow><mn>4</mn></mfrac>"),
        new(input: "(x+2)/3", output:"<mfrac><mrow><mi>x</mi><mo>+</mo><mn>2</mn></mrow><mn>3</mn></mfrac>"),

        // issue 114
        new(input: "u_-3 + u_- 3", output:"<msub><mi>u</mi><mrow><mo>-</mo><mn>3</mn></mrow></msub><mo>+</mo><msub><mi>u</mi><mo>-</mo></msub><mn>3</mn>"),
        new(input: "2^- +3", output:"<msup><mn>2</mn><mo>-</mo></msup><mo>+</mo><mn>3</mn>"),

        // sim
        new(input: "3~2,5sim4", output:"<mn>3</mn><mo>∼</mo><mn>2</mn><mo>,</mo><mn>5</mn><mo>∼</mo><mn>4</mn>"),
        //overparen
        new(input: "overparen(AB)", output:"<mover><mrow><mi>A</mi><mi>B</mi></mrow><mo>⏜</mo></mover>"),
        new(input: "overarc(AB)", output:"<mover><mrow><mi>A</mi><mi>B</mi></mrow><mo>⏜</mo></mover>"),

        //mp
        new(input: "(x-+5)(xmp5)", output:"<mrow><mo>(</mo><mi>x</mi><mo>∓</mo><mn>5</mn><mo>)</mo></mrow><mrow><mo>(</mo><mi>x</mi><mo>∓</mo><mn>5</mn><mo>)</mo></mrow>"),

        //bad/incomplete input
        new(input: "3/", output:"<mfrac><mn>3</mn><mo></mo></mfrac>"),
        new(input: "2^", output:"<msup><mn>2</mn><mo></mo></msup>"),
        new(input: "2^+3", output:"<msup><mn>2</mn><mo>+</mo></msup><mn>3</mn>"),
        new(input: "/4", output:"<mo>/</mo><mn>4</mn>"),
        new(input: "lim_(x rarr 2^-) f(x)", output:"<munder><mo>lim</mo><mrow><mi>x</mi><mo>→</mo><msup><mn>2</mn><mo>-</mo></msup></mrow></munder><mrow><mi>f</mi><mrow><mo>(</mo><mi>x</mi><mo>)</mo></mrow></mrow>"),
    ];
}