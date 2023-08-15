using System.Reflection.Emit;
using System.Reflection;

namespace LeanTest.Dynamic.ReflectionEmitting;

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

		var cleanServiceName = serviceType.Name
			.TrimStart('I')
			// TODO only trim Base
			.Replace("Base", string.Empty, StringComparison.OrdinalIgnoreCase);

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
