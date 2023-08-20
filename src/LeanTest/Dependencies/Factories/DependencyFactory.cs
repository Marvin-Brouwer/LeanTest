using LeanTest.Dynamic;
using LeanTest.Dynamic.Invocation;
using LeanTest.Dynamic.ReflectionEmitting;

using System.Reflection.Emit;

namespace LeanTest.Dependencies.Factories;

internal abstract class DependencyFactory
{
	private const string InterceptorFieldName = "interceptor";

	private readonly ModuleBuilder _moduleBuilder;
	private static readonly Dictionary<Type, Type> GeneratedTypes = new();


	protected DependencyFactory(ModuleBuilder moduleBuilder)
	{
		_moduleBuilder = moduleBuilder;
	}

	protected TService GenerateClass<TService>(IInvokeInterceptor interceptor)
		where TService : class
	{
		var serviceType = typeof(TService);
		if (GeneratedTypes.TryGetValue(serviceType, out var type))
		{
			return type
				.InitializeType<TService>(interceptor);
		}
		RuntimeProxyGenerator.Create(typeof(TService));

		var typeBuilder = _moduleBuilder
			.GenerateRuntimeType(serviceType);

		var interceptorField = typeBuilder
			.GeneratePrivateField(InterceptorFieldName, typeof(IInvokeInterceptor));

		typeBuilder
			.GenerateConstructor(interceptorField);

		typeBuilder
			.GenerateDynamicImplementationMethods(serviceType, interceptorField);

		// TODO properties

		return typeBuilder
			.GenerateType(serviceType, GeneratedTypes)
			.InitializeType<TService>(interceptor);
	}
}
