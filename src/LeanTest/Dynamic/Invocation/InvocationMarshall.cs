using LeanTest.Dependencies.Configuration;

using System.Reflection;

namespace LeanTest.Dynamic.Invocation;

// TODO NotSupportedException => Custom exception

internal sealed class InvocationMarshall : IInvokeInterceptor
{
	private readonly ConfiguredMethodSet _configuredMethods;

	internal InvocationMarshall(ConfiguredMethodSet configuredMethods)
	{
		_configuredMethods = configuredMethods;
	}

	public TReturn RequestInvoke<TReturn>(MethodBase methodInfo) => RequestInvoke<TReturn>(methodInfo, Array.Empty<object>());
	public TReturn RequestInvoke<TReturn>(
		MethodBase methodInfo,
		object?[] parameters)
	{
		if (!_configuredMethods.TryFind<TReturn>(methodInfo, parameters, out var configuredMethod))
			throw new NotSupportedException();

		return (TReturn)configuredMethod.Invoke(parameters)!;
	}

	public void RequestInvoke(MethodBase methodInfo) => RequestInvoke(methodInfo, Array.Empty<object>());
	public void RequestInvoke(
		MethodBase methodInfo,
		object?[] parameters)
	{

		if (!_configuredMethods.TryFind(methodInfo, parameters, out var configuredMethod))
			throw new NotSupportedException();

		_ = configuredMethod.Invoke(parameters)!;
	}
}