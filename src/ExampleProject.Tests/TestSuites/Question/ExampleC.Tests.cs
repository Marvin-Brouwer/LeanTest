using ExampleProject.Services;

using FluentAssertions;

using LeanTest;
using LeanTest.Dependencies;
using LeanTest.Tests;

using System.Collections.Generic;

namespace ExampleProject.Tests.TestSuites.Question;

public sealed class ExampleCTests : TestSuite
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

	private static IEnumerable<(int, bool)> NumbersForSomeTest()
	{
		yield return (1, true);
	}

	public ITestScenario DoThing_SomeString_DoOtherThingReturnsTrue_ReturnsExpected => Test(
		NumbersForSomeTest,
		async (number, isOkay) =>
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
		});
}