using System.Reflection;

namespace LeanTest.Dynamic.Invocation;

internal sealed class DummyInvocationMarshall : IInvokeInterceptor
{
	private static object?[] EmptyParams = Array.Empty<object>();

	public TReturn RequestInvoke<TReturn>(MethodBase methodInfo) => RequestInvoke<TReturn>(methodInfo, ref EmptyParams);
	public TReturn RequestInvoke<TReturn>( MethodBase methodInfo, ref object?[] parameters)
	{
		if (typeof(TReturn).IsValueType) return default!;
		if (typeof(TReturn).IsAbstract) return default!;
		if (typeof(TReturn).IsInterface) return default!;
		if (typeof(TReturn).IsNotPublic) return default!;

		try
		{
			return Activator.CreateInstance<TReturn>();
		}
		catch (Exception)
		{
			return default!;
		}
	}

	public void RequestInvoke(MethodBase methodInfo) => RequestInvoke(methodInfo, ref EmptyParams);
	public void RequestInvoke(MethodBase methodInfo, ref object?[] parameters)
	{
		// Do nothing
	}
}
