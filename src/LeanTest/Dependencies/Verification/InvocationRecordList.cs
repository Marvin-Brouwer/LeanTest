using LeanTest.Dependencies.Verification;

using System.Reflection;

namespace LeanTest.Dynamic.Invocation;

internal sealed class InvocationRecordList
{
	private readonly List<InvocationRecord> _invocationRecords;

	public InvocationRecordList()
	{
		_invocationRecords = new ();
	}

	internal void Add(MethodBase methodInfo, object?[] parameters, Exception? ex = null)
	{
		_invocationRecords.Add(new InvocationRecord(methodInfo, parameters, ex));
	}
}
