using ExampleProject.Services;

using FluentAssertions;

using LeanTest;
using LeanTest.Dependencies;

namespace ExampleProject.Tests.TestSuites.Question;

public sealed class ExampleBTests : TestSuite
{
	private readonly Stub<ISomeThing> _someStub;

	public ExampleBTests()
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
		TestTripleA(For<ExampleService>(sut => sut.DoThing)
			.Given("SomeString").When("DoOtherThingReturnsTrue").Then("ReturnsExpected"),
			Arrange(() =>
			{
				_someStub
				.Setup(x => x.DoThing(Parameter.Is<string>()))
				.Returns(true);
				_someStub
					.Setup(x => x.DoOtherThing())
					.Returns(true);

				var sut = new ExampleService(_someStub.Instance);
				var input = "SomeString";
				var expected = "SomeString";

				return (sut, input, expected);
			}),
			Act(async (ExampleService sut, string input) =>
			{
				return await sut.DoThing(input);
			}),
			Assert((string input, string expected) =>
			{
				input.Should().Be(expected);
			})
		)
	);
}