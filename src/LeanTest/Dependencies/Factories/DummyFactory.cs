using LeanTest.Dynamic.Invocation;

using System.Reflection.Emit;

namespace LeanTest.Dependencies.Factories;

internal sealed class DummyFactory : DependencyFactory, IDummyFactory
{
	public DummyFactory(ModuleBuilder moduleBuilder) :base(moduleBuilder) { }

	TService IDummyFactory.Of<TService>()
		where TService : class
	{
		// TODO validate type isn't sealed? Or test with sealed class and see what happens
		var invocationMarshall = new DummyInvocationMarshall();
		return GenerateClass<TService>(invocationMarshall);
	}
}
