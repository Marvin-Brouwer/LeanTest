using ExampleProject.Services;

using FluentAssertions;

using LeanTest;
using LeanTest.Dependencies;
using LeanTest.Tests;

namespace ExampleProject.Tests.TestSuites.Services;

public sealed record ExampleCTests : TestSuite
{
    private readonly Stub<ISomeThing> _someStub;

	public ExampleCTests()
    {
		_someStub = Stub
			.Of<ISomeThing>();
		_someStub
			.Setup(s => s.DoThing("test"))
			.Returns(true);
		_someStub
			.Setup(s => s.DoThing("test2"))
			.Returns(false);
	}

	public ITestScenario DoThing_SomeString_DoOtherThingReturnsTrue_ReturnsExpected => TestClassic(
		async () =>
		{
			// Arrange
			_someStub
				.Setup(x => x.DoThing(Parameter.Is<string>()))
				.Returns(true);
			_someStub
				.Setup(x => x.DoOtherThing())
				.Returns(true);

			var sut = new ExampleService(_someStub.Instance);
			var input = "SomeString";
			var expected = "SomeString";

			// Act
			var result = await sut.DoThing(input);

			// Assert
			result.Should().Be(expected);
		}
	);
}