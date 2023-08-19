using LeanTest.Dependencies.Configuration;
using LeanTest.Dependencies.Verification;

using System.Reflection;

namespace LeanTest.Dynamic.Invocation;

internal sealed class RecordingInvocationMarshall : IInvokeInterceptor
{
	private readonly InvocationRecordList _invocationRecords;
	private readonly InvocationMarshall _invocationMarshall;

	public RecordingInvocationMarshall(ConfiguredMethodSet configuredMethods, InvocationRecordList invocationRecords)
	{
		_invocationMarshall = new InvocationMarshall(configuredMethods);
		_invocationRecords = invocationRecords;
	}

	public TReturn RequestInvoke<TReturn>(MethodBase methodInfo) => RequestInvoke<TReturn>(methodInfo, Array.Empty<object>());
	public TReturn RequestInvoke<TReturn>( MethodBase methodInfo, object?[] parameters)
	{
		try
		{
			var returnValue = _invocationMarshall.RequestInvoke<TReturn>(methodInfo, parameters);
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
			_invocationMarshall.RequestInvoke(methodInfo, parameters);
			_invocationRecords.Add(methodInfo, parameters);
		}
		catch (Exception ex)
		{
			_invocationRecords.Add(methodInfo, parameters, ex);
			throw;
		}
	}
}
