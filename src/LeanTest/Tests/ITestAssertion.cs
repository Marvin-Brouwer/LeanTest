namespace LeanTest.Tests;

public interface ITestAssertion
{
	Task CallAssert(ITestSuite suite, IDictionary<string, (Type, object?)> parameters, CancellationToken cancellationToken);
}
