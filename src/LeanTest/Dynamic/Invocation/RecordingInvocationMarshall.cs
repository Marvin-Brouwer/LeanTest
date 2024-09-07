using LeanTest.Dependencies.Verification;

using System.Reflection;

namespace LeanTest.Dynamic.Invocation;

internal sealed class RecordingInvocationMarshall : IInvokeInterceptor
{
	private object?[] EmptyParams = Array.Empty<object>();

	private readonly InvocationRecordList _invocationRecords;
	private readonly IInvokeInterceptor _invocationMarshall;

	public RecordingInvocationMarshall(IInvokeInterceptor invocationMarshall, InvocationRecordList invocationRecords)
	{
		_invocationMarshall = invocationMarshall;
		_invocationRecords = invocationRecords;
	}

	public TReturn RequestInvoke<TReturn>(MethodBase methodInfo) => RequestInvoke<TReturn>(methodInfo, ref EmptyParams);
	public TReturn RequestInvoke<TReturn>( MethodBase methodInfo, ref object?[] parameters)
	{
		try
		{
			var returnValue = _invocationMarshall.RequestInvoke<TReturn>(methodInfo, ref parameters);
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
			_invocationMarshall.RequestInvoke(methodInfo, ref parameters);
			_invocationRecords.Add(methodInfo, parameters, true);
		}
		catch
		{
			_invocationRecords.Add(methodInfo, parameters, false);
			throw;
		}
	}
}
