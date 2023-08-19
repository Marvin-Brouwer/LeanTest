using LeanTest.Dependencies.Wrappers;
using LeanTest.Dynamic.Invocation;
using LeanTest.Dynamic.ReflectionEmitting;

using System.Reflection.Emit;

namespace LeanTest.Dependencies.Factories;

internal readonly record struct SpyFactory : ISpyFactory
{
	private readonly ModuleBuilder _moduleBuilder;

	public SpyFactory(ModuleBuilder moduleBuilder)
	{
		_moduleBuilder = moduleBuilder;
	}

	ISpy<TService> ISpyFactory.On<TService>(TService service)
		where TService : class
	{
		var invocationRecordList = new InvocationRecordList();
		var invocationRecorder = new InvocationRecorder<TService>(service, invocationRecordList);
		var instance = GenerateSpyClass<TService>(invocationRecorder);

		return new Spy<TService>(invocationRecordList, instance);
	}

	private TService GenerateSpyClass<TService>(IInvokeInterceptor invocationRecorder)
		where TService : class
	{
		var serviceType = typeof(TService);
		var typeBuilder = _moduleBuilder
			.GenerateRuntimeType(serviceType, nameof(Spy<TService>));

		var invocationRecorderField = typeBuilder
			.GeneratePrivateField(nameof(invocationRecorder), typeof(IInvokeInterceptor));

		typeBuilder
			.GenerateConstructor(invocationRecorderField);

		typeBuilder
			.GenerateDynamicImplementationMethods(
				serviceType,
				invocationRecorderField
			);

		// TODO properties

		return typeBuilder
			.Instantiate<TService>(invocationRecorder);
	}
}
