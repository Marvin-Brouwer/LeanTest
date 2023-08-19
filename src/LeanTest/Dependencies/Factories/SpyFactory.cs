using LeanTest.Dependencies.Wrappers;
using LeanTest.Dynamic.Invocation;

using System.Reflection.Emit;

namespace LeanTest.Dependencies.Factories;

internal sealed class SpyFactory : DependencyFactory, ISpyFactory
{
	public SpyFactory(ModuleBuilder moduleBuilder) : base(moduleBuilder) { }

	ISpy<TService> ISpyFactory.On<TService>(TService service)
		where TService : class
	{
		// TODO validate type isn't sealed? Or test with sealed class and see what happens
		var invocationRecordList = new InvocationRecordList();
		var invocationRecorder = new InvocationRecorder<TService>(service, invocationRecordList);
		var instance = GenerateClass<TService>(invocationRecorder);

		return new Spy<TService>(invocationRecordList, instance);
	}
}
