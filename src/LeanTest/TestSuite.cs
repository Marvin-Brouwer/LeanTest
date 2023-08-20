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

public abstract record TestSuite<TSut> : ITestSuite
{
	public Type ServiceType => typeof(TSut);

	protected TestSuite()
	{
		TestOutputLogger = TestContext.Current.TestLoggerFactory.CreateLogger<TSut>();
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

	internal protected readonly ICancellationTokenProvider CancellationToken;
	internal protected readonly ILogger<TSut> TestOutputLogger;
	protected ILoggerFactory TestOutputLoggerFactory => Stub.Of<ILoggerFactory>()
		.Setup(factory => factory.CreateLogger(typeof(TSut).Name))
		.Returns(() => TestOutputLogger)
		.Setup(factory => factory.CreateLogger(Parameter.Is<string>()))
		.Returns(() => throw new NotSupportedException($"The {nameof(TestOutputLoggerFactory)} " +
			$"only supports {nameof(ILoggerFactory.CreateLogger)} with a categoryName of \"{typeof(TSut).Name}\"."
		))
		.Instance;

	protected ILoggerProvider TestOutputLoggerProvider => Stub.Of<ILoggerProvider>()
		.Setup(factory => factory.CreateLogger(typeof(TSut).Name)).Returns(() => TestOutputLogger)
		.Setup(provider => provider.CreateLogger(Parameter.Is<string>()))
		.Returns(() => throw new NotSupportedException($"The {nameof(TestOutputLoggerProvider)} " +
			$"only supports {nameof(ILoggerProvider.CreateLogger)} with a categoryName of \"{typeof(TSut).Name}\"."
		))
		.Instance;
	#endregion

	// TODO ask the public which is better
	#region Tests
	public abstract TestCollection Tests { get; }

	protected ITestScenario TestClassic(
		ITestName testName,
		Action test
	)
	{
		var scenarioName = testName.GetNormalizedName();
		return new TestScenario(
			GetType(),
			ServiceType, scenarioName,
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
			ServiceType, scenarioName,
			// TODO these are just here for the example
			null,
			null,
			Assert(test)
		);
	}

	protected ITestScenario TestTripleA(
		ITestName testName, ITestArangement arrange, ITestAction act, ITestAssertion assert
	)
	{
		var scenarioName = testName.GetNormalizedName();
		return new TestScenario(
			GetType(),
			ServiceType, scenarioName,
			arrange, act, assert
		);
	}
	#endregion

	#region Naming
	protected Given Given(string value) => new(null, value);
	protected For For(Expression<Func<TSut, Delegate>> methodExpression) => new(methodExpression);
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