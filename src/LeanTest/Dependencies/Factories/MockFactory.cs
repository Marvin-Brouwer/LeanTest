using LeanTest.Dependencies.Configuration;
using LeanTest.Dependencies.Wrappers;
using LeanTest.Dynamic.Invocation;
using LeanTest.Dynamic.ReflectionEmitting;

using System.Reflection.Emit;

namespace LeanTest.Dependencies.Factories;

internal readonly record struct MockFactory : IMockFactory
{
	private readonly ModuleBuilder _moduleBuilder;
	private static readonly Dictionary<Type, Type> GeneratedTypes = new();

	public MockFactory(ModuleBuilder moduleBuilder)
	{
		_moduleBuilder = moduleBuilder;
	}

	IMock<TService> IMockFactory.Of<TService>()
		where TService : class
	{
		// TODO validate type isn't sealed? Or test with sealed class and see what happens
		var configuredMethods = new ConfiguredMethodSet();
		var invocationRecordList = new InvocationRecordList();
		var recordingInvocationMarshall = new RecordingInvocationMarshall(configuredMethods, invocationRecordList);
		var instance = GenerateMockClass<TService>(recordingInvocationMarshall);

		return new Mock<TService>(configuredMethods, invocationRecordList, instance);
	}

	private TService GenerateMockClass<TService>(IInvokeInterceptor recordingInvocationMarshall)
		where TService : class
	{
		var serviceType = typeof(TService);
		if (GeneratedTypes.TryGetValue(serviceType, out var type))
		{
			return serviceType
				.InitializeType<TService>(recordingInvocationMarshall);
		}

		var typeBuilder = _moduleBuilder
			.GenerateRuntimeType(serviceType, nameof(Mock<TService>));

		var invocationMarshallField = typeBuilder
			.GeneratePrivateField(nameof(recordingInvocationMarshall), typeof(IInvokeInterceptor));

		typeBuilder
			.GenerateConstructor(invocationMarshallField);

		typeBuilder
			.GenerateDynamicImplementationMethods(
				serviceType,
				invocationMarshallField
			);

		// TODO properties

		return typeBuilder
			.Instantiate<TService>(recordingInvocationMarshall);
	}
}

