using System.Linq.Expressions;

namespace LeanTest.Dependencies.Wrappers;

// TODO MockProxy
internal class Stub<TService> : IStub<TService>
{
	public TService Instance { get; }
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
