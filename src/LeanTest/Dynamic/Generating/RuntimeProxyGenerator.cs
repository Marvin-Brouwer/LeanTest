using LeanTest.Dynamic.Invocation;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using System.Reflection;

namespace LeanTest.Dynamic.Generating;

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

		var compilation = CSharpCompilationPreset.CreateNew(
			serviceType,
			_assemblyContext,
			syntaxTree,
			assemblyReferences
		);

		var result = compilation.Emit(ms, cancellationToken: _cancellationToken);
		if (!result.Success) throw RuntimeProxyGeneratorException.CompilationFaillure(serviceType, result.Diagnostics);

		ms.Seek(0, SeekOrigin.Begin);
#if WRITE_RUNTIME_DLL
		using var fs = File.OpenWrite(_assemblyContext.GetOutputDllPath());
		ms.CopyTo(fs);
		ms.Seek(0, SeekOrigin.Begin);
#endif

		var assembly = _assemblyContext.CreateCleanAssemblyLoadContext().LoadFromStream(ms);
		var generatedType = assembly.GetType($"{_assemblyContext.NamespaceName}.{className}")!;

		_assemblyContext.Add(serviceType, generatedType, syntaxTree, assemblyReferences);

		return generatedType;
	}
}
