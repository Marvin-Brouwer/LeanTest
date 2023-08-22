using LeanTest.Dependencies.Configuration;
using LeanTest.Dependencies.Verification;
using LeanTest.Dependencies.Wrappers;
using LeanTest.Dynamic.Generating;
using LeanTest.Dynamic.Invocation;

namespace LeanTest.Dependencies.Factories;

internal sealed class MockFactory : IMockFactory
{
	private readonly RuntimeProxyGenerator _proxyGenerator;

	public MockFactory(RuntimeProxyGenerator proxyGenerator)
	{
		_proxyGenerator = proxyGenerator;
	}

	Mock<TService> IMockFactory.Of<TService>()
		where TService : class
	{
		// TODO validate type isn't sealed? Or test with sealed class and see what happens

		var configuredMethods = new ConfiguredMethodSet();
		var invocationRecordList = new InvocationRecordList();
		var recordingInvocationMarshall = new RecordingInvocationMarshall(configuredMethods, invocationRecordList);

		var instance = _proxyGenerator
			.GenerateProxy<TService>()
			.InitializeType<TService>(recordingInvocationMarshall);

		return new Mock<TService>(configuredMethods, invocationRecordList, instance);
	}
}

