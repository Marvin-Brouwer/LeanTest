using System.Reflection;

namespace LeanTest.Dynamic.Invocation;

internal sealed class InvocationRecorder<TService> : IInvokeInterceptor
{
	private readonly TService? _service;
	private readonly InvocationRecordList _invocationRecords;

	public InvocationRecorder(TService service, InvocationRecordList invocationRecords)
	{
		_service = service;
		_invocationRecords = invocationRecords;
	}

	public TReturn RequestInvoke<TReturn>(MethodBase methodInfo) => RequestInvoke<TReturn>(methodInfo, Array.Empty<object>());
	public TReturn RequestInvoke<TReturn>( MethodBase methodInfo, object?[] parameters)
	{
		try
		{
			var returnValue = (TReturn)methodInfo.Invoke(_service, parameters)!;
			_invocationRecords.Add(methodInfo, parameters);
			return returnValue;
		}
		catch (Exception ex)
		{
			_invocationRecords.Add(methodInfo, parameters, ex);
			throw;
		}
	}

	public void RequestInvoke(MethodBase methodInfo) => RequestInvoke(methodInfo, Array.Empty<object>());
	public void RequestInvoke(MethodBase methodInfo, object?[] parameters)
	{
		try
		{
			_ = methodInfo.Invoke(_service, parameters)!;
			_invocationRecords.Add(methodInfo, parameters);
		}
		catch (Exception ex)
		{
			_invocationRecords.Add(methodInfo, parameters, ex);
			throw;
		}
	}
}
