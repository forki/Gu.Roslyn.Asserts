﻿namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts.Internals;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Formatting;

    /// <summary>
    /// The AnalyzerAssert class contains a collection of static methods used for assertions on the behavior of analyzers and code fixes.
    /// </summary>
    public static partial class AnalyzerAssert
    {
        /// <summary>
        /// The metadata references used when creating the projects created in the tests.
        /// </summary>
        public static readonly List<MetadataReference> MetadataReferences = Asserts.MetadataReferences.FromAttributes().ToList();

        /// <summary>
        /// The metadata references used when creating the projects created in the tests.
        /// </summary>
        public static readonly List<string> SuppressedDiagnostics = DiagnosticSettings.AllowedErrorIds().ToList();

        /// <summary>
        /// Add <paramref name="assembly"/> and all assemblies referenced by it.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/></param>
        public static void AddTransitiveMetadataReferences(Assembly assembly)
        {
            MetadataReferences.AddRange(Asserts.MetadataReferences.Transitive(assembly));
        }

        /// <summary>
        /// Resets <see cref="MetadataReferences"/> to <see cref="Asserts.MetadataReferences.FromAttributes"/>
        /// </summary>
        public static void ResetMetadataReferences()
        {
            MetadataReferences.Clear();
            MetadataReferences.AddRange(Asserts.MetadataReferences.FromAttributes());
        }

        /// <summary>
        /// Resets <see cref="SuppressedDiagnostics"/> to <see cref="DiagnosticSettings.AllowedErrorIds()"/>
        /// </summary>
        public static void ResetSuppressedDiagnostics()
        {
            SuppressedDiagnostics.Clear();
            SuppressedDiagnostics.AddRange(DiagnosticSettings.AllowedErrorIds());
        }

        /// <summary>
        /// Resets <see cref="SuppressedDiagnostics"/> and <see cref="MetadataReferences"/>
        /// </summary>
        public static void ResetAll()
        {
            ResetMetadataReferences();
            ResetSuppressedDiagnostics();
        }

        private static void AssertAnalyzerSupportsExpectedDiagnostic(DiagnosticAnalyzer analyzer, ExpectedDiagnostic expectedDiagnostic, out DiagnosticDescriptor descriptor, out IReadOnlyList<string> suppressedDiagnostics)
        {
            var descriptors = analyzer.SupportedDiagnostics.Where(x => x.Id == expectedDiagnostic.Id).ToArray();
            if (descriptors.Length == 0)
            {
                var message = $"Analyzer {analyzer} does not produce a diagnostic with ID {expectedDiagnostic.Id}.{Environment.NewLine}" +
                              $"The analyzer produces the following diagnostics: {{{string.Join(", ", analyzer.SupportedDiagnostics.Select(d => d.Id))}}}{Environment.NewLine}" +
                              $"The expected diagnostic is: {expectedDiagnostic.Id}";
                throw AssertException.Create(message);
            }

            if (descriptors.Length > 1)
            {
                var message = $"Analyzer {analyzer} supports multiple diagnostics with ID {expectedDiagnostic.Id}.{Environment.NewLine}" +
                              $"The analyzer produces the following diagnostics: {{{string.Join(", ", analyzer.SupportedDiagnostics.Select(d => d.Id))}}}{Environment.NewLine}" +
                              $"The expected diagnostic is: {expectedDiagnostic.Id}";
                throw AssertException.Create(message);
            }

            suppressedDiagnostics = analyzer.SupportedDiagnostics.Select(x => x.Id).Where(x => x != expectedDiagnostic.Id).ToArray();
            descriptor = descriptors[0];
        }

        private static void AssertAnalyzerSupportsExpectedDiagnostics(DiagnosticAnalyzer analyzer, IReadOnlyList<ExpectedDiagnostic> expectedDiagnostics, out IReadOnlyList<DiagnosticDescriptor> descriptors, out IReadOnlyList<string> suppressed)
        {
            var tempDescriptors = new List<DiagnosticDescriptor>();
            var tempSuppressed = new List<string>();
            foreach (var expectedDiagnostic in expectedDiagnostics)
            {
                AssertAnalyzerSupportsExpectedDiagnostic(analyzer, expectedDiagnostic, out var descriptor, out var suppressedDiagnostics);
                tempDescriptors.Add(descriptor);
                tempSuppressed.AddRange(suppressedDiagnostics);
            }

            descriptors = tempDescriptors;
            suppressed = tempSuppressed.Distinct().Except(descriptors.Select(x => x.Id)).ToArray();
        }

        private static void AssertCodeFixCanFixDiagnosticsFromAnalyzer(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix)
        {
            if (!analyzer.SupportedDiagnostics.Select(d => d.Id).Intersect(codeFix.FixableDiagnosticIds).Any())
            {
                var message = $"Analyzer {analyzer} does not produce diagnostics fixable by {codeFix}.{Environment.NewLine}" +
                              $"The analyzer produces the following diagnostics: {{{string.Join(", ", analyzer.SupportedDiagnostics.Select(d => d.Id))}}}{Environment.NewLine}" +
                              $"The code fix supports the following diagnostics: {{{string.Join(", ", codeFix.FixableDiagnosticIds)}}}";
                throw AssertException.Create(message);
            }
        }

        private static async Task AreEqualAsync(IReadOnlyList<string> expected, Solution actual, string messageHeader)
        {
            var index = 0;
            foreach (var project in actual.Projects)
            {
                for (var i = 0; i < project.DocumentIds.Count; i++)
                {
                    var fixedSource = await CodeReader.GetStringFromDocumentAsync(project.GetDocument(project.DocumentIds[i]), Formatter.Annotation, CancellationToken.None).ConfigureAwait(false);
                    CodeAssert.AreEqual(expected[index], fixedSource, messageHeader);
                    index++;
                }
            }
        }

        private static async Task AssertNoCompilerErrorsAsync(CodeFixProvider codeFix, Solution fixedSolution)
        {
            var diagnostics = await Analyze.GetDiagnosticsAsync(fixedSolution).ConfigureAwait(false);
            var introducedDiagnostics = diagnostics
                .SelectMany(x => x)
                .Where(IsIncluded)
                .ToArray();
            if (introducedDiagnostics.Select(x => x.Id)
                                     .Except(DiagnosticSettings.AllowedErrorIds())
                                     .Any())
            {
                var errorBuilder = StringBuilderPool.Borrow();
                errorBuilder.AppendLine($"{codeFix} introduced syntax error{(introducedDiagnostics.Length > 1 ? "s" : string.Empty)}.");
                foreach (var introducedDiagnostic in introducedDiagnostics)
                {
                    var errorInfo = await introducedDiagnostic.ToStringAsync(fixedSolution).ConfigureAwait(false);
                    errorBuilder.AppendLine($"{errorInfo}");
                }

                errorBuilder.AppendLine("First source file with error is:");
                var sources = await Task.WhenAll(fixedSolution.Projects.SelectMany(p => p.Documents).Select(d => CodeReader.GetStringFromDocumentAsync(d, Formatter.Annotation, CancellationToken.None)));
                var lineSpan = introducedDiagnostics.First().Location.GetMappedLineSpan();
                var match = sources.SingleOrDefault(x => CodeReader.FileName(x) == lineSpan.Path);
                errorBuilder.Append(match);
                errorBuilder.AppendLine();
                throw AssertException.Create(errorBuilder.Return());
            }
        }

        private static bool IsIncluded(Diagnostic diagnostic)
        {
            return IsIncluded(diagnostic, DiagnosticSettings.AllowedDiagnostics());
        }

        private static bool IsIncluded(Diagnostic diagnostic, AllowedDiagnostics allowedDiagnostics)
        {
            switch (allowedDiagnostics)
            {
                case AllowedDiagnostics.Warnings:
                    return diagnostic.Severity == DiagnosticSeverity.Error;
                case AllowedDiagnostics.None:
                    return diagnostic.Severity == DiagnosticSeverity.Error || diagnostic.Severity == DiagnosticSeverity.Warning;
                case AllowedDiagnostics.WarningsAndErrors:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}