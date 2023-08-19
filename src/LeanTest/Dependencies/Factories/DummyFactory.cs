using LeanTest.Dynamic.Invocation;

using System.Reflection.Emit;

namespace LeanTest.Dependencies.Factories;

// TODO DummyWrapperProxy for debug inspection purposes
internal sealed class DummyFactory : DependencyFactory, IDummyFactory
{
	protected override string FieldName => "dummyInterceptor";
	protected override string DependencyName => "Dummy";

	public DummyFactory(ModuleBuilder moduleBuilder) :base(moduleBuilder) { }

	TService IDummyFactory.Of<TService>()
		where TService : class
	{
		// TODO validate type isn't sealed? Or test with sealed class and see what happens
		var invocationMarshall = new DummyInvocationMarshall();
		return GenerateClass<TService>(invocationMarshall);
	}
}
