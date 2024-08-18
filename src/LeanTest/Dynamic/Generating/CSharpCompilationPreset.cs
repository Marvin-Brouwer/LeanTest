using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using System.Text;

namespace LeanTest.Dynamic.Generating;
internal static class CSharpCompilationPreset
{
	/// <inheritdoc cref="CompilationOptions.SpecificDiagnosticOptions">
	private static readonly Dictionary<string, ReportDiagnostic> _specificDiagnosticOptions = new Dictionary<string, ReportDiagnostic>
	{
		// TODO forgot what this is, 
		["TODO sealed?"] = ReportDiagnostic.Hidden
	};

	private static CSharpCompilationOptions CreateOptions(string? serviceTypeNamespace) => new (

#region output
		// We generate a dependency, so a DLL
		outputKind: OutputKind.DynamicallyLinkedLibrary,
		// Just include verything and we're fine
		platform: Platform.AnyCpu,
		// Use the passed namespace as a module name
		moduleName: serviceTypeNamespace,
		// Not required for a library
		mainTypeName: null,
		// Not required for a library
		scriptClassName: null,

		// Make it fast
		concurrentBuild: true,
		// Make it consistent
		deterministic: true,
#if DEBUG
		// Keep the optimization to DEBUG when in debug mode for easier debugging
		optimizationLevel: OptimizationLevel.Debug,
#else
		// Optimize the build since it's a dependency anyway.
		optimizationLevel: OptimizationLevel.Release,
#endif
#endregion

#region diagnostics
		// Make it as lenient as possible for the way we generate proxies
		reportSuppressedDiagnostics: false,
		// Make it as lenient as possible for the way we generate proxies
		checkOverflow: false,
		// Make it as lenient as possible for the way we generate proxies
		allowUnsafe: true,
#if DEBUG
		// Keep it on default for DEBUG build so we can inspect better.
		generalDiagnosticOption: ReportDiagnostic.Default,
#else
		// Make it as lenient as possible for the way we generate proxies
		generalDiagnosticOption: ReportDiagnostic.Error,
#endif
		// Make it as lenient as possible for the way we generate proxies, this is about the parsing, not the generating.
		nullableContextOptions: NullableContextOptions.Disable,
#if DEBUG
		// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-options/errors-warnings#warninglevel
		warningLevel: 4,
#else
		// Set warninglevel to 0, we don't want to see any warnings from generated code.
		warningLevel: 0
#endif
		// Make it as lenient as possible for the way we generate proxie
		specificDiagnosticOptions: _specificDiagnosticOptions,
#endregion

#region Customization
		// We don't use any global namespace usings, we write everything we need into the proxy classes.
		usings: null,
		// No XML references are passed
		xmlReferenceResolver: null,
		// No additonal sources are passed
		sourceReferenceResolver: null,
		// No customization is necessary for the passed metadata
		metadataReferenceResolver: null,
		// Since we generate our assembly and the rest is directly passed and only used directly,
		// we don't need any customization here.
		assemblyIdentityComparer: null,
		// Make it as lenient as possible for the way we generate proxies
		metadataImportOptions: MetadataImportOptions.All,
#endregion

		// TODO: Do we need this when supporting signed code?
#region Cryptography
		// We don't sign the output (currently)
		cryptoKeyContainer: null,
		// We don't sign the output (currently)
		cryptoKeyFile: null,
		// We don't sign the output (currently)
		cryptoPublicKey: default,
		// We don't sign the output (currently)
		delaySign: null,
		// We don't sign the output (currently)
		strongNameProvider: null,
		// We don't sign the output (currently)
		publicSign: false
#endregion
	);

	internal static CSharpCompilation CreateNew(
		Type serviceType,
		RuntimeAssemblyContext assemblyContext,
		SyntaxTree generatedSyntaxTree,
		MetadataReference[] generatedAssemblyReferences)
	{
		var mergedSyntaxTrees = assemblyContext
			.GeneratedSyntaxTrees
			.Append(generatedSyntaxTree);
		var mergedReferences = assemblyContext
			.ReferencedAssemblies
			.Concat(generatedAssemblyReferences);

		return CSharpCompilation.Create(
			assemblyContext.NamespaceName,
			syntaxTrees: mergedSyntaxTrees,
			references: mergedReferences,
			options: CreateOptions(serviceType.Namespace)
		);
	}
}
