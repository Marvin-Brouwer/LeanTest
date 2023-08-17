using System.Reflection;

namespace LeanTest.Tests.TestBody;

internal sealed record TestArrangement(Delegate Arrange) : TestDelegate(Arrange), ITestArangement
{
	async Task<IDictionary<string, (Type, object?)>> ITestArangement.CallArrange(
		ITestSuite suite,
		ParameterInfo[]? actParameters,
		CancellationToken cancellationToken
	) {
		// TODO:
		// Check if return value is named tuple, throw if not
		// check on reserved name "result"
		// If tuple add types an values to dictionary
		// If not, get name from actParameters, add type and value to dictionary,

		//return dictionary;

		var result = await ExecuteAsync(suite, cancellationToken);
		throw new NotImplementedException();
	}
}
