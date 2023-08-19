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

		// This doesn't need to be completely random, this just needs to be random engouh to prevent namespace classes.
		// Based on: https://stackoverflow.com/a/41723783/2319865
		var ticks = new DateTime(2016, 1, 1).Ticks;
		var timeId = DateTime.Now.Ticks - ticks;
		var assemblyName = $"{serviceType.Assembly.GetName().Name}.RuntimeGenerated<>{timeId:x}";

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
		string cleanServiceName = serviceType.Name.CleanClassName();

		return AssemblyBuilder
			// TODO see if Run is sufficient
			.DefineDynamicAssembly(concreteAssemblyName, AssemblyBuilderAccess.RunAndCollect)
			.DefineDynamicModule($"{assemblyName}.{cleanServiceName}Module");
	}

	internal static string CleanClassName(this string serviceTypeName)
	{
		const string BaseFix = "Base";
		const int BaseFixLength = 4;

		var cleanServiceName = serviceTypeName.TrimStart('I');
		if (serviceTypeName.StartsWith(BaseFix, StringComparison.Ordinal))
			cleanServiceName = cleanServiceName.Substring(BaseFixLength);
		else if (serviceTypeName.EndsWith(BaseFix, StringComparison.Ordinal))
			cleanServiceName = cleanServiceName.Substring(0, cleanServiceName.Length - BaseFixLength);

		return cleanServiceName;
	}

	public static Type GenerateType(this TypeBuilder typeBuilder, Type serviceType, Dictionary<Type, Type> generatedTypes)
	{
		var generatedType = typeBuilder.CreateType()!;

#if WRITE_RUNTIME_DLL
		var runtimeAssembly = Assembly.GetEntryAssembly()!;
		var runtimeBinFolder = new FileInfo(runtimeAssembly.Location).Directory!;
		_ = runtimeAssembly;


		var generator = new Lokad.ILPack.AssemblyGenerator();
		// Validate assembly
		_ = generator.GenerateAssemblyBytes(generatedType.Assembly);
		// Write assembly
		var simplifiedAssemblyName = generatedType.Assembly!.GetName()!.Name!.Split('<')[0];
		generator.GenerateAssembly(
			generatedType.Assembly,
			Assembly.GetExecutingAssembly().GetLoadedModules().Select(m => m.Assembly),
			System.IO.Path.Join(runtimeBinFolder.FullName, simplifiedAssemblyName + ".dll")
		);
#endif
		generatedTypes.Add(serviceType, generatedType);
		return generatedType;
	}

	public static TService InitializeType<TService>(this Type generatedType, params object[] constructorParameters)
		where TService : class
	{

#if DEBUG
		// #1: Figure out why IL output breaks for constructor when adding parameters
		var wrappedInstance = Activator.CreateInstance(generatedType)!;
		var runtimeFields = generatedType.GetRuntimeFields().ToArray();
		for (var i = 0; i < constructorParameters.Length; i++)
			runtimeFields[i].SetValue(wrappedInstance, constructorParameters[i]);
#else
		var wrappedInstance = Activator.CreateInstance(generatedType, constructorParameters)!;
#endif
		return (TService)wrappedInstance;
	}
}
