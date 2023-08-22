using LeanTest.Dependencies.Verification;
using LeanTest.Dependencies.Wrappers;
using LeanTest.Dynamic.Generating;
using LeanTest.Dynamic.Invocation;

namespace LeanTest.Dependencies.Factories;

internal sealed class SpyFactory : ISpyFactory
{
	private readonly RuntimeProxyGenerator _proxyGenerator;

	public SpyFactory(RuntimeProxyGenerator proxyGenerator)
	{
		_proxyGenerator = proxyGenerator;
	}

	Spy<TService> ISpyFactory.On<TService>(TService service)
		where TService : class
	{
		// TODO validate type isn't sealed? Or test with sealed class and see what happens

		var invocationRecordList = new InvocationRecordList();
		var invocationRecorder = new InvocationRecorder<TService>(service, invocationRecordList);

		var instance = _proxyGenerator
			.GenerateProxy<TService>()
			.InitializeType<TService>(invocationRecorder);

		return new Spy<TService>(invocationRecordList, instance);
	}
}
