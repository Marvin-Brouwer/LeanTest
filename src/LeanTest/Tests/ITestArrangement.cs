using System.Reflection;

namespace LeanTest.Tests;

public interface ITestArangement
{
	Task<IDictionary<string, (Type, object?)>> CallArrange(
		ITestSuite suite,
		ParameterInfo[]? actParameters,
		CancellationToken cancellationToken
	);
}
