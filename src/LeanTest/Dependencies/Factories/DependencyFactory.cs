using LeanTest.Dynamic.Invocation;
using LeanTest.Dynamic.ReflectionEmitting;

using System.Reflection.Emit;

namespace LeanTest.Dependencies.Factories;

// TODO DummyWrapperProxy for debug inspection purposes
internal abstract class DependencyFactory
{
	private readonly ModuleBuilder _moduleBuilder;
	private static readonly Dictionary<string, Type> GeneratedTypes = new();

	protected abstract string FieldName { get; }
	protected abstract string DependencyName { get; }

	protected DependencyFactory(ModuleBuilder moduleBuilder)
	{
		_moduleBuilder = moduleBuilder;
	}

	protected TService GenerateClass<TService>(IInvokeInterceptor interceptor)
		where TService : class
	{
		var serviceType = typeof(TService);
		var serviceTypeKey = $"{serviceType.FullName}_{DependencyName}";
		if (GeneratedTypes.TryGetValue(serviceTypeKey, out var type))
		{
			return type
				.InitializeType<TService>(interceptor);
		}

		var typeBuilder = _moduleBuilder
			.GenerateRuntimeType(serviceType, DependencyName);

		var interceptorField = typeBuilder
			.GeneratePrivateField(FieldName, typeof(IInvokeInterceptor));

		typeBuilder
			.GenerateConstructor(interceptorField);

		typeBuilder
			.GenerateDynamicImplementationMethods(serviceType, interceptorField);

		// TODO properties

		return typeBuilder
			.GenerateType(serviceTypeKey, GeneratedTypes)
			.InitializeType<TService>(interceptor);
	}
}
