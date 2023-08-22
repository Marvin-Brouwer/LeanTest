using LeanTest.Dependencies.Factories;
using LeanTest.Dependencies.Providers;
using LeanTest.Dynamic.Generating;
using LeanTest.Hosting;
using LeanTest.Tests;
using LeanTest.Tests.Naming;
using LeanTest.Tests.TestBody;

using Microsoft.Extensions.Logging;

using System.Linq.Expressions;

namespace LeanTest;

public abstract class TestSuite : ITestSuite
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

	// TODO ask the public which is better
	#region Tests
	public virtual TestCollection Tests { get; }

	protected ITestScenario Test(
		Action test
	)
	{
		return new TestScenario(
			GetType(),
			"",
			// TODO these are just here for the example
			null,
			null,
			Assert(test)
		);
	}

	protected ITestScenario Test<T1>(
		Func<IEnumerable<T1>> seed,
		Action<T1> test
	)
	{
		return new TestScenario(
			GetType(),
			"",
			// TODO these are just here for the example
			null,
			null,
			Assert(test)
		);
	}

	protected ITestScenario Test<T1, T2>(
		Func<IEnumerable<(T1, T2)>> seed,
		Action<T1, T2> test
	)
	{
		return new TestScenario(
			GetType(),
			"",
			// TODO these are just here for the example
			null,
			null,
			Assert(test)
		);
	}

	protected ITestScenario TestClassic(
		Action test
	)
	{
		return new TestScenario(
			GetType(),
			"",
			// TODO these are just here for the example
			null,
			null,
			Assert(test)
		);
	}
	protected ITestScenario TestClassic(
		ITestName testName,
		Action test
	)
	{
		var scenarioName = testName.GetNormalizedName();
		return new TestScenario(
			GetType(),
			scenarioName,
			// TODO these are just here for the example
			null,
			null,
			Assert(test)
		);
	}
	protected ITestScenario TestClassic(
		string testName,
		Action test
	)
	{
		return new TestScenario(
			GetType(),
			testName,
			// TODO these are just here for the example
			null,
			null,
			Assert(test)
		);
	}

	protected ITestScenario TestClassic(
		ITestName testName, Func<Task> test
	)
	{
		var scenarioName = testName.GetNormalizedName();
		return new TestScenario(
			GetType(),
			scenarioName,
			// TODO these are just here for the example
			null,
			null,
			Assert(test)
		);
	}

	protected ITestScenario TestTripleA(
		ITestArangement arrange, ITestAction act, ITestAssertion assert
	)
	{
		return new TestScenario(
			GetType(),
			"",
			arrange, act, assert
		);
	}

	protected ITestScenario TestTripleA(
		ITestName testName, ITestArangement arrange, ITestAction act, ITestAssertion assert
	)
	{
		var scenarioName = testName.GetNormalizedName();
		return new TestScenario(
			GetType(),
			scenarioName,
			arrange, act, assert
		);
	}
	#endregion

	#region Naming
	protected Given Given(string value) => new(null, value);
	protected For For<TSut>(Expression<Func<TSut, Delegate>> methodExpression) => new(methodExpression);
	#endregion

	#region AAA
	protected ITestArangement Arrange<TArrange>(TArrange arrange)
		where TArrange : Delegate => new TestArrangement(arrange);
	protected ITestAction Act<TAct>(TAct act)
		where TAct : Delegate => new TestAction(act);
	protected ITestAssertion Assert<TAssert>(TAssert assert)
		where TAssert : Delegate => new TestAssertion(assert);
	#endregion
}