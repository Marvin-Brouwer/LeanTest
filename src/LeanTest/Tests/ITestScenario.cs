namespace LeanTest.Tests;

public interface ITestScenario
{
	Type SuiteType { get; }
	// TODO Define returnType like ITestReport
	Task Run(CancellationToken cancellationToken);
}
