using LeanTest.Dependencies.Configuration;
using LeanTest.Dependencies.Wrappers;
using LeanTest.Dynamic.Invocation;

using System.Reflection.Emit;

namespace LeanTest.Dependencies.Factories;

internal sealed class MockFactory : DependencyFactory, IMockFactory
{
	protected override string FieldName => "recordingInvocationMarshall";
	protected override string DependencyName => nameof(Mock<object>);

	public MockFactory(ModuleBuilder moduleBuilder) : base(moduleBuilder) { }

	IMock<TService> IMockFactory.Of<TService>()
		where TService : class
	{
		// TODO validate type isn't sealed? Or test with sealed class and see what happens
		var configuredMethods = new ConfiguredMethodSet();
		var invocationRecordList = new InvocationRecordList();
		var recordingInvocationMarshall = new RecordingInvocationMarshall(configuredMethods, invocationRecordList);
		var instance = GenerateClass<TService>(recordingInvocationMarshall);

		return new Mock<TService>(configuredMethods, invocationRecordList, instance);
	}
}

