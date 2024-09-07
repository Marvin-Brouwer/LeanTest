using LeanTest.Dependencies.Configuration;

using System.Reflection;

namespace LeanTest.Dynamic.Invocation;

internal sealed class InvocationMarshall : IInvokeInterceptor
{
	private static object?[] EmptyParams = Array.Empty<object>();

	private readonly ConfiguredMethodSet _configuredMethods;

	internal InvocationMarshall(ConfiguredMethodSet configuredMethods)
	{
		_configuredMethods = configuredMethods;
	}

	public TReturn RequestInvoke<TReturn>(MethodBase methodInfo) => RequestInvoke<TReturn>(methodInfo, ref EmptyParams);
	public TReturn RequestInvoke<TReturn>(
		MethodBase methodInfo,
		ref object?[] parameters)
	{
		if (!_configuredMethods.TryFind<TReturn>(methodInfo, parameters, out var configuredMethod))
			throw new InvocationNotFoundException(methodInfo, parameters, typeof(TReturn));

		return (TReturn)configuredMethod.Invoke(parameters)!;
	}

	public void RequestInvoke(MethodBase methodInfo) => RequestInvoke(methodInfo, ref EmptyParams);
	public void RequestInvoke(
		MethodBase methodInfo,
		ref object?[] parameters)
	{
		if (!_configuredMethods.TryFind(methodInfo, parameters, out var configuredMethod))
			throw new InvocationNotFoundException(methodInfo, parameters);

		_ = configuredMethod.Invoke(parameters)!;
	}
}