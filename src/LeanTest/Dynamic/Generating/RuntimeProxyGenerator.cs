using LeanTest.Dynamic.Invocation;
using LeanTest.Exceptions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using System.Reflection;

namespace LeanTest.Dynamic.Generating;

// TODO Replace LINQ with Methods.
internal sealed class RuntimeProxyGenerator
{
	private readonly RuntimeAssemblyContext _assemblyContext;
	private readonly CancellationToken _cancellationToken;

	public RuntimeProxyGenerator(RuntimeAssemblyContext assemblyContext, CancellationToken cancellationToken)
	{
		_assemblyContext = assemblyContext;
		_cancellationToken = cancellationToken;
	}

	public Type GenerateProxy<TServiceType>()
	{
		var serviceType = typeof(TServiceType);
		if (_assemblyContext.TryGetType(serviceType, out var generatedProxyType))
			return generatedProxyType;

		using var ms = new MemoryStream();

		var className = serviceType.CleanClassName();
		var generatedProxyClass = ClassBuilder.GenerateProxyClass(_assemblyContext, serviceType, className);

		// Language vesion as low as possible, this is to make sure attributes exist
		var syntaxTree = CSharpSyntaxTree
			.ParseText(generatedProxyClass, new CSharpParseOptions(LanguageVersion.CSharp7),
			cancellationToken: _cancellationToken
		);
		var proxyReferences = new[] {
			typeof(MethodBase).GetTypeInfo().Assembly,
			typeof(IInvokeInterceptor).GetTypeInfo().Assembly,
			serviceType.GetTypeInfo().Assembly,
		};

		// Currently this is not cached, it doesn't seem necessary since the eventual generated assembly is cached
		// by "_assemblyContext.TryGetType"
		// However, if we do experience performance issues this might be a good spot to optimize.
		var originalAssemblyReferences = serviceType.GetTypeInfo().Assembly
			.GetReferencedAssemblies()
			.Select(rn => Assembly.Load(rn.FullName));

		var assemblyReferences = proxyReferences
			.Concat(originalAssemblyReferences)
			.Select(r => MetadataReference.CreateFromFile(r.Location))
			.ToArray<MetadataReference>()!;

		var syntaxTrees = _assemblyContext
			.GeneratedSyntaxTrees
			.Append(syntaxTree);
		var references = _assemblyContext
			.ReferencedAssemblies
			.Concat(assemblyReferences);

		var specificDiagnosticOptions = new Dictionary<string, ReportDiagnostic> {
			// TODO forgot what this is, 
			["TODO sealed?"] = ReportDiagnostic.Hidden
		};
		var compilation = CSharpCompilation.Create(
			_assemblyContext.NamespaceName,
			syntaxTrees: syntaxTrees,
			references: references,
			options: new CSharpCompilationOptions(
				OutputKind.DynamicallyLinkedLibrary,
				false,
				serviceType.Namespace,
				null,
				null,
				null,
#if DEBUG
				OptimizationLevel.Debug,
#else
				OptimizationLevel.Release,
#endif
				checkOverflow: false,
				true,
				null,
				null,
				default,
				null,
				Platform.AnyCpu,
				ReportDiagnostic.Default,
#if DEBUG
				// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-options/errors-warnings#warninglevel
				warningLevel: 4,
#else
				// Set warninglevel to 0, we don't want to see any warnings from generated code.
				warningLevel: 0
#endif
				specificDiagnosticOptions,
				true,
				true,
				null,
				null,
				null,
				null,
				null,
				false,
				MetadataImportOptions.All
			)
		);

		var result = compilation.Emit(ms, cancellationToken: _cancellationToken);
		if (!result.Success)
		{
			var failures = result.Diagnostics.Where(diagnostic =>
				diagnostic.IsWarningAsError ||
				diagnostic.Severity == DiagnosticSeverity.Error
			);

			foreach (Diagnostic diagnostic in failures)
			{
				Console.Error.WriteLine("\t{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
			}

			// TODO better exception
			throw new LeanTestException("TODO better exception");
		}
		else
		{
			ms.Seek(0, SeekOrigin.Begin);
#if WRITE_RUNTIME_DLL
			using var fs = File.OpenWrite(_assemblyContext.GetOutputDllPath());
			ms.CopyTo(fs);
			ms.Seek(0, SeekOrigin.Begin);
#endif

			// If this ever causes performance issues, we can also just generate an assembly per type
			// And maybe just aggregate one #if WRITE_RUNTIME_DLL for ease of access.
			// Or just don't use _assemblyContext.GetOutputDllPath() and create an "{assemblyName}.g.dll" instead
			var assembly = _assemblyContext.CreateCleanAssemblyLoadContext().LoadFromStream(ms);
			var generatedType = assembly.GetType($"{_assemblyContext.NamespaceName}.{className}")!;

			_assemblyContext.Add(serviceType, generatedType, syntaxTree, assemblyReferences);

			return generatedType;
		}
	}
}
