using System.Reflection;

namespace LeanTest.Dependencies.Verification;

internal readonly record struct InvocationRecord
{
	private readonly MethodBase _methodInfo;
	private readonly object?[] _parameters;
	private readonly Exception? _ex;

	public InvocationRecord(MethodBase methodInfo, object?[] parameters, Exception? ex)
	{
		_methodInfo = methodInfo;
		_parameters = parameters;
		_ex = ex;
	}
}
