using LeanTest.Dependencies.Configuration;

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
	public bool HasUncheckedItems => _invocationRecords.Any(x => !x.HasBeenValidated);

	internal void Add(MethodBase methodInfo, object?[] parameters, bool successful)
	{
		_invocationRecords.Add(new InvocationRecord(methodInfo, parameters, successful));
	}

	internal uint Count(MethodInfo method, ConfiguredParametersCollection parameters)
	{
		uint count = 0;
		for (int i = 0; i < _invocationRecords.Count; i++)
		{
			var invocation = _invocationRecords[i];
			if (invocation.HasBeenValidated) continue;
			if (!invocation.Matches(method, parameters)) continue;
			count++;
			invocation.MarkAsValidated();
			_invocationRecords[i] = invocation;
		}
		return count;
	}
}
