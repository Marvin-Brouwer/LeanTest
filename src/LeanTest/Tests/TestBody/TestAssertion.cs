using LeanTest.Extensions;

namespace LeanTest.Tests.TestBody;

internal sealed record TestAssertion(Delegate Assert) : ITestAssertion
{
	Task ITestAssertion.CallAssert(
		ITestSuite suite,
		IDictionary<string, (Type, object?)> parameters,
		CancellationToken cancellationToken
	) {
		return Assert.ExecuteAsync(suite, parameters, cancellationToken);
	}
}
