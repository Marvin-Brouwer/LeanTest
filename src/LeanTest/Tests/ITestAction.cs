using System.Reflection;

namespace LeanTest.Tests
{
    public interface ITestAction
    {
        ParameterInfo[] GetParameters();
        Task CallAct(ITestSuite suite, IDictionary<string, (Type, object?)> parameters, CancellationToken cancellationToken);
    }
}
