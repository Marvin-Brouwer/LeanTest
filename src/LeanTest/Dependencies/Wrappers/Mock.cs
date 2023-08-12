using LeanTest.Dependencies.Definitions;
using LeanTest.Dependencies.Factories;

using System.Linq.Expressions;

namespace LeanTest.Dependencies.Wrappers;

// TODO MockProxy
internal class Mock<TService> : IMock<TService>
{
	private readonly ISpy<TService> _spy;
	private readonly IStub<TService> _stub;

	public TService Instance { get; }

	public IMock<TService> Setup(Expression<Func<TService>> member)
	{
		_stub.Setup(member);
		return this;
	}

	public IMock<TService> Setup(Expression<Func<TService, Task>> member)
	{
		_stub.Setup(member);
		return this;
	}

	public IMock<TService> Setup<TReturn>(Expression<Func<TService, TReturn>> member, Func<TReturn> returnValue)
	{
		_stub.Setup(member, returnValue);
		return this;
	}

	public IMock<TService> Setup<TReturn>(Expression<Func<TService, Task<TReturn>>> member, Func<TReturn> returnValue)
	{
		_stub.Setup(member, returnValue);
		return this;
	}

	public IMock<TService> Verify(Expression<Func<TService>> member, ITimesConstraint times)
	{
		_spy.Verify(member, times);
		return this;
	}

	public IMock<TService> Verify<TValue>(Expression<Func<TService, TValue>> member, ITimesConstraint times)
	{
		_spy.Verify(member, times);
		return this;
	}

	public IMock<TService> Verify(Expression<Func<TService, Task>> member, ITimesConstraint times)
	{
		_spy.Verify(member, times);
		return this;
	}

	public IMock<TService> Verify<TValue>(Expression<Func<TService, Task<TValue>>> member, ITimesConstraint times)
	{
		_spy.Verify(member, times);
		return this;
	}

	public IMock<TService> VerifyOnce(Expression<Func<TService>> member) =>
		Verify(member, TimesFactory.Instance.Once);

	public IMock<TService> VerifyOnce<TValue>(Expression<Func<TService, TValue>> member) =>
		Verify(member, TimesFactory.Instance.Once);

	public IMock<TService> VerifyOnce(Expression<Func<TService, Task>> member) =>
		Verify(member, TimesFactory.Instance.Once);

	public IMock<TService> VerifyOnce<TValue>(Expression<Func<TService, Task<TValue>>> member) =>
		Verify(member, TimesFactory.Instance.Once);

	public IMock<TService> VerifyNever(Expression<Func<TService>> member) =>
		Verify(member, TimesFactory.Instance.Never);

	public IMock<TService> VerifyNever<TValue>(Expression<Func<TService, TValue>> member) =>
		Verify(member, TimesFactory.Instance.Never);

	public IMock<TService> VerifyNever(Expression<Func<TService, Task>> member) =>
		Verify(member, TimesFactory.Instance.Never);

	public IMock<TService> VerifyNever<TValue>(Expression<Func<TService, Task<TValue>>> member) =>
		Verify(member, TimesFactory.Instance.Never);

	public void VerifyNoOtherCalls()
	{
		_spy.VerifyNoOtherCalls();
	}
}
