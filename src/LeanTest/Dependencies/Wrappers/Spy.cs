using LeanTest.Dependencies.Definitions;
using LeanTest.Dependencies.Providers;

using System.Linq.Expressions;

namespace LeanTest.Dependencies.Wrappers;

internal class Spy<TService> : ISpy<TService>
{
	public TService Instance { get; }

	public Spy(TService service)
	{
	}

	public ISpy<TService> Verify(Expression<Func<TService>> member, ITimesConstraint times)
	{
		throw new NotImplementedException();
	}

	public ISpy<TService> Verify<TValue>(Expression<Func<TService, TValue>> member, ITimesConstraint times)
	{
		throw new NotImplementedException();
	}

	public ISpy<TService> Verify(Expression<Func<TService, Task>> member, ITimesConstraint times)
	{
		throw new NotImplementedException();
	}

	public ISpy<TService> Verify<TValue>(Expression<Func<TService, Task<TValue>>> member, ITimesConstraint times)
	{
		throw new NotImplementedException();
	}

	public ISpy<TService> VerifyOnce(Expression<Func<TService>> member) =>
		Verify(member, TimesContstraintProvider.Instance.Once);

	public ISpy<TService> VerifyOnce<TValue>(Expression<Func<TService, TValue>> member) =>
		Verify(member, TimesContstraintProvider.Instance.Once);

	public ISpy<TService> VerifyOnce(Expression<Func<TService, Task>> member) =>
		Verify(member, TimesContstraintProvider.Instance.Once);

	public ISpy<TService> VerifyOnce<TValue>(Expression<Func<TService, Task<TValue>>> member) =>
		Verify(member, TimesContstraintProvider.Instance.Once);

	public ISpy<TService> VerifyNever(Expression<Func<TService>> member) =>
		Verify(member, TimesContstraintProvider.Instance.Never);

	public ISpy<TService> VerifyNever<TValue>(Expression<Func<TService, TValue>> member) =>
		Verify(member, TimesContstraintProvider.Instance.Never);

	public ISpy<TService> VerifyNever(Expression<Func<TService, Task>> member) =>
		Verify(member, TimesContstraintProvider.Instance.Never);

	public ISpy<TService> VerifyNever<TValue>(Expression<Func<TService, Task<TValue>>> member) =>
		Verify(member, TimesContstraintProvider.Instance.Never);

	public void VerifyNoOtherCalls()
	{
		throw new NotImplementedException();
	}
}
