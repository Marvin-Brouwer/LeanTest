using LeanTest.Tests.Naming;

namespace LeanTest.Tests;

public interface ITestName : ITestNamePart
{
	string GetNormalizedName();
}
