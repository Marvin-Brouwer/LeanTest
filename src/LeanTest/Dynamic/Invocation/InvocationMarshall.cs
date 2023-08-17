using System.Reflection;

namespace LeanTest.Dynamic.Invocation;

// TODO NotSupportedException => Custom exception
public static class InvocationMarshall
{
	public static TReturn InvokeStub<TReturn>(
		ConfiguredMethodSet configuredMethods,
		MethodBase methodInfo,
		object?[] parameters)
	{
		if (!configuredMethods.TryFind<TReturn>(methodInfo, parameters, out var returnDelegate))
			throw new NotSupportedException();

		return returnDelegate()!;
	}

	public static void InvokeStub(
		ConfiguredMethodSet configuredMethods,
		MethodBase methodInfo,
		object?[] parameters)
	{

		if (!configuredMethods.TryFind(methodInfo, parameters, out var returnDelegate))
			throw new NotSupportedException();

		returnDelegate.DynamicInvoke(parameters);
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
		ConfiguredMethodSet configuredMethods,
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
		ConfiguredMethodSet configuredMethods,
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
