using System.Reflection.Emit;
using System.Reflection;

namespace LeanTest.Dynamic.ReflectionEmitting;

/// <summary>
/// Sulution based on:
/// <see href="https://sharplab.io/#gist:2b2438e6d192a5b70b373565b8201207"/>
/// </summary>
internal static class AssemblyEmitExtensions
{
	public static ModuleBuilder GenerateRuntimeModuleAssembly(this Type serviceType)
	{
		// TODO package release date
		// TODO ref stackoverflow

		// This doesn't need to be completely random, this just needs to be random engouh to prevent namespace classes.
		var ticks = new DateTime(2016, 1, 1).Ticks;
		var timeId = DateTime.Now.Ticks - ticks;
		var assemblyName = $"{serviceType.Assembly.GetName().Name}.RuntimeGenerated_{timeId:x}";

		var originalAssembly = serviceType.Assembly;
		var originalAssemblyName = originalAssembly.GetName();
		var concreteAssemblyName = new AssemblyName
		{
			Name = assemblyName,
			ContentType = AssemblyContentType.Default,
			CultureInfo = originalAssemblyName.CultureInfo,
			CultureName = originalAssemblyName.CultureName,
			Flags = AssemblyNameFlags.Retargetable,
			ProcessorArchitecture = originalAssemblyName.ProcessorArchitecture,
			Version = originalAssemblyName.Version,
			VersionCompatibility = System.Configuration.Assemblies.AssemblyVersionCompatibility.SameMachine
		};

		const string BaseFix = "Base";
		const int BaseFixLength = 4;

		var cleanServiceName = serviceType.Name.TrimStart('I');
		if (serviceType.Name.StartsWith(BaseFix, StringComparison.Ordinal))
			cleanServiceName = cleanServiceName.Substring(BaseFixLength);
		else if (serviceType.Name.EndsWith(BaseFix, StringComparison.Ordinal))
			cleanServiceName = cleanServiceName.Substring(0, cleanServiceName.Length - BaseFixLength);

		return AssemblyBuilder
			// TODO see if Run is sufficient
			.DefineDynamicAssembly(concreteAssemblyName, AssemblyBuilderAccess.RunAndCollect)
			.DefineDynamicModule($"{assemblyName}.{cleanServiceName}Module");
	}

	public static TService Instantiate<TService>(this TypeBuilder typeBuilder, params object[] constructorParameters)
		where TService : class
	{
		var generatedType = typeBuilder.CreateType()!;
		var wrappedInstance = Activator.CreateInstance(generatedType, constructorParameters)!;
		return (TService)wrappedInstance;
	}
}
