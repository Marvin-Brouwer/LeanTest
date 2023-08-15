using System.Linq.Expressions;
using System.Reflection;

namespace LeanTest.Dependencies.Wrappers;

internal class Stub<TService> : IStub<TService>
{
	private readonly IDictionary<MethodInfo, object?> _configuredMethods;

	public Stub(IDictionary<MethodInfo, object?> configuredMethods)
	{
		_configuredMethods = configuredMethods;
	}

	public TService Instance { get; internal init; } = default!;

	public IStub<TService> Setup<TReturn>(Expression<Func<TService, TReturn>> member, Func<TReturn> returnValue)
	{
		throw new NotImplementedException();
	}

	public IStub<TService> Setup<TReturn>(Expression<Func<TService, Task<TReturn>>> member, Func<TReturn> returnValue)
	{
		throw new NotImplementedException();
	}

	public IStub<TService> Setup(Expression<Func<TService>> member)
	{
		throw new NotImplementedException();
	}

	public IStub<TService> Setup(Expression<Func<TService, Task>> member)
	{
		throw new NotImplementedException();
	}
}
