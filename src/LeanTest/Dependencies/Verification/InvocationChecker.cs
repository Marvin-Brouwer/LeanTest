using LeanTest.Dependencies.Definitions;
using LeanTest.Exceptions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LeanTest.Dependencies.Verification;

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
		if (_invocationRecordList.HasItems) throw new LeanTestException("TODO");
	}
}
