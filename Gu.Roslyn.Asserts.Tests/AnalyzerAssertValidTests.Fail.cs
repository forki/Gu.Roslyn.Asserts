﻿// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public partial class AnalyzerAssertValidTests
    {
        public class Fail
        {
            private static readonly bool Throw = false; // for testing what the output is in the runner.

            [Test]
            public void SingleClassFieldNameMustNotBeginWithUnderscore()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int _value = 1;
    }
}";
                var expected = "Expected no diagnostics, found:\r\n" +
                               "SA1309 Field '_value' must not begin with an underscore\r\n" +
                               "  at line 5 and character 29 in file Foo.cs | private readonly int ↓_value = 1;\r\n";

                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid<FieldNameMustNotBeginWithUnderscore>(code));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscore), code));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid(new FieldNameMustNotBeginWithUnderscore(), code));
                Assert.AreEqual(expected, exception.Message);

                if (Throw)
                {
                    AnalyzerAssert.Valid<FieldNameMustNotBeginWithUnderscore>(code);
                }
            }

            [Test]
            public void SingleClassFieldNameMustNotBeginWithUnderscoreDisabled()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int _value = 1;
    }
}";
                var expected = "Expected no diagnostics, found:\r\n" +
                               "SA1309 Field \'_value\' must not begin with an underscore\r\n" +
                               "  at line 5 and character 29 in file Foo.cs | private readonly int ↓_value = 1;\r\n";

                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid<FieldNameMustNotBeginWithUnderscoreDisabled>(code));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscoreDisabled), code));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid(new FieldNameMustNotBeginWithUnderscoreDisabled(), code));
                Assert.AreEqual(expected, exception.Message);

                if (Throw)
                {
                    AnalyzerAssert.Valid<FieldNameMustNotBeginWithUnderscoreDisabled>(code);
                }
            }

            [Test]
            public void TwoClassesOneError()
            {
                var foo1 = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int _value = 1;
    }
}";
                var foo2 = @"
namespace RoslynSandbox
{
    class Foo2
    {
    }
}";
                var expected = "Expected no diagnostics, found:\r\n" +
                               "SA1309 Field '_value' must not begin with an underscore\r\n" +
                               "  at line 5 and character 29 in file Foo1.cs | private readonly int ↓_value = 1;\r\n";

                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid<FieldNameMustNotBeginWithUnderscore>(foo1, foo2));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscore), foo1, foo2));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid(new FieldNameMustNotBeginWithUnderscore(), foo1, foo2));
                Assert.AreEqual(expected, exception.Message);

                if (Throw)
                {
                    AnalyzerAssert.Valid<FieldNameMustNotBeginWithUnderscore>(foo1, foo2);
                }
            }

            [Test]
            public void TwoClassesTwoErrors()
            {
                var foo1 = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int _value1 = 1;
    }
}";
                var foo2 = @"
namespace RoslynSandbox
{
    class Foo2
    {
        private readonly int _value2 = 2;
    }
}";
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid<FieldNameMustNotBeginWithUnderscore>(foo1, foo2));
                StringAssert.Contains("SA1309 Field '_value1' must not begin with an underscore", exception.Message);
                StringAssert.Contains("SA1309 Field '_value2' must not begin with an underscore", exception.Message);

                exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscore), foo1, foo2));
                StringAssert.Contains("SA1309 Field '_value1' must not begin with an underscore", exception.Message);
                StringAssert.Contains("SA1309 Field '_value2' must not begin with an underscore", exception.Message);

                exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid(new FieldNameMustNotBeginWithUnderscore(), foo1, foo2));
                StringAssert.Contains("SA1309 Field '_value1' must not begin with an underscore", exception.Message);
                StringAssert.Contains("SA1309 Field '_value2' must not begin with an underscore", exception.Message);

                if (Throw)
                {
                    AnalyzerAssert.Valid<FieldNameMustNotBeginWithUnderscore>(foo1, foo2);
                }
            }

            [Test]
            public void TwoProjectsTwoErrors()
            {
                var foo1 = @"
namespace Project1
{
    class Foo1
    {
        private readonly int _value1 = 1;
    }
}";
                var foo2 = @"
namespace Project2
{
    class Foo2
    {
        private readonly int _value2 = 2;
    }
}";
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid<FieldNameMustNotBeginWithUnderscore>(foo1, foo2));
                StringAssert.Contains("SA1309 Field '_value1' must not begin with an underscore", exception.Message);
                StringAssert.Contains("SA1309 Field '_value2' must not begin with an underscore", exception.Message);

                exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscore), foo1, foo2));
                StringAssert.Contains("SA1309 Field '_value1' must not begin with an underscore", exception.Message);
                StringAssert.Contains("SA1309 Field '_value2' must not begin with an underscore", exception.Message);

                exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid(new FieldNameMustNotBeginWithUnderscore(), foo1, foo2));
                StringAssert.Contains("SA1309 Field '_value1' must not begin with an underscore", exception.Message);
                StringAssert.Contains("SA1309 Field '_value2' must not begin with an underscore", exception.Message);

                if (Throw)
                {
                    AnalyzerAssert.Valid<FieldNameMustNotBeginWithUnderscore>(foo1, foo2);
                }
            }

            [TestCase("ClassLibrary1.csproj", "ClassLibrary1Class1.cs(8,21): warning SA1309: Field '_value' must not begin with an underscore")]
            [TestCase("ClassLibrary2.csproj", "ClassLibrary2Class1.cs(8,21): warning SA1309: Field '_value' must not begin with an underscore")]
            public void ProjectFileFieldNameMustNotBeginWithUnderscoreDisabled(string fileName, string expected)
            {
                var csproj = CodeFactory.FindProjectFile(fileName);
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid<FieldNameMustNotBeginWithUnderscoreDisabled>(csproj));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscoreDisabled), csproj));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid(new FieldNameMustNotBeginWithUnderscoreDisabled(), csproj));
                Assert.AreEqual(expected, exception.Message);

                if (Throw)
                {
                    AnalyzerAssert.Valid<FieldNameMustNotBeginWithUnderscoreDisabled>(csproj);
                }
            }

            [TestCase("ClassLibrary1.csproj", "ClassLibrary1Class1.cs(8,21): warning SA1309: Field '_value' must not begin with an underscore")]
            [TestCase("ClassLibrary2.csproj", "ClassLibrary2Class1.cs(8,21): warning SA1309: Field '_value' must not begin with an underscore")]
            public void SolutionFieldNameMustNotBeginWithUnderscoreDisabled(string fileName, string expected)
            {
                var sln = CodeFactory.CreateSolution(CodeFactory.FindProjectFile(fileName), new[] { new FieldNameMustNotBeginWithUnderscoreDisabled() }, AnalyzerAssert.MetadataReferences);
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid<FieldNameMustNotBeginWithUnderscoreDisabled>(sln));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscoreDisabled), sln));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid(new FieldNameMustNotBeginWithUnderscoreDisabled(), sln));
                Assert.AreEqual(expected, exception.Message);

                if (Throw)
                {
                    AnalyzerAssert.Valid<FieldNameMustNotBeginWithUnderscoreDisabled>(sln);
                }
            }

            [Test]
            public void WithExpectedDiagnosticWithWrongId()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value1;
    }
}";
                var expected = "Analyzer Gu.Roslyn.Asserts.Tests.FieldNameMustNotBeginWithUnderscore does not produce a diagnostic with ID WRONG.\r\n" +
                               "The analyzer produces the following diagnostics: {SA1309}\r\n" +
                               "The expected diagnostic is: WRONG";

                var expectedDiagnostic = ExpectedDiagnostic.Create("WRONG");

                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid<FieldNameMustNotBeginWithUnderscore>(expectedDiagnostic, code));
                CodeAssert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscore), expectedDiagnostic, code));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid(new FieldNameMustNotBeginWithUnderscore(), expectedDiagnostic, code));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid<FieldNameMustNotBeginWithUnderscore>(new[] { expectedDiagnostic }, code));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscore), new[] { expectedDiagnostic }, code));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid(new FieldNameMustNotBeginWithUnderscore(), new[] { expectedDiagnostic }, code));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void WithExpectedDiagnosticWhenOneReportsError()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int wrongName;
        
        public int WrongName { get; set; }
    }
}";

                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldAndPropertyMustBeNamedFooAnalyzer.FieldDiagnosticId);
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid<FieldAndPropertyMustBeNamedFooAnalyzer>(expectedDiagnostic, code));
                string expected = "Expected no diagnostics, found:\r\n" +
                                  "Field Message format.\r\n" +
                                  "  at line 5 and character 29 in file Foo.cs | private readonly int ↓wrongName;\r\n";
                Assert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid(typeof(FieldAndPropertyMustBeNamedFooAnalyzer), expectedDiagnostic, code));
                Assert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid(new FieldAndPropertyMustBeNamedFooAnalyzer(), expectedDiagnostic, code));
                Assert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid<FieldAndPropertyMustBeNamedFooAnalyzer>(new[] { expectedDiagnostic }, code));
                Assert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid(typeof(FieldAndPropertyMustBeNamedFooAnalyzer), new[] { expectedDiagnostic }, code));
                Assert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Valid(new FieldAndPropertyMustBeNamedFooAnalyzer(), new[] { expectedDiagnostic }, code));
                Assert.AreEqual(expected, exception.Message);
            }
        }
    }
}