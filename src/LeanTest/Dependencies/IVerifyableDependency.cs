using LeanTest.Dependencies.Definitions;

using System.Linq.Expressions;

namespace LeanTest.Dependencies;

public interface IVerifyableDependency<TService, TDependency> : IDependency<TService>
	where TDependency : IVerifyableDependency<TService, TDependency>
{
	TDependency Verify(ITimesConstraint times, Expression<Func<TService>> member);
	TDependency Verify<TValue>(ITimesConstraint times, Expression<Func<TService, TValue>> member);
	TDependency Verify(ITimesConstraint times, Expression<Func<TService, Task>> member);
	TDependency Verify<TValue>(ITimesConstraint times, Expression<Func<TService, Task<TValue>>> member);
	TDependency VerifyOnce(Expression<Func<TService>> member);
	TDependency VerifyOnce<TValue>(Expression<Func<TService, TValue>> member);
	TDependency VerifyOnce(Expression<Func<TService, Task>> member);
	TDependency VerifyOnce<TValue>(Expression<Func<TService, Task<TValue>>> member);
	TDependency VerifyExactly(uint amountOfTimes, Expression<Func<TService>> member);
	TDependency VerifyExactly<TValue>(uint amountOfTimes, Expression<Func<TService, TValue>> member);
	TDependency VerifyExactly(uint amountOfTimes, Expression<Func<TService, Task>> member);
	TDependency VerifyExactly<TValue>(uint amountOfTimes, Expression<Func<TService, Task<TValue>>> member);
	TDependency VerifyAtLeast(uint amountOfTimes, Expression<Func<TService>> member);
	TDependency VerifyAtLeast<TValue>(uint amountOfTimes, Expression<Func<TService, TValue>> member);
	TDependency VerifyAtLeast(uint amountOfTimes, Expression<Func<TService, Task>> member);
	TDependency VerifyAtLeast<TValue>(uint amountOfTimes, Expression<Func<TService, Task<TValue>>> member);
	TDependency VerifyAtMost(uint amountOfTimes, Expression<Func<TService>> member);
	TDependency VerifyAtMost<TValue>(uint amountOfTimes, Expression<Func<TService, TValue>> member);
	TDependency VerifyAtMost(uint amountOfTimes, Expression<Func<TService, Task>> member);
	TDependency VerifyAtMost<TValue>(uint amountOfTimes, Expression<Func<TService, Task<TValue>>> member);
	TDependency VerifyBetween(uint leastAmountOfTimes, uint mostAmountOfTimes,Expression<Func<TService>> member);
	TDependency VerifyBetween<TValue>(uint leastAmountOfTimes, uint mostAmountOfTimes, Expression<Func<TService, TValue>> member);
	TDependency VerifyBetween(uint leastAmountOfTimes, uint mostAmountOfTimes, Expression<Func<TService, Task>> member);
	TDependency VerifyBetween<TValue>(uint leastAmountOfTimes, uint mostAmountOfTimes, Expression<Func<TService, Task<TValue>>> member);
	TDependency VerifyBetween(uint leastAmountOfTimes, uint mostAmountOfTimes, bool inclusive, Expression<Func<TService>> member);
	TDependency VerifyBetween<TValue>(uint leastAmountOfTimes, uint mostAmountOfTimes, bool inclusive, Expression<Func<TService, TValue>> member);
	TDependency VerifyBetween(uint leastAmountOfTimes, uint mostAmountOfTimes, bool inclusive, Expression<Func<TService, Task>> member);
	TDependency VerifyBetween<TValue>(uint leastAmountOfTimes, uint mostAmountOfTimes, bool inclusive, Expression<Func<TService, Task<TValue>>> member);
	TDependency VerifyNever(Expression<Func<TService>> member);
	TDependency VerifyNever<TValue>(Expression<Func<TService, TValue>> member);
	TDependency VerifyNever(Expression<Func<TService, Task>> member);
	TDependency VerifyNever<TValue>(Expression<Func<TService, Task<TValue>>> member);

	void VerifyNoOtherCalls();
	void VerifyNoCalls();
}
