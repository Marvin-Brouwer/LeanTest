using LeanTest.Dependencies.Definitions;
using LeanTest.Dependencies.Providers;
using LeanTest.Dependencies.Verification;

using System.Linq.Expressions;

namespace LeanTest.Dependencies;

public sealed class Spy<TService> :
	IVerifyableDependency<TService, Spy<TService>>
{
	private readonly InvocationRecordList _invocationRecordList;
	private readonly InvocationChecker _invocationChecker;

	/// <inheritdoc />
	public TService Instance { get; }

	/// <summary>
	/// This is merely a shorthand for <see cref="Spy{TService}.Instance"/>
	/// </summary>
	public static TService operator ~(Spy<TService> spy) => spy.Instance;

	internal Spy(InvocationRecordList invocationRecordList, TService service)
	{
		// TODO, this probably isn't threadsafe, perhaps it's cool to have a context associated with the current thread,
		// or in other words the current Test(() => ....)
		_invocationRecordList = invocationRecordList;
		_invocationChecker = new InvocationChecker(invocationRecordList);
		Instance = service;
	}

	public Spy<TService> Verify(ITimesConstraint times, Expression<Action<TService>> member)
	{
		_invocationChecker.Verify(times, member);
		return this;
	}
	public Spy<TService> Verify<TValue>(ITimesConstraint times, Expression<Func<TService, TValue>> member)
	{
		_invocationChecker.Verify(times, member);
		return this;
	}

	public Spy<TService> Verify(ITimesConstraint times, Expression<Func<TService, Task>> member)
	{
		_invocationChecker.Verify(times, member);
		return this;
	}

	public Spy<TService> Verify<TValue>(ITimesConstraint times, Expression<Func<TService, Task<TValue>>> member)
	{
		_invocationChecker.Verify(times, member);
		return this;
	}

	public Spy<TService> VerifyOnce(Expression<Action<TService>> member) =>
		Verify(TimesContstraintProvider.Instance.Once, member);

	public Spy<TService> VerifyOnce<TValue>(Expression<Func<TService, TValue>> member) =>
		Verify(TimesContstraintProvider.Instance.Once, member);

	public Spy<TService> VerifyOnce(Expression<Func<TService, Task>> member) =>
		Verify(TimesContstraintProvider.Instance.Once, member);

	public Spy<TService> VerifyOnce<TValue>(Expression<Func<TService, Task<TValue>>> member) =>
		Verify(TimesContstraintProvider.Instance.Once, member);

	public Spy<TService> VerifyExactly(uint amountOfTimes, Expression<Action<TService>> member) =>
		Verify(TimesContstraintProvider.Instance.Exactly(amountOfTimes), member);
	public Spy<TService> VerifyExactly<TValue>(uint amountOfTimes, Expression<Func<TService, TValue>> member) =>
		Verify(TimesContstraintProvider.Instance.Exactly(amountOfTimes), member);
	public Spy<TService> VerifyExactly(uint amountOfTimes, Expression<Func<TService, Task>> member) =>
		Verify(TimesContstraintProvider.Instance.Exactly(amountOfTimes), member);
	public Spy<TService> VerifyExactly<TValue>(uint amountOfTimes, Expression<Func<TService, Task<TValue>>> member) =>
		Verify(TimesContstraintProvider.Instance.Exactly(amountOfTimes), member);
	public Spy<TService> VerifyAtLeast(uint amountOfTimes, Expression<Action<TService>> member) =>
		Verify(TimesContstraintProvider.Instance.AtLeast(amountOfTimes), member);
	public Spy<TService> VerifyAtLeast<TValue>(uint amountOfTimes, Expression<Func<TService, TValue>> member) =>
		Verify(TimesContstraintProvider.Instance.AtLeast(amountOfTimes), member);
	public Spy<TService> VerifyAtLeast(uint amountOfTimes, Expression<Func<TService, Task>> member) =>
		Verify(TimesContstraintProvider.Instance.AtLeast(amountOfTimes), member);
	public Spy<TService> VerifyAtLeast<TValue>(uint amountOfTimes, Expression<Func<TService, Task<TValue>>> member) =>
		Verify(TimesContstraintProvider.Instance.AtLeast(amountOfTimes), member);
	public Spy<TService> VerifyAtMost(uint amountOfTimes, Expression<Action<TService>> member) =>
		Verify(TimesContstraintProvider.Instance.AtMost(amountOfTimes), member);
	public Spy<TService> VerifyAtMost<TValue>(uint amountOfTimes, Expression<Func<TService, TValue>> member) =>
		Verify(TimesContstraintProvider.Instance.AtMost(amountOfTimes), member);
	public Spy<TService> VerifyAtMost(uint amountOfTimes, Expression<Func<TService, Task>> member) =>
		Verify(TimesContstraintProvider.Instance.AtMost(amountOfTimes), member);
	public Spy<TService> VerifyAtMost<TValue>(uint amountOfTimes, Expression<Func<TService, Task<TValue>>> member) =>
		Verify(TimesContstraintProvider.Instance.AtMost(amountOfTimes), member);
	public Spy<TService> VerifyBetween(uint leastAmountOfTimes, uint mostAmountOfTimes, Expression<Action<TService>> member) =>
		VerifyBetween(leastAmountOfTimes, mostAmountOfTimes, false, member);
	public Spy<TService> VerifyBetween<TValue>(uint leastAmountOfTimes, uint mostAmountOfTimes, Expression<Func<TService, TValue>> member) =>
		VerifyBetween(leastAmountOfTimes, mostAmountOfTimes, false, member);
	public Spy<TService> VerifyBetween(uint leastAmountOfTimes, uint mostAmountOfTimes, Expression<Func<TService, Task>> member) =>
		VerifyBetween(leastAmountOfTimes, mostAmountOfTimes, false, member);
	public Spy<TService> VerifyBetween<TValue>(uint leastAmountOfTimes, uint mostAmountOfTimes, Expression<Func<TService, Task<TValue>>> member) =>
		VerifyBetween(leastAmountOfTimes, mostAmountOfTimes, false, member);
	public Spy<TService> VerifyBetween(uint leastAmountOfTimes, uint mostAmountOfTimes, bool inclusive, Expression<Action<TService>> member) =>
		Verify(TimesContstraintProvider.Instance.Between(leastAmountOfTimes, mostAmountOfTimes, inclusive), member);
	public Spy<TService> VerifyBetween<TValue>(uint leastAmountOfTimes, uint mostAmountOfTimes, bool inclusive, Expression<Func<TService, TValue>> member) =>
		Verify(TimesContstraintProvider.Instance.Between(leastAmountOfTimes, mostAmountOfTimes, inclusive), member);
	public Spy<TService> VerifyBetween(uint leastAmountOfTimes, uint mostAmountOfTimes, bool inclusive, Expression<Func<TService, Task>> member) =>
		Verify(TimesContstraintProvider.Instance.Between(leastAmountOfTimes, mostAmountOfTimes, inclusive), member);
	public Spy<TService> VerifyBetween<TValue>(uint leastAmountOfTimes, uint mostAmountOfTimes, bool inclusive, Expression<Func<TService, Task<TValue>>> member) =>
		Verify(TimesContstraintProvider.Instance.Between(leastAmountOfTimes, mostAmountOfTimes, inclusive), member);

	public Spy<TService> VerifyNever(Expression<Action<TService>> member) =>
		Verify(TimesContstraintProvider.Instance.Never, member);

	public Spy<TService> VerifyNever<TValue>(Expression<Func<TService, TValue>> member) =>
		Verify(TimesContstraintProvider.Instance.Never, member);

	public Spy<TService> VerifyNever(Expression<Func<TService, Task>> member) =>
		Verify(TimesContstraintProvider.Instance.Never, member);

	public Spy<TService> VerifyNever<TValue>(Expression<Func<TService, Task<TValue>>> member) =>
		Verify(TimesContstraintProvider.Instance.Never, member);

	public void VerifyNoOtherCalls() => _invocationChecker.VerifyNoOtherCalls();
	public void VerifyNoCalls() => _invocationChecker.VerifyNoCalls();
}
