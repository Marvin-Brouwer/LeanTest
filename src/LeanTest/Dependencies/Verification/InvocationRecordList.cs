using System.Reflection;

namespace LeanTest.Dependencies.Verification;

internal sealed class InvocationRecordList
{
	private readonly List<InvocationRecord> _invocationRecords;

	public InvocationRecordList()
	{
		_invocationRecords = new();
	}

	public bool HasItems => _invocationRecords.Any();

	internal void Add(MethodBase methodInfo, object?[] parameters, Exception? ex = null)
	{
		_invocationRecords.Add(new InvocationRecord(methodInfo, parameters, ex));
	}
}
