using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LeanTest.Dependencies.Invocation;
public static class InvocationMarshall
{
	public static TReturn InvokeStub<TReturn>(
		IDictionary<MethodInfo, Action<object?>> configuredMethods,
		MethodBase methodInfo)
	{

		throw new NotSupportedException();
	}

	public static void InvokeStub(
		IDictionary<MethodInfo, Action<object?>> configuredMethods,
		MethodBase methodInfo)
	{

		throw new NotSupportedException();
	}

	public static void InvokeSpy(
		IDictionary<MethodInfo, IInvocationRecord> calledMethods,
		MethodBase methodInfo)
	{

		throw new NotSupportedException();
	}


	public static void InvokeSpy<TReturn>(
		IDictionary<MethodInfo, IInvocationRecord> calledMethods,
		MethodBase methodInfo)
	{

		throw new NotSupportedException();
	}

	public static TReturn InvokeMock<TReturn>(
		IDictionary<MethodInfo, Action<object?>> configuredMethods,
		IDictionary<MethodInfo, IInvocationRecord> calledMethods,
		MethodBase methodInfo)
	{

		try
		{
			return InvokeStub<TReturn>(configuredMethods, methodInfo);
		}
		finally
		{
			InvokeSpy<TReturn>(calledMethods, methodInfo);
		}
	}
	public static void InvokeMock(
		IDictionary<MethodInfo, Action<object?>> configuredMethods,
		IDictionary<MethodInfo, IInvocationRecord> calledMethods,
		MethodBase methodInfo)
	{

		try
		{
			InvokeStub(configuredMethods, methodInfo);
		}
		finally
		{
			InvokeSpy(calledMethods, methodInfo);
		}
	}
}
