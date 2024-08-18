using LeanTest.Dependencies.Definitions;

using System.Linq.Expressions;

namespace LeanTest.Dependencies.Verification;

// TODO Implement like TimesContstraint
internal class InvocationChecker
{
	private readonly InvocationRecordList _invocationRecordList;

	public InvocationChecker(InvocationRecordList invocationRecordList)
	{
		_invocationRecordList = invocationRecordList;
	}

	public void Verify<TService>(ITimesConstraint timesConstraint, Expression<Func<TService>> member)
	{
		// TODO
		throw new NotImplementedException();
	}
	public void Verify<TService, TValue>(ITimesConstraint timesConstraint, Expression<Func<TService, TValue>> member)
	{
		// TODO
		throw new NotImplementedException();
	}
	public void Verify<TService>(ITimesConstraint timesConstraint, Expression<Func<TService, Task>> member)
	{
		// TODO
		throw new NotImplementedException();
	}
	public void Verify<TService, TValue>(ITimesConstraint timesConstraint, Expression<Func<TService, Task<TValue>>> member)
	{
		// TODO
		throw new NotImplementedException();
	}
	public void VerifyNoOtherCalls()
	{
		// TODO
		throw new NotImplementedException();
	}
	public void VerifyNoCalls()
	{
		if (_invocationRecordList.HasItems) throw new NotImplementedException("TODO implement like the TimesConstaint");
	}
}
