namespace LeanTest
{
    public interface ITestSuite
    {
        Type ServiceType { get; }
        TestCollection Tests { get; }
    }
}