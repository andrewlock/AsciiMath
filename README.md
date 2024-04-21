# AsciiMath

![Build status](https://github.com/andrewlock/AsciiMath/actions/workflows/BuildAndPack.yml/badge.svg)
[![NuGet](https://img.shields.io/nuget/v/AsciiMath.svg)](https://www.nuget.org/packages/AsciiMath/)

An AsciiMath parser for .NET that converts `string` AsciiMath expressions into MathML.  

> This is a .NET port of the ruby [AsciiDoctor/AsciiMath](https://github.com/asciidoctor/asciimath) implementation

Add the package to your application using

```bash
dotnet add package AsciiMath --prerelease
```

To use the parser, call the static `Parser.ToMathMl(input)` method with the AsciiMath `string` 

```csharp
var asciiMath = "int_-1^1 sqrt(1-x^2)dx = pi/2";
var converted = Parser.ToMathMl(asciiMath);
Console.WriteLine(converted);
// prints <math><msub><mo>&#x222B;</mo><mo>&#x2212;</mo></msub><msup><mn>1</mn><mn>1</mn></msup><msqrt><mrow><mn>1</mn><mo>&#x2212;</mo><msup><mi>x</mi><mn>2</mn></msup></mrow></msqrt><mi>dx</mi><mo>=</mo><mfrac><mi>&#x3C0;</mi><mn>2</mn></mfrac></math>
```

## Status

This parser is currently in early development and may evolve both in the features it provides and the interface it uses.