using LeanTest.Dependencies.Verification;

using System.Reflection;

namespace LeanTest.Dynamic.Invocation;

internal sealed class RecordingInvocationMarshall : IInvokeInterceptor
{
	private readonly InvocationRecordList _invocationRecords;
	private readonly IInvokeInterceptor _invocationMarshall;

	public RecordingInvocationMarshall(IInvokeInterceptor invocationMarshall, InvocationRecordList invocationRecords)
	{
		_invocationMarshall = invocationMarshall;
		_invocationRecords = invocationRecords;
	}

	public TReturn RequestInvoke<TReturn>(MethodBase methodInfo) => RequestInvoke<TReturn>(methodInfo, Array.Empty<object>());
	public TReturn RequestInvoke<TReturn>( MethodBase methodInfo, object?[] parameters)
	{
		try
		{
			var returnValue = _invocationMarshall.RequestInvoke<TReturn>(methodInfo, parameters);
			_invocationRecords.Add(methodInfo, parameters, true);
			return returnValue;
		}
		catch
		{
			_invocationRecords.Add(methodInfo, parameters, false);
			throw;
		}
	}

	public void RequestInvoke(MethodBase methodInfo) => RequestInvoke(methodInfo, Array.Empty<object>());
	public void RequestInvoke(MethodBase methodInfo, object?[] parameters)
	{
		try
		{
			_invocationMarshall.RequestInvoke(methodInfo, parameters);
			_invocationRecords.Add(methodInfo, parameters, true);
		}
		catch
		{
			_invocationRecords.Add(methodInfo, parameters, false);
			throw;
		}
	}
}
