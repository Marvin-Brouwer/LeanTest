using LeanTest.Tests.Naming;

using System.Linq.Expressions;

namespace LeanTest.Tests
{
    public interface ITestName : ITestNamePart
    {
        string GetName(Expression methodExpression);
    }
}
