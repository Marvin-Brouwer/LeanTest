using LeanTest.Dynamic.Generating;
using LeanTest.Dynamic.Invocation;

namespace LeanTest.Dependencies.Factories;

internal sealed class DummyFactory : IDummyFactory
{
	private readonly RuntimeProxyGenerator _proxyGenerator;
	private readonly DummyInvocationMarshall _invocationMarshall;

	public DummyFactory(RuntimeProxyGenerator proxyGenerator)
	{
		_proxyGenerator = proxyGenerator;
		_invocationMarshall = new DummyInvocationMarshall();
	}

	TService IDummyFactory.Of<TService>()
		where TService : class => _proxyGenerator
			.GenerateProxy<TService>("Dummy")
			.InitializeType<TService>(_invocationMarshall);
}
