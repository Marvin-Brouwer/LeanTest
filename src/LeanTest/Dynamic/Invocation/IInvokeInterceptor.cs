using System.Reflection;

namespace LeanTest.Dynamic.Invocation;

public interface IInvokeInterceptor
{
	TReturn RequestInvoke<TReturn>(MethodBase methodInfo);
	TReturn RequestInvoke<TReturn>(MethodBase methodInfo, object?[] parameters);
	void RequestInvoke(MethodBase methodInfo);
	void RequestInvoke(MethodBase methodInfo, object?[] parameters);
}