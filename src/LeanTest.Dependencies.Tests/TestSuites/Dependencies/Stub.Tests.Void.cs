using FluentAssertions;

using LeanTest.Dependencies.Tests.Fixtures;

using System;

namespace LeanTest.Dependencies.Tests.TestSuites.Dependencies;

public sealed record StubTests : TestSuite<IExampleService>
{
	private readonly Stub<IExampleService> _someStub;

	public StubTests()
	{
		_someStub = Stub.Of<IExampleService>();
	}

	public override TestCollection Tests => new(

		TestClassic(For(sut => sut.VoidNoParameters).Given("MethodCallAttempted").When("Configured").Then("Successful"), () =>
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
		TestClassic(For(sut => sut.VoidNoParameters).Given("MethodCallAttempted").When("NotConfigured").Then("Throws"), () =>
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
		TestClassic(For(sut => sut.VoidNoParameters).Given("MethodCallAttempted").When("ConfiguredToThrow").Then("Throws"), () =>
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
		}),

		TestClassic(For(sut => sut.VoidWithParameters).Given("MethodCallAttempted").When("Configured").Then("Successful"), () =>
		{
			// Arrange
			_someStub
				.Setup(sut => sut.VoidWithParameters(Parameter.Is<string>()))
				.Executes();

			var sut = _someStub.Instance;

			// Act
			var result = () => sut.VoidWithParameters("test");

			// Assert
			result.Should().NotThrow();
		}),
		TestClassic(For(sut => sut.VoidWithParameters).Given("MethodCallAttempted").When("NotConfigured").Then("Throws"), () =>
		{
			// Arrange
			_someStub
				.Setup(sut => sut.VoidWithParameters(Parameter.Is<string>()))
				.Executes();

			var sut = _someStub.Instance;

			// Act
			var result = () => sut.VoidWithParameters("test");

			// Assert
			result.Should().ThrowExactly<NotSupportedException>();
		}),
		TestClassic(For(sut => sut.VoidWithParameters).Given("MethodCallAttempted").When("ConfiguredToThrow").Then("Throws"), () =>
		{
			// Arrange
			_someStub
				.Setup(sut => sut.VoidWithParameters(Parameter.Is<string>()))
				.Executes(() => throw new Exception());

			var sut = _someStub.Instance;

			// Act
			var result = () => sut.VoidWithParameters("test");

			// Assert
			result.Should().ThrowExactly<Exception>();
		}),

		TestClassic(For(sut => sut.VoidWithGenericParameters<int>).Given("MethodCallAttempted").When("Configured").Then("Successful"), () =>
		{
			// Arrange
			_someStub
				.Setup(sut => sut.VoidWithGenericParameters(Parameter.Is<int>(), Parameter.Is<bool>()))
				.Executes();

			var sut = _someStub.Instance;

			// Act
			var result = () => sut.VoidWithGenericParameters(1, true);

			// Assert
			result.Should().NotThrow();
		}),
		TestClassic(For(sut => sut.VoidWithGenericParameters<int>).Given("MethodCallAttempted").When("NotConfigured").Then("Throws"), () =>
		{
			// Arrange
			_someStub
				.Setup(sut => sut.VoidWithGenericParameters(Parameter.Is<int>(), Parameter.Is<bool>()))
				.Executes();

			var sut = _someStub.Instance;

			// Act
			var result = () => sut.VoidWithGenericParameters(1, true);

			// Assert
			result.Should().ThrowExactly<NotSupportedException>();
		}),
		TestClassic(For(sut => sut.VoidWithGenericParameters<int>).Given("MethodCallAttempted").When("ConfiguredToThrow").Then("Throws"), () =>
		{
			// Arrange
			_someStub
				.Setup(sut => sut.VoidWithGenericParameters(Parameter.Is<int>(), Parameter.Is<bool>()))
				.Executes(() => throw new Exception());

			var sut = _someStub.Instance;

			// Act
			var result = () => sut.VoidWithGenericParameters(1, true);

			// Assert
			result.Should().ThrowExactly<Exception>();
		})
	);
}