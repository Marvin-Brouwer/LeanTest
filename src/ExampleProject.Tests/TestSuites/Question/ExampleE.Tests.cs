using ExampleProject.Services;

using FluentAssertions;

using LeanTest;

namespace ExampleProject.Tests.TestSuites.Question;

public sealed class ExampleETests : TestSuite
{
	public ExampleETests()
	{
		var someStub = Stub
			.Of<ISomeThing>();
		someStub
			.Setup(s => s.DoThing("test"))
			.Returns(true);
		someStub
			.Setup(s => s.DoThing("test2"))
			.Returns(false);

		TestClassic("DoThing_SomeString_DoOtherThingReturnsTrue_ReturnsExpected", async () =>
		{
			// Arrange
			someStub
				.Setup(x => x.DoThing(Parameter.Is<string>()))
				.Returns(true);
			someStub
				.Setup(x => x.DoOtherThing())
				.Returns(true);

			var sut = new ExampleService(someStub.Instance);
			var input = "SomeString";
			var expected = "SomeString";

			// Act
			var result = await sut.DoThing(input);

			// Assert
			result.Should().Be(expected);
		});
	}

}