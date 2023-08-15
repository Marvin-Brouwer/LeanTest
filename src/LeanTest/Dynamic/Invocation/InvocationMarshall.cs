using System.Reflection;

namespace LeanTest.Dynamic.Invocation;

// TODO implement arguments (requires boxing for valuetypes)
public static class InvocationMarshall
{
	public static TReturn InvokeStub<TReturn>(
		IDictionary<MethodInfo, Action<object?>> configuredMethods,
		MethodBase methodInfo,
		object?[] parameters)
	{

		throw new NotSupportedException();
	}

	public static void InvokeStub(
		IDictionary<MethodInfo, Action<object?>> configuredMethods,
		MethodBase methodInfo,
		object?[] parameters)
	{

		throw new NotSupportedException();
	}

	public static void InvokeSpy(
		IDictionary<MethodInfo, IInvocationRecord> calledMethods,
		MethodBase methodInfo,
		object?[] parameters)
	{

		throw new NotSupportedException();
	}


	public static void InvokeSpy<TReturn>(
		IDictionary<MethodInfo, IInvocationRecord> calledMethods,
		MethodBase methodInfo,
		object?[] parameters)
	{

		throw new NotSupportedException();
	}

	public static TReturn InvokeMock<TReturn>(
		IDictionary<MethodInfo, Action<object?>> configuredMethods,
		IDictionary<MethodInfo, IInvocationRecord> calledMethods,
		MethodBase methodInfo,
		object?[] parameters)
	{

		try
		{
			return InvokeStub<TReturn>(configuredMethods, methodInfo, parameters);
		}
		finally
		{
			InvokeSpy<TReturn>(calledMethods, methodInfo, parameters);
		}
	}
	public static void InvokeMock(
		IDictionary<MethodInfo, Action<object?>> configuredMethods,
		IDictionary<MethodInfo, IInvocationRecord> calledMethods,
		MethodBase methodInfo,
		object?[] parameters)
	{

		try
		{
			InvokeStub(configuredMethods, methodInfo, parameters);
		}
		finally
		{
			InvokeSpy(calledMethods, methodInfo, parameters);
		}
	}
}
