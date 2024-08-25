using LeanTest.Dependencies.Verification;
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
		var invocationMarshall = new InvocationForwarder<TService>(service);
		var invocationRecordList = new InvocationRecordList();
		var recordingInvocationMarshall = new RecordingInvocationMarshall(invocationMarshall, invocationRecordList);

		var instance = _proxyGenerator
			.GenerateProxy<TService>("Spy", service.GetType().GetHashCode())
			.InitializeType<TService>(recordingInvocationMarshall);

		return new Spy<TService>(invocationRecordList, instance);
	}
}
