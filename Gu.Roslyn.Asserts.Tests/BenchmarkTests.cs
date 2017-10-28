﻿namespace Gu.Roslyn.Asserts.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public class BenchmarkTests
    {
        [Test]
        public async Task Solution()
        {
            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            var sln = CodeFactory.CreateSolution(
                CodeFactory.FindSolutionFile("Gu.Roslyn.Asserts.sln"),
                MetadataReferences.Transitive(typeof(BenchmarkTests).Assembly).ToArray());
            var benchmark = await Benchmark.CreateAsync(sln, analyzer).ConfigureAwait(false);
            CollectionAssert.IsNotEmpty(benchmark.SyntaxNodeActions);
            CollectionAssert.AllItemsAreInstancesOfType(benchmark.SyntaxNodeActions.Select(x => x.Context.Node), typeof(FieldDeclarationSyntax));
            CollectionAssert.AllItemsAreInstancesOfType(benchmark.SyntaxNodeActions.Select(x => x.Context.ContainingSymbol), typeof(IFieldSymbol));
            Assert.AreSame(analyzer, benchmark.Analyzer);
            benchmark.Run();
            benchmark.Run();
        }

        [Test]
        public async Task Project()
        {
            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            var sln = CodeFactory.CreateSolution(
                CodeFactory.FindProjectFile("Gu.Roslyn.Asserts.csproj"),
                MetadataReferences.Transitive(typeof(Benchmark).Assembly).ToArray());
            var benchmark = await Benchmark.CreateAsync(sln.Projects.Single(), analyzer).ConfigureAwait(false);
            CollectionAssert.IsNotEmpty(benchmark.SyntaxNodeActions);
            Assert.AreSame(analyzer, benchmark.Analyzer);
            benchmark.Run();
            benchmark.Run();
        }

        [Test]
        public async Task ClassLibrary1FieldNameMustNotBeginWithUnderscore()
        {
            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            var sln = CodeFactory.CreateSolution(
                CodeFactory.FindProjectFile("ClassLibrary1.csproj"),
                MetadataReferences.Transitive(typeof(Benchmark).Assembly).ToArray());
            var benchmark = await Benchmark.CreateAsync(sln.Projects.Single(), analyzer).ConfigureAwait(false);
            var expected = new[] { "private int _value;" };
            CollectionAssert.AreEqual(expected, benchmark.SyntaxNodeActions.Select(x => x.Context.Node.ToString()));

            expected = new[] { "ClassLibrary1.ClassLibrary1Class1._value" };
            CollectionAssert.AreEqual(expected, benchmark.SyntaxNodeActions.Select(x => x.Context.ContainingSymbol.ToString()));
            Assert.AreSame(analyzer, benchmark.Analyzer);
            benchmark.Run();
            benchmark.Run();
        }

        [Test]
        public async Task ClassLibrary1FieldDeclarations()
        {
            var analyzer = new SyntaxNodeAnalyzer(SyntaxKind.FieldDeclaration);
            var sln = CodeFactory.CreateSolution(
                CodeFactory.FindProjectFile("ClassLibrary1.csproj"),
                MetadataReferences.Transitive(typeof(Benchmark).Assembly).ToArray());
            var benchmark = await Benchmark.CreateAsync(sln.Projects.Single(), analyzer).ConfigureAwait(false);
            var expected = new List<string> { "private int _value;" };
            CollectionAssert.AreEqual(expected, benchmark.SyntaxNodeActions.Select(x => x.Context.Node.ToString()));
            CollectionAssert.IsEmpty(analyzer.Contexts);

            benchmark.Run();
            CollectionAssert.AreEqual(expected, analyzer.Contexts.Select(x => x.Node.ToString()));

            expected.AddRange(expected);
            benchmark.Run();
            CollectionAssert.AreEqual(expected, analyzer.Contexts.Select(x => x.Node.ToString()));
        }
    }
}
