using LeanTest.Dependencies.Wrappers;

using System.Reflection.Emit;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using LeanTest.Dependencies.Invocation;
using System.Runtime.Intrinsics.Arm;

namespace LeanTest.Dependencies.Factories;

internal readonly record struct StubFactory : IStubFactory
{
	internal static readonly IStubFactory Instance = new StubFactory();

	IStub<TService> IStubFactory.Of<TService>()
		where TService : class
	{
		// TODO validate type isn't sealed
		var configuredMethods = new Dictionary<MethodInfo, object?>();
		var instance = GenerateStubClass<TService>(configuredMethods);

		return new Stub<TService>(configuredMethods)
		{
			Instance = instance
		};
	}

	private TService GenerateStubClass<TService>(IDictionary<MethodInfo, object?> configuredMethods)
		where TService : class
	{
		var serviceType = typeof(TService);
		// TODO Move to Injected
		var ticks = new DateTime(2016, 1, 1).Ticks;
		var timeId = DateTime.Now.Ticks - ticks;
		var assemblyName = $"{serviceType.Assembly.GetName().Name}.RuntimeGenerated_{timeId:x}";
		var (assemblyBuilder, moduleBuilder) = GenerateRuntimeModuleAssembly(serviceType, assemblyName);

		var newTypeName = $"{assemblyName}.RuntimeStub<{serviceType.Name.TrimStart('I')}>";
		var newTypeAttribute = TypeAttributes.Class | TypeAttributes.Public;

		var typeBuilder = moduleBuilder
			.DefineType(newTypeName, newTypeAttribute,
			!serviceType.IsInterface ? serviceType : null,
			serviceType.IsInterface ? new Type[] { serviceType } : null
		);

		var configuredMethodsField = GenerateConfiguredMethodsField(typeBuilder);

		var baseConstructor = typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
		GenerateConstructor(typeBuilder, baseConstructor, configuredMethodsField);

		MethodInfo[] methods = serviceType.GetMethods();
		for (int i = 0; i < methods.Length; i++)
		{
			GenerateMethod(typeBuilder, methods[i], configuredMethodsField);
		}
		// TODO props, fields, etc

		var generatedType = typeBuilder.CreateType()!;
		var wrappedInstance = Activator.CreateInstance(generatedType, configuredMethods)!;
		var castInstance = (TService)wrappedInstance;

		var generator = new Lokad.ILPack.AssemblyGenerator();

		// for ad-hoc serialization
		var bytes = generator.GenerateAssemblyBytes(generatedType.Assembly);

		// direct serialization to disk
		generator.GenerateAssembly(generatedType.Assembly, "D:\\GEN\\Gen.dll");

		return castInstance;
	}

	private FieldBuilder GenerateConfiguredMethodsField(TypeBuilder typeBuilder) => typeBuilder
		.DefineField("_configuredMethods", typeof(IDictionary<MethodInfo, object?>),
			FieldAttributes.Private | FieldAttributes.InitOnly);

	private void GenerateMethod(TypeBuilder typeBuilder, MethodInfo method, FieldBuilder configuredMethodsField)
	{
		var parameters = method.GetParameters();
		var hasParameters = parameters.Length > 0;
		var isVoid = method.ReturnType == typeof(void);

		var attibutes = MethodAttributes.Public
			| MethodAttributes.Final
			| MethodAttributes.HideBySig
			| MethodAttributes.NewSlot
			| MethodAttributes.Virtual;

		var methodBuilder = typeBuilder.DefineMethod(
			method.Name, attibutes, method.ReturnType,
			// TODO remove linq
			parameters.Select(param => param.ParameterType).ToArray()
		);

		var methodIL = methodBuilder.GetILGenerator();

		// TODO Explain
		methodIL.Emit(OpCodes.Ldarg_0);
		methodIL.Emit(OpCodes.Ldfld, configuredMethodsField);
		methodIL.Emit(OpCodes.Call, typeof(MethodBase).GetMethod(nameof(MethodBase.GetCurrentMethod))!);

		if (hasParameters)
		{
			// Create new array of object
			methodIL.Emit(OpCodes.Ldc_I4, parameters.Length);
			methodIL.Emit(OpCodes.Newarr, typeof(object));
			methodIL.Emit(OpCodes.Dup);

			// Add parameters
			for (int i = 0; i < parameters.Length; i++)
			{
				var parameter = parameters[i]!;
				var shouldBox = parameter.ParameterType.IsValueType;
				var parameterNumber = i + 1;

				methodIL.Emit(OpCodes.Ldc_I4, i);
				// TODO check if ldarg s name works
				if (parameterNumber < 4) methodIL.Emit(OpCodes.Ldarg, parameterNumber);
				else methodIL.Emit(OpCodes.Ldarg_S, parameterNumber);

				if (shouldBox)
					methodIL.Emit(OpCodes.Box, parameter.ParameterType);

				methodIL.Emit(OpCodes.Stelem_Ref);

				if (i < parameters.Length - 1)
					methodIL.Emit(OpCodes.Dup);
			}
		}
		else
		{
			// TODO cache
			var arrayEmpty = typeof(Array)
				.GetMethod(nameof(Array.Empty))!
				.GetGenericMethodDefinition()!
				.MakeGenericMethod(new[] { typeof(object) })!;
			methodIL.Emit(OpCodes.Call, arrayEmpty);
		}

		// Making a type generic of TReturn is easier than boxing if nexessary
		// TODO remove linq
		// TODO cache base
		var invokeMethod = typeof(InvocationMarshall)
			.GetMethods()
			.First(method => method.Name == nameof(InvocationMarshall.InvokeStub)
				&& method.IsGenericMethod == !isVoid
			)!;
		if (!isVoid)
			invokeMethod = invokeMethod!
				.GetGenericMethodDefinition()!
				.MakeGenericMethod(new[] { method.ReturnType })!;
		methodIL.Emit(OpCodes.Call, invokeMethod);

		if (isVoid)
			methodIL.Emit(OpCodes.Nop);
		methodIL.Emit(OpCodes.Ret);
	}

	private static void GenerateConstructor(TypeBuilder typeBuilder, ConstructorBuilder baseConstructor, FieldBuilder configuredMethodsField)
	{
		var wrapperCtor = typeBuilder.DefineConstructor(
			MethodAttributes.Public,
			CallingConventions.Standard,
			parameterTypes: new Type[] { typeof(IDictionary<MethodInfo, object?>) }
		);

		var ctorIL = wrapperCtor.GetILGenerator();

		//IL_0000: ldarg.0
		//IL_0001: call instance void [System.Runtime]System.Object::.ctor()
		//IL_0006: nop
		//IL_0007: nop
		//IL_0008: ldarg.0
		//IL_0009: ldarg.1
		//IL_000a: stfld class [System.Runtime]System.Collections.Generic.IDictionary`2<string, object> MARVIN::_configuredMethods
		//IL_000f: ret

		ctorIL.Emit(OpCodes.Ldarg_0);
		ctorIL.Emit(OpCodes.Call, baseConstructor);
		// TODO, does it need NOP?
		ctorIL.Emit(OpCodes.Nop);
		ctorIL.Emit(OpCodes.Nop);
		ctorIL.Emit(OpCodes.Ldarg_0);
		ctorIL.Emit(OpCodes.Ldarg_1);
		ctorIL.Emit(OpCodes.Stfld, configuredMethodsField);
		ctorIL.Emit(OpCodes.Ret);
	}

	private static (AssemblyBuilder, ModuleBuilder) GenerateRuntimeModuleAssembly(Type serviceType, string assemblyName)
	{
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

		var assemblyBuilder = AssemblyBuilder
			// TODO see if Run is sufficient
			.DefineDynamicAssembly(concreteAssemblyName, AssemblyBuilderAccess.RunAndCollect);
		var moduleBuilder = assemblyBuilder
			.DefineDynamicModule($"{assemblyName}.{serviceType.Name.TrimStart('I')}Module");

		return (assemblyBuilder, moduleBuilder);
	}
}
