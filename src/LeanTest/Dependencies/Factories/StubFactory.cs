using LeanTest.Dependencies.Configuration;
using LeanTest.Dependencies.Wrappers;
using LeanTest.Dynamic.Invocation;
using LeanTest.Dynamic.ReflectionEmitting;

using System.Reflection.Emit;

namespace LeanTest.Dependencies.Factories;

internal readonly record struct StubFactory : IStubFactory
{
	private readonly ModuleBuilder _moduleBuilder;
	private static readonly Dictionary<Type, Type> GeneratedTypes = new();

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

		return new Stub<TService>(configuredMethods, instance);
	}

	private TService GenerateStubClass<TService>(IInvokeInterceptor invocationMarshall)
		where TService : class
	{
		var serviceType = typeof(TService);
		if (GeneratedTypes.TryGetValue(serviceType, out var type))
		{
			return serviceType
				.InitializeType<TService>(invocationMarshall);
		}

		var typeBuilder = _moduleBuilder
			.GenerateRuntimeType(serviceType, nameof(Stub<TService>));

		var invocationMarshallField = typeBuilder
			.GeneratePrivateField(nameof(invocationMarshall), typeof(IInvokeInterceptor));

		typeBuilder
			.GenerateConstructor(invocationMarshallField);
		
		typeBuilder
			.GenerateDynamicImplementationMethods(
				serviceType,
				invocationMarshallField
			);

		// TODO properties

		return typeBuilder
			.Instantiate<TService>(invocationMarshall);
	}
}
