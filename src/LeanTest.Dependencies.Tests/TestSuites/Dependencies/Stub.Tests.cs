using FluentAssertions;

using LeanTest;
using LeanTest.Dependencies;
using LeanTest.Dependencies.Tests.Fixtures;

using System;

namespace ExampleProject.Tests.TestSuites.Services;

public sealed record StubTests : TestSuite<IExampleService>
{
    private readonly IStub<IExampleService> _someStub;

    public StubTests()
    {
		_someStub = Stub.Of<IExampleService>();
    }

    public override TestCollection Tests => new(

		TestClassic(sut => sut.VoidNoParameters, Given("MethodCallAttempted").When("Configured").Then("Successful"), () =>
		{
			// Arrange
			_someStub
				.Setup(sut => sut.VoidNoParameters())
				.Executes();

			var sut = _someStub.Instance;

			// Act
			var result = () => sut.VoidNoParameters();

			// Assert
			result.Should().NotThrow();
		}),
		TestClassic(sut => sut.VoidNoParameters, Given("MethodCallAttempted").When("NotConfigured").Then("Throws"), () =>
		{
			// Arrange
			_someStub
				.Setup(sut => sut.VoidNoParameters())
				.Executes();

			var sut = _someStub.Instance;

			// Act
			var result = () => sut.VoidNoParameters();

			// Assert
			result.Should().ThrowExactly<NotSupportedException>();
		}),
		TestClassic(sut => sut.VoidNoParameters, Given("MethodCallAttempted").When("ConfiguredToThrow").Then("Throws"), () =>
		{
			// Arrange
			_someStub
				.Setup(sut => sut.VoidNoParameters())
				.Executes(() => throw new Exception());

			var sut = _someStub.Instance;

			// Act
			var result = () => sut.VoidNoParameters();

			// Assert
			result.Should().ThrowExactly<Exception>();
		})
	);
}