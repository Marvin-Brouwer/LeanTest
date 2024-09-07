using System.Reflection;

namespace LeanTest.Dynamic.Invocation;

public interface IInvokeInterceptor
{
	TReturn RequestInvoke<TReturn>(MethodBase methodInfo);
	TReturn RequestInvoke<TReturn>(MethodBase methodInfo, ref object?[] parameters);
	void RequestInvoke(MethodBase methodInfo);
	void RequestInvoke(MethodBase methodInfo, ref object?[] parameters);
}