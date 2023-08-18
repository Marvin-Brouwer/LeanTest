using LeanTest.Dependencies.Configuration;
using LeanTest.Dynamic.Invocation;
using LeanTest.Dynamic.ReflectionEmitting;

using System.Reflection;
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
		var invocationMarshall = new InvocationMarshall(configuredMethods);
		var instance = GenerateStubClass<TService>(invocationMarshall);

		return new Stub<TService>(configuredMethods)
		{
			Instance = instance
		};
	}

	private TService GenerateStubClass<TService>(IInvocationMarshall invocationMarshall)
		where TService : class
	{
		var serviceType = typeof(TService);
		var typeBuilder = _moduleBuilder
			.GenerateRuntimeType(serviceType, nameof(Stub<TService>));

		var invocationMarshallField = typeBuilder
			.GeneratePrivateField(nameof(invocationMarshall), typeof(IInvocationMarshall));

		typeBuilder
			.GenerateConstructor(invocationMarshallField);
		
		typeBuilder
			.GenerateDynamicImplementationMethods(
				serviceType,
				invocationMarshallField
			);

		// TODO properties

		#region TODO this is just for dev
		var generator = new Lokad.ILPack.AssemblyGenerator();
		var bytes = generator.GenerateAssemblyBytes(typeBuilder.CreateType()!.Assembly);
		generator.GenerateAssembly(typeBuilder.CreateType()!.Assembly, Assembly.GetExecutingAssembly().GetLoadedModules().Select(m => m.Assembly), "D:\\GEN\\Gen.dll");
		#endregion

		return typeBuilder
			.Instantiate<TService>(invocationMarshall);
	}
}
