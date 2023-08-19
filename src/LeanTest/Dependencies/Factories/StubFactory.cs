using LeanTest.Dependencies.Configuration;
using LeanTest.Dependencies.Wrappers;
using LeanTest.Dynamic.Invocation;

using System.Reflection.Emit;

namespace LeanTest.Dependencies.Factories;

internal sealed class StubFactory : DependencyFactory, IStubFactory
{
	public StubFactory(ModuleBuilder moduleBuilder) : base(moduleBuilder) { }

	IStub<TService> IStubFactory.Of<TService>()
		where TService : class
	{
		// TODO validate type isn't sealed? Or test with sealed class and see what happens
		var configuredMethods = new ConfiguredMethodSet();
		var invocationMarshall = new InvocationMarshall(configuredMethods);
		var instance = GenerateClass<TService>(invocationMarshall);

		return new Stub<TService>(configuredMethods, instance);
	}
}
