// TODO figure out if this can be forced with an attribute or an analyzer
#pragma warning disable RCS1032 // Remove redundant parentheses.
#pragma warning disable IDE0055 // Remove redundant parentheses.
#pragma warning disable RCS1021 // Convert lambda expression body to expression body.

using ExampleProject.Models;
using ExampleProject.Services;
using ExampleProject.Tests.Fixtures;

using FluentAssertions;

using LeanTest;
using LeanTest.Dependencies;
using LeanTest.Dependencies.Async;

using Microsoft.Extensions.Logging;

using System;

namespace ExampleProject.Tests.TestSuites.Services;

public sealed class ExampleServiceTests : TestSuite
{
    private readonly Stub<ISomeThing> _someStub;
    private readonly Spy<ISomeThing> _someSpy;
    private readonly Mock<ISomeThing> _someMock;
	private readonly Fixture<SomeDataType> _someFixture;
	private readonly IServiceOutOfScope _outOfScopeDummy;
	private readonly Mock<IExampleService> _asyncExampleMock;

	public ExampleServiceTests()
    {
		TestOutputLogger.LogInformation("Succesfully instantiated base of {type}", typeof(ExampleServiceTests));

		_someStub = Stub
			.Of<ISomeThing>();
		_someStub
			.Setup(s => s.DoThing("test"))
			.Returns((string _) => true);
		_someStub
			.Setup(s => s.DoThing("test2"))
			.Returns((string _) => false);
		_someStub
			.Setup(s => s.DoString("FOO"))
			.Returns("Bar");
		_someStub
			.Setup(s => s.DoString(Parameter.Is<string>()))
			.Returns((string a) => a);
		_someStub
			.Setup(s => s.DoOtherThing())
			.Returns(() => true);
		_someStub
			.Setup(s => s.SomeAction(false))
			.Executes();
		_someStub
			.Setup(s => s.SomeAction(true))
			.Executes(() => TestOutputLogger.LogInformation("SomeAction was true"));

		_someSpy = Spy
			.On<ISomeThing>(new SomeThing());
		_someMock = Mock
			.Of<ISomeThing>();
		_outOfScopeDummy = Dummy
			.Of<IServiceOutOfScope>();

		try
		{
			_someStub.Instance.DoOtherThing();

			var one = _someStub.Instance.DoString("one");
			var bar = _someStub.Instance.DoString("FOO");
		}
		catch (Exception ex)
		{
			// TODO should throw notimplemented exception (or custom) with clear message
		}
		try
		{
			_someStub.Instance.DoThing("test2");
			_someStub.Instance.DoThing("Thing");
		}
		catch (Exception ex)
		{
			// TODO should throw notimplemented exception (or custom) with clear message
		}
		try
		{
			_someStub.Instance.TestManyParam("a", true, 8, DateTime.Now, 0);
		}
		catch (Exception ex)
		{
			// TODO should throw notimplemented exception (or custom) with clear message
		}
		_someSpy = Spy
			.On<ISomeThing>(new SomeThing());
        _someMock = Mock
			.Of<ISomeThing>();

		_someFixture = Fixture
			.ForSomeDataType()
			.WithRealName();

		_outOfScopeDummy = Dummy
			.Of<IServiceOutOfScope>();

		_asyncExampleMock = Mock
			.Of<IExampleService>();

		_asyncExampleMock
			.Setup(sut => sut.DoThing(Parameter.Is<string>()))
			.ReturnsAsync((string _) => "Test");
		_asyncExampleMock
			.Setup(sut => sut.DoAsync())
			.ExecutesAsync(() => TestOutputLogger.LogInformation("Async callback"));
	}

    public override TestCollection Tests => new(

		TestClassic(For<IExampleService>(sut => sut.DoThing).Given("Some thing").When("Condition_for_something    else").Then("Result"), async () =>
		{
			// Arrange
			_someStub
				.Setup(x => x.DoThing(Parameter.Is<string>()))
				.Returns(true);
			_someStub
				.Setup(x => x.DoOtherThing())
				.Returns(true);

			var logger = TestOutputLoggerFactory.CreateLogger<IExampleService>();
			var sut = new ExampleService(_someStub.Instance, _outOfScopeDummy, logger);

			var input = "SomeString";
			var expected = "SomeString";

			// Act
			var result = await sut.DoThing(input);

			// Assert
			result.Should().Be(expected);

			_someSpy
				.VerifyOnce(x => x.DoThing(Parameter.Is<string>()))
				.VerifyNoOtherCalls();
		}),

		TestTripleA(For<IExampleService>(sut => sut.DoThing).Given("Something").When("Condition").Then("Result"),
            Arrange(() =>
            {
				_someStub
					.Setup(x => x.DoThing(Parameter.Is<string>()))
					.Returns((string _) => true);
				_someStub
					.Setup(x => x.DoOtherThing())
					.Returns(() => true);

				var logger = TestOutputLoggerFactory.CreateLogger<IExampleService>();
				var sut = new ExampleService(_someStub.Instance, _outOfScopeDummy, logger);

                var input = "SomeString";
                var expected = "SomeString";

                return (sut, input, expected);
            }),
            Act(async (ExampleService sut, string input) =>
			
                await sut.DoThing(input)
            ),
            Assert((string result, string expected) =>
            {
                result.Should().Be(expected);

				_someSpy
					.VerifyOnce(x => x.DoThing(Parameter.Is<string>()))
					.VerifyNoOtherCalls();
			})
        )
    );
}