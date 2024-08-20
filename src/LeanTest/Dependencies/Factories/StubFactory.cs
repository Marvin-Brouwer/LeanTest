using LeanTest.Dependencies.Configuration;
using LeanTest.Dynamic.Generating;
using LeanTest.Dynamic.Invocation;

namespace LeanTest.Dependencies.Factories;

internal sealed class StubFactory : IStubFactory
{
	private readonly RuntimeProxyGenerator _proxyGenerator;

	public StubFactory(RuntimeProxyGenerator proxyGenerator)
	{
		_proxyGenerator = proxyGenerator;
	}

	Stub<TService> IStubFactory.Of<TService>()
		where TService : class
	{
		var configuredMethods = new ConfiguredMethodSet();
		var invocationMarshall = new InvocationMarshall(configuredMethods);

		var instance = _proxyGenerator
			.GenerateProxy<TService>()
			.InitializeType<TService>(invocationMarshall);

		return new Stub<TService>(configuredMethods, instance);
	}
}
