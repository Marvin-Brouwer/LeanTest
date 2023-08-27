using LeanTest.Dependencies.Factories;
using LeanTest.Dependencies.Providers;
using LeanTest.Dynamic.Generating;
using LeanTest.Hosting;
using LeanTest.Tests;

using Microsoft.Extensions.Logging;

namespace LeanTest;

public abstract class TestSuite
{
	protected TestSuite()
	{
		TestOutputLoggerFactory = TestContext.Current.TestLoggerFactory;
		TestOutputLogger = TestContext.Current.TestLoggerFactory.CreateLogger(GetType());
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
	protected readonly ILogger TestOutputLogger;
	protected readonly ILoggerFactory TestOutputLoggerFactory;
	#endregion

	#region Tests
	protected TestCase Test(Action test) => new(test);
	protected TestCase Test(Func<ValueTask> test) => new(test);
	protected DataTestScenario Test<T1>(IEnumerable<T1> testData, Action<T1> test) =>
		new (testData.Select(data => new object?[] { data }), (object?[] data) => test((T1)data[0]!));
	protected DataTestScenario Test<T1>(IEnumerable<T1> testData, Func<T1, ValueTask> test) =>
		new(testData.Select(data => new object?[] { data }), (object?[] data) => test((T1)data[0]!));
	protected DataTestScenario Test<T1>(Func<IEnumerable<T1>> testData, Action<T1> test) => Test(testData(), test);
	protected DataTestScenario Test<T1>(Func<IEnumerable<T1>> testData, Func<T1, ValueTask> test) => Test(testData(), test);
	protected DataTestScenario Test<T1, T2>(IEnumerable<(T1, T2)> testData, Action<T1, T2> test) =>
		new(testData.Select(data => new object?[] { data.Item1, data.Item2 }), (object?[] data) => test((T1)data[0]!, (T2)data[1]!));
	protected DataTestScenario Test<T1, T2>(IEnumerable<(T1, T2)> testData, Func<T1, T2, ValueTask> test) =>
		new(testData.Select(data => new object?[] { data.Item1, data.Item2 }), (object?[] data) => test((T1)data[0]!, (T2)data[1]!));
	protected DataTestScenario Test<T1, T2>(Func<IEnumerable<(T1, T2)>> testData, Action<T1, T2> test) => Test(testData(), test);
	protected DataTestScenario Test<T1, T2>(Func<IEnumerable<(T1, T2)>> testData, Func<T1, T2, ValueTask> test) => Test(testData(), test);

	// TODO up to 5

	#endregion
}