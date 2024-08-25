using LeanTest.Dependencies.Factories;
using LeanTest.Dependencies.Providers;
using LeanTest.Dynamic.Generating;
using LeanTest.Hosting;
using LeanTest.Tests;

using Microsoft.Extensions.Logging;

namespace LeanTest;

public static partial class TestSuite
{
	public abstract class UnitTests
	{
		protected UnitTests()
		{
			CancellationToken = TestContext.Current.TestCancellationToken;

			var proxyGenerator = new RuntimeProxyGenerator(
				TestContext.Current.AssemblyContext,
				CancellationToken.ForTest
			);

			Stub = new StubFactory(proxyGenerator);
			Spy = new SpyFactory(proxyGenerator);
			Mock = new MockFactory(proxyGenerator);
			Dummy = new DummyFactory(proxyGenerator);
		}

		#region Dependencies
		protected readonly IStubFactory Stub;
		protected readonly ISpyFactory Spy;
		protected readonly IMockFactory Mock;
		protected readonly IFixtureFactory Fixture = FixtureFactory.Instance;
		protected readonly IDummyFactory Dummy;

		protected readonly IParameterFactory Parameter = ParameterFactory.Instance;
		protected readonly ITimesContstraintProvider Times = TimesContstraintProvider.Instance;

		protected readonly ICancellationTokenProvider CancellationToken;
		protected ILogger TestOutputLogger => TestOutputLoggerFactory.CreateLogger(GetType());
		protected ILoggerFactory TestOutputLoggerFactory => TestContext.Current.TestLoggerFactory;
		#endregion

		#region Tests
		protected UnitTestCase Test(Action test) => new(test);
		protected UnitTestCase Test(Func<ValueTask> test) => new(test);
		protected UnitTestDataScenario Test<T1>(IEnumerable<T1> testData, Action<T1> test) =>
			new(testData.Select(data => new object?[] { data }), test);
		protected UnitTestDataScenario Test<T1>(IEnumerable<T1> testData, Func<T1, ValueTask> test) =>
			new(testData.Select(data => new object?[] { data }), test);
		protected UnitTestDataScenario Test<T1>(Func<IEnumerable<T1>> testData, Action<T1> test) => Test(testData(), test);
		protected UnitTestDataScenario Test<T1>(Func<IEnumerable<T1>> testData, Func<T1, ValueTask> test) => Test(testData(), test);
		protected UnitTestDataScenario Test<T1, T2>(IEnumerable<(T1, T2)> testData, Action<T1, T2> test) =>
			new(testData.Select(data => new object?[] { data.Item1, data.Item2 }), test);
		protected UnitTestDataScenario Test<T1, T2>(IEnumerable<(T1, T2)> testData, Func<T1, T2, ValueTask> test) =>
			new(testData.Select(data => new object?[] { data.Item1, data.Item2 }), test);
		protected UnitTestDataScenario Test<T1, T2>(Func<IEnumerable<(T1, T2)>> testData, Action<T1, T2> test) => Test(testData(), test);
		protected UnitTestDataScenario Test<T1, T2>(Func<IEnumerable<(T1, T2)>> testData, Func<T1, T2, ValueTask> test) => Test(testData(), test);

		// TODO up to 5

		#endregion
	}
}