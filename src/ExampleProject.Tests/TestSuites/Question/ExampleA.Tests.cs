using ExampleProject.Services;

using FluentAssertions;

using LeanTest;
using LeanTest.Dependencies;

namespace ExampleProject.Tests.TestSuites.Services;

public sealed record ExampleATests : TestSuite
{
    private readonly Stub<ISomeThing> _someStub;

	public ExampleATests()
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

    public override TestCollection Tests => new(
		TestClassic(For<ExampleService>(sut => sut.DoThing)
			.Given("SomeString").When("DoOtherThingReturnsTrue").Then("ReturnsExpected"), async () =>
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
		})
    );
}