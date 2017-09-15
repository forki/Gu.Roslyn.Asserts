﻿namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    public static partial class AnalyzerAssert
    {
        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <typeparamref name="TAnalyzer"/>.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <param name="code">The code with error positions indicated.</param>
        [Obsolete("Renamed to Valid")]
        public static void NoDiagnostics<TAnalyzer>(params string[] code)
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            Valid(new TAnalyzer(), (IReadOnlyList<string>)code);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzerType"/>.
        /// </summary>
        /// <param name="analyzerType">The type of the analyzer.</param>
        /// <param name="code">The code with error positions indicated.</param>
        [Obsolete("Renamed to Valid")]
        public static void NoDiagnostics(Type analyzerType, params string[] code)
        {
            Valid((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType), (IReadOnlyList<string>)code);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="code">The code with error positions indicated.</param>
        [Obsolete("Renamed to Valid")]
        public static void NoDiagnostics(DiagnosticAnalyzer analyzer, params string[] code)
        {
            Valid(analyzer, (IReadOnlyList<string>)code);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="code">The code with error positions indicated.</param>
        [Obsolete("Renamed to Valid")]
        public static void NoDiagnostics(DiagnosticAnalyzer analyzer, IReadOnlyList<string> code)
        {
            Valid(analyzer, code);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="code">The code with error positions indicated.</param>
        /// <param name="metadataReferences">The metadata references to use when compiling.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Obsolete("Renamed to Valid")]
        public static Task NoDiagnosticsAsync(DiagnosticAnalyzer analyzer, IReadOnlyList<string> code, IReadOnlyList<MetadataReference> metadataReferences)
        {
            return ValidAsync(analyzer, code, metadataReferences);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <typeparamref name="TAnalyzer"/>.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <param name="code">The code with error positions indicated.</param>
        [Obsolete("Renamed to Valid")]
        public static void NoDiagnostics<TAnalyzer>(FileInfo code)
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            Valid(new TAnalyzer(), code);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzerType"/>.
        /// </summary>
        /// <param name="analyzerType">The type of the analyzer.</param>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file
        /// </param>
        [Obsolete("Renamed to Valid")]
        public static void NoDiagnostics(Type analyzerType, FileInfo code)
        {
            Valid((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType), code);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file
        /// </param>
        [Obsolete("Renamed to Valid")]
        public static void NoDiagnostics(DiagnosticAnalyzer analyzer, FileInfo code)
        {
            Valid(analyzer, code);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file
        /// </param>
        /// <param name="metadataReferences">The metadata references to use when compiling.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Obsolete("Renamed to Valid")]
        public static Task NoDiagnosticsAsync(DiagnosticAnalyzer analyzer, FileInfo code, IReadOnlyList<MetadataReference> metadataReferences)
        {
            return ValidAsync(analyzer, code, metadataReferences);
        }
    }
}
