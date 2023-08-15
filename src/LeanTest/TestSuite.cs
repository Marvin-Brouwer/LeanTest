using LeanTest.Dependencies.Factories;
using LeanTest.Tests;
using LeanTest.Tests.Naming;
using LeanTest.Tests.TestBody;

using Microsoft.Extensions.Logging;

using System.Linq.Expressions;

namespace LeanTest;

public abstract record TestSuite<TSut> : ITestSuite
{
	public Type ServiceType => typeof(TSut);


	#region Dependencies
	// TODO, since we have a TestSuite per invocation, we can new these up and share a context for IParameterFactory
	protected readonly IStubFactory Stub = StubFactory.Instance;
	protected readonly ISpyFactory Spy = SpyFactory.Instance;
	protected readonly IMockFactory Mock = MockFactory.Instance;
	protected readonly IFixtureFactory Fixture = FixtureFactory.Instance;
	protected readonly IDummyFactory Dummy = DummyFactory.Instance;

	protected readonly IParameterFactory Parameter = ParameterFactory.Instance;
	protected readonly ITimesFactory Times = TimesFactory.Instance;

	internal protected ILogger<TSut> TestOutputLogger { get; internal set; } = default!;
	protected ILoggerFactory TestOutputLoggerFactory => Stub.Of<ILoggerFactory>()
		.Setup(factory => factory.CreateLogger(typeof(TSut).Name), () => TestOutputLogger)
		.Setup(
			factory => factory.CreateLogger(Parameter.Is<string>()),
			() => throw new NotSupportedException($"The {nameof(TestOutputLoggerFactory)} " +
				$"only supports {nameof(ILoggerFactory.CreateLogger)} with a categoryName of \"{typeof(TSut).Name}\"."
			)
		)
		.Instance;
	protected ILoggerProvider TestOutputLoggerProvider => Stub.Of<ILoggerProvider>()
		.Setup(factory => factory.CreateLogger(typeof(TSut).Name), () => TestOutputLogger)
		.Setup(
			provider => provider.CreateLogger(Parameter.Is<string>()),
			() => throw new NotSupportedException($"The {nameof(TestOutputLoggerProvider)} " +
				$"only supports {nameof(ILoggerProvider.CreateLogger)} with a categoryName of \"{typeof(TSut).Name}\"."
			)
		)
		.Instance;
	#endregion


	#region Tests
	public abstract TestCollection Tests { get; }

	protected ITestScenario Test(
		Expression<Func<TSut, Delegate>> method, ITestName testName,
		ITestArangement arrange, ITestAction act, ITestAssertion assert
	)
	{
		var scenarioName = testName.GetName(method);
		return new TestScenario(
			GetType(),
			ServiceType, scenarioName,
			arrange, act, assert
		);
	}
	#endregion

	#region Naming
	protected Given Given(string value) => new(value);
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