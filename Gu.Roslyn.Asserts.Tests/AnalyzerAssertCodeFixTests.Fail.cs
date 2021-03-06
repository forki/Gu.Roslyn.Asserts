﻿// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using Gu.Roslyn.Asserts.Tests.CodeFixes;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    [TestFixture]
    public partial class AnalyzerAssertCodeFixTests
    {
        public class Fail
        {
            [TearDown]
            public void TearDown()
            {
                AnalyzerAssert.MetadataReferences.Clear();
            }

            [Test]
            public void SingleClassExplicitTitle()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value;
    }
}";
                var expected = "Did not find a code fix with title WRONG.\r\n" +
                               "Found:\r\n" +
                               "Rename to: value\r\n";

                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));

                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, fixedCode, "WRONG"));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(new[] { code }, fixedCode, "WRONG"));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.CodeFix(new FieldNameMustNotBeginWithUnderscore(), new DontUseUnderscoreCodeFixProvider(), new[] { code }, fixedCode, "WRONG"));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void SingleClassTwoIndicatedErrors()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value1;
        private readonly int ↓_value2;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value1;
        private readonly int value2;
    }
}";
                var expected = "Code analyzed with Gu.Roslyn.Asserts.Tests.FieldNameMustNotBeginWithUnderscore generated more than one diagnostic fixable by Gu.Roslyn.Asserts.Tests.CodeFixes.DontUseUnderscoreCodeFixProvider.\r\n" +
                               "The analyzed code contained the following diagnostics: {SA1309, SA1309}\r\n" +
                               "The code fix supports the following diagnostics: {SA1309, ID1, ID2}\r\n" +
                               "Maybe you meant to call AnalyzerAssert.FixAll?";

                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, fixedCode));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(new[] { code }, fixedCode));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.CodeFix(new FieldNameMustNotBeginWithUnderscore(), new DontUseUnderscoreCodeFixProvider(), new[] { code }, fixedCode));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void SingleClassTwoErrors()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value1;
        private readonly int _value2;
    }
}";

                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, null));
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Actual:\r\n" +
                               "SA1309 Field '_value2' must not begin with an underscore\r\n" +
                               "  at line 6 and character 29 in file Foo.cs | private readonly int ↓_value2;\r\n";
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void SingleClassOneErrorWrongPosition()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private ↓readonly int _value1;
    }
}";

                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, null));
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Expected:\r\n" +
                               "SA1309 \r\n" +
                               "  at line 5 and character 16 in file Foo.cs | private ↓readonly int _value1;\r\n" +
                               "Actual:\r\n" +
                               "SA1309 Field '_value1' must not begin with an underscore\r\n" +
                               "  at line 5 and character 29 in file Foo.cs | private readonly int ↓_value1;\r\n";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void SingleClassOneErrorNoFix()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var temp = code.AssertReplace("↓", string.Empty);
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, NoCodeFixProvider>(code, temp));
                var expected = "Expected one code fix, was 0.";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void SingleClassOneErrorTwoFixes()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var temp = code.AssertReplace("↓", string.Empty);
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, TwoFixProvider>(code, temp));
                var expected = "Expected only one code fix, found 2:\r\n" +
                               "Rename to: value1\r\n" +
                               "Rename to: value2\r\n" +
                               "Use the overload that specifies title.";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void SingleClassOneErrorEmptyFix()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var temp = code.AssertReplace("↓", string.Empty);
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, EmptyCodeFixProvider>(code, temp));
                var expected = "Gu.Roslyn.Asserts.Tests.CodeFixes.EmptyCodeFixProvider did not change any document.";
                Console.Write(exception.Message);
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void FixDoesNotMatchAnalyzer()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int bar;
    }
}";

                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.CodeFix<NoErrorAnalyzer, DontUseUnderscoreCodeFixProvider>(code, fixedCode));
                var expected = "Analyzer Gu.Roslyn.Asserts.Tests.NoErrorAnalyzer does not produce diagnostics fixable by Gu.Roslyn.Asserts.Tests.CodeFixes.DontUseUnderscoreCodeFixProvider.\r\n" +
                               "The analyzer produces the following diagnostics: {NoError}\r\n" +
                               "The code fix supports the following diagnostics: {SA1309, ID1, ID2}";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void OneErrorWhenFixedCodeDoesNotMatchExpected()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int bar;
    }
}";
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, fixedCode));
                var expected = "Mismatch on line 6 of file Foo.cs\r\n" +
                               "Expected:         private readonly int bar;\r\n" +
                               "Actual:           private readonly int value;\r\n" +
                               "                                       ^\r\n" +
                               "Expected:\r\n\r\n" +
                               "namespace RoslynSandbox\r\n" +
                               "{\r\n" +
                               "    class Foo\r\n" +
                               "    {\r\n" +
                               "        private readonly int bar;\r\n" +
                               "    }\r\n" +
                               "}\r\n" +
                               "Actual:\r\n\r\n" +
                               "namespace RoslynSandbox\r\n" +
                               "{\r\n" +
                               "    class Foo\r\n" +
                               "    {\r\n" +
                               "        private readonly int value;\r\n" +
                               "    }\r\n" +
                               "}\r\n";
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void TwoClassesOneErrorWhenFixedCodeDoesNotMatchExpected()
            {
                var barCode = @"
namespace RoslynSandbox
{
    class Bar
    {
        private readonly int value;
    }
}";

                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int bar;
    }
}";
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(new[] { barCode, code }, fixedCode));
                var expected = "Mismatch on line 6 of file Foo.cs\r\n" +
                               "Expected:         private readonly int bar;\r\n" +
                               "Actual:           private readonly int value;\r\n" +
                               "                                       ^\r\n" +
                               "Expected:\r\n\r\n" +
                               "namespace RoslynSandbox\r\n" +
                               "{\r\n" +
                               "    class Foo\r\n" +
                               "    {\r\n" +
                               "        private readonly int bar;\r\n" +
                               "    }\r\n" +
                               "}\r\n" +
                               "Actual:\r\n\r\n" +
                               "namespace RoslynSandbox\r\n" +
                               "{\r\n" +
                               "    class Foo\r\n" +
                               "    {\r\n" +
                               "        private readonly int value;\r\n" +
                               "    }\r\n" +
                               "}\r\n";
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void WhenFixIntroducesCompilerErrors()
            {
                var code = @"
namespace RoslynSandbox
{
    ↓class Foo
    {
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        public event EventHandler SomeEvent;
    }
}";
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.CodeFix<ClassMustHaveEventAnalyzer, InsertEventFixProvider>(code, fixedCode));
                var expected = "Gu.Roslyn.Asserts.Tests.CodeFixes.InsertEventFixProvider introduced syntax errors.\r\n" +
                               "CS0518 Predefined type 'System.Object' is not defined or imported\r\n" +
                               "  at line 3 and character 10 in file Foo.cs | class ↓Foo\r\n" +
                               "CS0518 Predefined type 'System.Object' is not defined or imported\r\n" +
                               "  at line 5 and character 21 in file Foo.cs | public event ↓EventHandler SomeEvent;\r\n" +
                               "CS0246 The type or namespace name 'EventHandler' could not be found (are you missing a using directive or an assembly reference?)\r\n" +
                               "  at line 5 and character 21 in file Foo.cs | public event ↓EventHandler SomeEvent;\r\n" +
                               "CS0518 Predefined type 'System.Void' is not defined or imported\r\n" +
                               "  at line 5 and character 34 in file Foo.cs | public event EventHandler ↓SomeEvent;\r\n" +
                               "CS0518 Predefined type 'System.Void' is not defined or imported\r\n" +
                               "  at line 5 and character 34 in file Foo.cs | public event EventHandler ↓SomeEvent;\r\n" +
                               "CS1729 'object' does not contain a constructor that takes 0 arguments\r\n" +
                               "  at line 3 and character 10 in file Foo.cs | class ↓Foo\r\n" +
                               "First source file with error is:\r\n\r\n" +
                               "namespace RoslynSandbox\r\n" +
                               "{\r\n" +
                               "    class Foo\r\n" +
                               "    {\r\n" +
                               "        public event EventHandler SomeEvent;\r\n" +
                               "    }\r\n" +
                               "}\r\n";
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void WhenFixIsCorrectWithMetadataReferences()
            {
                var code = @"
namespace RoslynSandbox
{
    using System;

    ↓class Foo
    {
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    using System;

    class Foo
    {
        public event EventHandler SomeEvent;
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                AnalyzerAssert.CodeFix<ClassMustHaveEventAnalyzer, InsertEventFixProvider>(code, fixedCode);
            }

            [Test]
            public void IndicatedAndActualPositionDoNotMatch()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private ↓readonly int _value1;
    }
}";

                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, null));
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Expected:\r\n" +
                               "SA1309 \r\n" +
                               "  at line 5 and character 16 in file Foo.cs | private ↓readonly int _value1;\r\n" +
                               "Actual:\r\n" +
                               "SA1309 Field '_value1' must not begin with an underscore\r\n" +
                               "  at line 5 and character 29 in file Foo.cs | private readonly int ↓_value1;\r\n";
                Assert.AreEqual(expected, exception.Message);
            }
        }
    }
}