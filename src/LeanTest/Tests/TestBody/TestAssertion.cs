namespace LeanTest.Tests.TestBody;

internal sealed record TestAssertion(Delegate Assert) : TestDelegate(Assert), ITestAssertion
{
	Task ITestAssertion.CallAssert(
		ITestSuite suite,
		IDictionary<string, (Type, object?)> parameters,
		CancellationToken cancellationToken
	) {
		return ExecuteAsync(suite, parameters, cancellationToken);
	}
}
