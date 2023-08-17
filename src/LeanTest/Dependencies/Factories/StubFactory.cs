using LeanTest.Dependencies.Wrappers;
using LeanTest.Dynamic.Invocation;
using LeanTest.Dynamic.ReflectionEmitting;

using System.Reflection.Emit;

namespace LeanTest.Dependencies.Factories;

internal readonly record struct StubFactory : IStubFactory
{
	private readonly ModuleBuilder _moduleBuilder;

	public StubFactory(ModuleBuilder moduleBuilder)
	{
		_moduleBuilder = moduleBuilder;
	}

	IStub<TService> IStubFactory.Of<TService>()
		where TService : class
	{
		// TODO validate type isn't sealed? Or test with sealed class and see what happens
		var configuredMethods = new ConfiguredMethodSet();
		var instance = GenerateStubClass<TService>(configuredMethods);

		return new Stub<TService>(configuredMethods)
		{
			Instance = instance
		};
	}

	private TService GenerateStubClass<TService>(ConfiguredMethodSet configuredMethods)
		where TService : class
	{
		var serviceType = typeof(TService);
		var typeBuilder = _moduleBuilder
			.GenerateRuntimeType(serviceType, nameof(Stub<TService>));

		var configuredMethodsField = typeBuilder
			.GeneratePrivateField(nameof(configuredMethods), typeof(ConfiguredMethodSet));

		typeBuilder
			.GenerateConstructor(configuredMethodsField);
		
		typeBuilder
			.GenerateDynamicImplementationMethods(
				serviceType,
				ReflectionReferenceConstants.InvokeStubGeneric,
				ReflectionReferenceConstants.InvokeStubVoid,
				configuredMethodsField
			);

		// TODO props, fields, etc

		#region TODO this is just for dev
		var generator = new Lokad.ILPack.AssemblyGenerator();
		var bytes = generator.GenerateAssemblyBytes(typeBuilder.CreateType()!.Assembly);
		generator.GenerateAssembly(typeBuilder.CreateType()!.Assembly, "D:\\GEN\\Gen.dll");
		#endregion

		return typeBuilder
			.Instantiate<TService>(configuredMethods);
	}
}
