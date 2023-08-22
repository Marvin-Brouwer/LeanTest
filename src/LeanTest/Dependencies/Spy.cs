using LeanTest.Dependencies.Definitions;
using LeanTest.Dependencies.Providers;
using LeanTest.Dependencies.Verification;

using System.Linq.Expressions;

namespace LeanTest.Dependencies;

public sealed class Spy<TService> :
	IVerifyableDependency<TService, Spy<TService>>
{
	private readonly InvocationRecordList _invocationRecordList;

	/// <inheritdoc />
	public TService Instance { get; }

	/// <summary>
	/// This is merely a shorthand for <see cref="Spy{TService}.Instance"/>
	/// </summary>
	public static TService operator ~(Spy<TService> spy) => spy.Instance;

	internal Spy(InvocationRecordList invocationRecordList, TService service)
	{
		_invocationRecordList = invocationRecordList;
		Instance = service;
	}

	public Spy<TService> Verify(Expression<Func<TService>> member, ITimesConstraint times)
	{
		throw new NotImplementedException();
	}

	public Spy<TService> Verify<TValue>(Expression<Func<TService, TValue>> member, ITimesConstraint times)
	{
		throw new NotImplementedException();
	}

	public Spy<TService> Verify(Expression<Func<TService, Task>> member, ITimesConstraint times)
	{
		throw new NotImplementedException();
	}

	public Spy<TService> Verify<TValue>(Expression<Func<TService, Task<TValue>>> member, ITimesConstraint times)
	{
		throw new NotImplementedException();
	}

	public Spy<TService> VerifyOnce(Expression<Func<TService>> member) =>
		Verify(member, TimesContstraintProvider.Instance.Once);

	public Spy<TService> VerifyOnce<TValue>(Expression<Func<TService, TValue>> member) =>
		Verify(member, TimesContstraintProvider.Instance.Once);

	public Spy<TService> VerifyOnce(Expression<Func<TService, Task>> member) =>
		Verify(member, TimesContstraintProvider.Instance.Once);

	public Spy<TService> VerifyOnce<TValue>(Expression<Func<TService, Task<TValue>>> member) =>
		Verify(member, TimesContstraintProvider.Instance.Once);

	public Spy<TService> VerifyNever(Expression<Func<TService>> member) =>
		Verify(member, TimesContstraintProvider.Instance.Never);

	public Spy<TService> VerifyNever<TValue>(Expression<Func<TService, TValue>> member) =>
		Verify(member, TimesContstraintProvider.Instance.Never);

	public Spy<TService> VerifyNever(Expression<Func<TService, Task>> member) =>
		Verify(member, TimesContstraintProvider.Instance.Never);

	public Spy<TService> VerifyNever<TValue>(Expression<Func<TService, Task<TValue>>> member) =>
		Verify(member, TimesContstraintProvider.Instance.Never);

	public void VerifyNoOtherCalls()
	{
		throw new NotImplementedException();
	}
}
