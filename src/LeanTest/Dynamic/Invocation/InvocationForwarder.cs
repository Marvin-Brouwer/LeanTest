using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace LeanTest.Dynamic.Invocation;

internal sealed class InvocationForwarder<TService> : IInvokeInterceptor
{
	private static object?[] EmptyParams = Array.Empty<object?[]>();

	private readonly TService _service;
	private readonly MethodInfo[] _methods;

	internal InvocationForwarder(TService service)
	{
		_service = service;
		_methods = service!.GetType().GetMethods();
	}

	public TReturn RequestInvoke<TReturn>(MethodBase methodInfo) => RequestInvoke<TReturn>(methodInfo, ref EmptyParams);
	public TReturn RequestInvoke<TReturn>(
		MethodBase methodInfo,
		ref object?[] parameters)
	{
		if (!TryFind<TReturn>(methodInfo, parameters, out var serviceMethod))
			throw new InvocationNotFoundException(methodInfo, parameters, typeof(TReturn));

		return (TReturn)serviceMethod.Invoke(_service, parameters)!;
	}

	public void RequestInvoke(MethodBase methodInfo) => RequestInvoke(methodInfo, ref EmptyParams);
	public void RequestInvoke(
		MethodBase methodInfo,
		ref object?[] parameters)
	{
		if (!TryFind(methodInfo, parameters, out var serviceMethod))
			throw new InvocationNotFoundException(methodInfo, parameters);

		_ = serviceMethod.Invoke(_service, parameters)!;
	}

	private bool TryFind<TReturn>(MethodBase methodInfo, object?[] parameters, [NotNullWhen(true)] out MethodInfo? serviceMethod)
	{
		// TODO: this is duplicate
		serviceMethod = _methods
			.Where(method => method.Name.Equals(methodInfo.Name, StringComparison.Ordinal))
			.Where(method => method.ReturnType == typeof(TReturn))
			.FirstOrDefault(method => method.GetParameters().Length == parameters.Length);

		return serviceMethod is not null;
	}
	private bool TryFind(MethodBase methodInfo, object?[] parameters, [NotNullWhen(true)] out MethodInfo? serviceMethod)
	{
		// TODO: this is duplicate
		serviceMethod = _methods
			.Where(method => method.Name.Equals(methodInfo.Name, StringComparison.Ordinal))
			.FirstOrDefault(method => method.GetParameters().Length == parameters.Length);

		return serviceMethod is not null;
	}
}