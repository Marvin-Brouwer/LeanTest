using LeanTest.Dynamic.Invocation;

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
		// TODO cache?
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

		var compilation = CSharpCompilation.Create(
			_assemblyContext.NamespaceName,
			syntaxTrees: syntaxTrees,
			references: references,
			options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
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
			throw new Exception("TODO better exception");
		}
		else
		{
#if WRITE_RUNTIME_DLL
			using var fs = File.OpenWrite(_assemblyContext.GetOutputDllPath());
			ms.Seek(0, SeekOrigin.Begin);
			ms.CopyTo(fs);
#endif
			ms.Seek(0, SeekOrigin.Begin);

			// If this ever causes performance issues, we can also just generate an assembly per type
			// And maybe just aggregate one #if WRITE_RUNTIME_DLL for ease of access.
			var assembly = _assemblyContext.CreateCleanAssemblyLoadContext().LoadFromStream(ms);
			var generatedType = assembly.GetType($"{_assemblyContext.NamespaceName}.{className}")!;

			_assemblyContext.Add(serviceType, generatedType, syntaxTree, assemblyReferences);

			return generatedType;
		}
	}
}
