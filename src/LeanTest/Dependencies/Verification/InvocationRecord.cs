using LeanTest.Dependencies.Configuration;

using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace LeanTest.Dependencies.Verification;

[DebuggerDisplay("{DebugDisplay}")]
internal record struct InvocationRecord
{
	private string DebugDisplay =>
		(Successful ? "SuccessfulInvocation" : "FailedInvocation") + " -> " +
		Method.Name;

	public MethodBase Method { get; }
	public object?[] Parameters { get; }
	public Type[] ParameterTypes { get; }
	public bool HasBeenValidated { get; private set; }

	public bool Successful { get; }

	public InvocationRecord(MethodBase method, object?[] parameters, bool successful)
	{
		Method = method;
		Parameters = parameters;
		ParameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
		Successful = successful;
		HasBeenValidated = false;
	}

	public void MarkAsValidated() => HasBeenValidated = true;
	public bool Matches(InvocationRecord other)
	{
		if (!Method.Name.Equals(other.Method.Name)) return false;
		if (!SequenceMarshal.Equals(Parameters, other.Parameters)) return false;
		return true;
	}

	public bool Matches(ConfiguredMethod other)
	{
		if (!Method.Name.Equals(other.Method.Name)) return false;
		if (!other.Parameters.ParametersMatch(Parameters)) return false;
		return true;
	}

	public bool Matches(MethodInfo method, ConfiguredParametersCollection parameters)
	{
		if (!Method.Name.Equals(method.Name)) return false;
		if (!parameters.ParametersMatch(Parameters)) return false;
		return true;
	}
};
