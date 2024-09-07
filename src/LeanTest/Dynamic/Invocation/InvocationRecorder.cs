using LeanTest.Dependencies.Verification;

using System.Reflection;

namespace LeanTest.Dynamic.Invocation;

internal sealed class InvocationRecorder<TService> : IInvokeInterceptor
{
	private static object?[] EmptyParams = Array.Empty<object>();

	private readonly TService? _service;
	private readonly InvocationRecordList _invocationRecords;

	public InvocationRecorder(TService service, InvocationRecordList invocationRecords)
	{
		_service = service;
		_invocationRecords = invocationRecords;
	}

	public TReturn RequestInvoke<TReturn>(MethodBase methodInfo) => RequestInvoke<TReturn>(methodInfo, ref EmptyParams);
	public TReturn RequestInvoke<TReturn>(MethodBase methodInfo, ref object?[] parameters)
	{
		try
		{
			var returnValue = (TReturn)methodInfo.Invoke(_service, parameters)!;
			_invocationRecords.Add(methodInfo, parameters, true);
			return returnValue;
		}
		catch
		{
			_invocationRecords.Add(methodInfo, parameters, false);
			throw;
		}
	}

	public void RequestInvoke(MethodBase methodInfo) => RequestInvoke(methodInfo, ref EmptyParams);
	public void RequestInvoke(MethodBase methodInfo, ref object?[] parameters)
	{
		try
		{
			_ = methodInfo.Invoke(_service, parameters)!;
			_invocationRecords.Add(methodInfo, parameters, true);
		}
		catch
		{
			_invocationRecords.Add(methodInfo, parameters, false);
			throw;
		}
	}
}
