using LeanTest.Dependencies.Configuration;
using LeanTest.Dependencies.Wrappers;
using LeanTest.Dynamic.Invocation;
using LeanTest.Dynamic.ReflectionEmitting;

using System.Reflection.Emit;

namespace LeanTest.Dependencies.Factories;

// TODO DummyWrapperProxy for debug inspection purposes
internal readonly record struct DummyFactory : IDummyFactory
{
	private readonly ModuleBuilder _moduleBuilder;
	private static readonly Dictionary<Type, Type> GeneratedTypes = new();

	public DummyFactory(ModuleBuilder moduleBuilder)
	{
		_moduleBuilder = moduleBuilder;
	}

	TService IDummyFactory.Of<TService>()
		where TService : class
	{
		// TODO validate type isn't sealed? Or test with sealed class and see what happens
		var invocationMarshall = new DummyInvocationMarshall();
		return GenerateStubClass<TService>(invocationMarshall);
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
			.GenerateRuntimeType(serviceType, "Dummy");

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
