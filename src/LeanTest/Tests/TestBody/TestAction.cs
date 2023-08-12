using System.Reflection;

using LeanTest.Extensions;

namespace LeanTest.Tests.TestBody;

internal sealed record TestAction(Delegate Act) : ITestAction
{
	public ParameterInfo[] GetParameters()
	{
		return Act.GetMethodInfo().GetParameters();
	}

	async Task ITestAction.CallAct(
		ITestSuite suite,
		IDictionary<string, (Type, object?)> parameters,
		CancellationToken cancellationToken
	) {
		var result = await Act.ExecuteAsync(suite, parameters, cancellationToken);
		parameters.Add("result", (Act.GetMethodInfo().ReturnType, result));
	}
}
