namespace LeanTest.Tests
{
    public interface ITestScenario
    {
        Type ServiceType { get; }
        // TODO Define returnType like ITestReport
        Task Run(CancellationToken cancellationToken);
    }
}
