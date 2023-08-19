using System.Reflection;

namespace LeanTest.Dynamic.Invocation;

internal sealed class DummyInvocationMarshall : IInvokeInterceptor
{
	public TReturn RequestInvoke<TReturn>(MethodBase methodInfo) => RequestInvoke<TReturn>(methodInfo, Array.Empty<object>());
	public TReturn RequestInvoke<TReturn>( MethodBase methodInfo, object?[] parameters)
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

	public void RequestInvoke(MethodBase methodInfo) => RequestInvoke(methodInfo, Array.Empty<object>());
	public void RequestInvoke(MethodBase methodInfo, object?[] parameters)
	{
		// Do nothing
	}
}
