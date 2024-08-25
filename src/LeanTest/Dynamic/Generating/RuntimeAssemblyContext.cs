using Microsoft.CodeAnalysis;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Loader;

namespace LeanTest.Dynamic.Generating;

internal sealed class RuntimeAssemblyContext
{
	private readonly Dictionary<string, Type> _generatedTypes;
	private AssemblyLoadContext? _assemblyLoadContext = null;

	public RuntimeAssemblyContext(Assembly originalAssembly)
	{
		var originalAssemblyName = originalAssembly.GetName()!;

		// This doesn't need to be completely random, this just needs to be random engouh to prevent namespace classes.
		// Based on: https://stackoverflow.com/a/41723783/2319865
		var ticks = new DateTime(2023, 1, 1).Ticks;
		var timeId = DateTime.Now.Ticks - ticks;
		AssemblyName = $"{originalAssemblyName.Name}.RuntimeGenerated_{timeId:x}";
		NamespaceName = AssemblyName;

		_generatedTypes = new();
		GeneratedSyntaxTrees = new List<SyntaxTree>();
		ReferencedAssemblies = new List<MetadataReference>();
	}

	public string AssemblyName { get; }
	public string NamespaceName { get; }

	public AssemblyLoadContext CreateCleanAssemblyLoadContext()
	{
		_assemblyLoadContext?.Unload();
		_assemblyLoadContext = new AssemblyLoadContext(nameof(RuntimeAssemblyContext), true);
		return _assemblyLoadContext;
	}

	public ICollection<SyntaxTree> GeneratedSyntaxTrees { get; }
	public ICollection<MetadataReference> ReferencedAssemblies { get; }

	internal bool TryGetType(string cleanClassName, [NotNullWhen(true)] out Type? generatedProxyType) =>
		_generatedTypes.TryGetValue(cleanClassName, out generatedProxyType);

	internal void Add(string cleanClassName, Type generatedProxyType,
		SyntaxTree syntaxTree, ICollection<MetadataReference> referencedAssemblies)
	{
		_generatedTypes.Add(cleanClassName, generatedProxyType);
		GeneratedSyntaxTrees.Add(syntaxTree);

		foreach (var reference in referencedAssemblies)
		{
			if (ReferencedAssemblies.Contains(reference)) continue;
			ReferencedAssemblies.Add(reference);
		}
	}


#if WRITE_RUNTIME_DLL
	public string GetOutputDllPath()
	{
		var runtimeAssembly = Assembly.GetEntryAssembly()!;
		var runtimeBinFolder = new FileInfo(runtimeAssembly.Location).Directory!;
		var simplifiedAssemblyName = AssemblyName!.Split('_')[0];

		return Path.Join(runtimeBinFolder.FullName, simplifiedAssemblyName + ".g.dll");
	}
#endif

}
