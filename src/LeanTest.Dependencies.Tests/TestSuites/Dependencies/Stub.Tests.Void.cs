using FluentAssertions;

using LeanTest.Dependencies.Tests.Fixtures;
using LeanTest.Tests;

using System;

namespace LeanTest.Dependencies.Tests.TestSuites.Dependencies;

public sealed class StubTests : TestSuite
{
	private readonly Stub<IExampleService> _someStub;

	public StubTests()
	{
		_someStub = Stub.Of<IExampleService>();
	}

	#region NoParameters

	public ITestScenario VoidNoParameters_Configured_Returns => Test(() =>
	{
		// Arrange
		_someStub
			.Setup(sut => sut.VoidNoParameters())
			.Executes();

		var sut = _someStub.Instance;

		// Act
		var result = () => sut.VoidNoParameters();

		// Assert
		result.Should()
			.NotThrow();
	});

	public ITestScenario VoidNoParameters_NotConfigured_Throws => Test(() =>
	{
		// Arrange
		var sut = _someStub.Instance;

		// Act
		var result = () => sut.VoidNoParameters();

		// Assert
		result.Should()
			.ThrowExactly<NotSupportedException>();
	});

	public ITestScenario VoidNoParameters_ConfiguredToThrow_Throws => Test(() =>
	{
		// Arrange
		_someStub
			.Setup(sut => sut.VoidNoParameters())
			.Throws(new DataMisalignedException());

		var sut = _someStub.Instance;

		// Act
		var result = () => sut.VoidNoParameters();

		// Assert
		result.Should()
			.ThrowExactly<DataMisalignedException>();
	});

	#endregion

	#region WithParameters

	public ITestScenario VoidWithParameters_Configured_Returns => Test(() =>
	{
		// Arrange
		_someStub
			.Setup(sut => sut.VoidWithParameters(Parameter.Is<string>()))
			.Executes();

		var sut = _someStub.Instance;

		// Act
		var result = () => sut.VoidWithParameters("Test");

		// Assert
		result.Should()
			.NotThrow();
	});

	public ITestScenario VoidWithParameters_NotConfigured_Throws => Test(() =>
	{
		// Arrange
		var sut = _someStub.Instance;

		// Act
		var result = () => sut.VoidWithParameters("Test");

		// Assert
		result.Should()
			.ThrowExactly<NotSupportedException>();
	});

	public ITestScenario VoidWithParameters_ConfiguredToThrow_Throws => Test(() =>
	{
		// Arrange
		_someStub
			.Setup(sut => sut.VoidWithParameters(Parameter.Is<string>()))
			.Throws(new DataMisalignedException());

		var sut = _someStub.Instance;

		// Act
		var result = () => sut.VoidWithParameters("Test");

		// Assert
		result.Should()
			.ThrowExactly<DataMisalignedException>();
	});

	#endregion

	#region GenericParameters

	public ITestScenario VoidWithGenericParameters_Configured_Returns => Test(() =>
	{
		// Arrange
		_someStub
			.Setup(sut => sut.VoidWithGenericParameters<int>(Parameter.Is<int>(), Parameter.Is<bool>()))
			.Executes();

		var sut = _someStub.Instance;

		// Act
		var result = () => sut.VoidWithGenericParameters(2, true);

		// Assert
		result.Should()
			.NotThrow();
	});

	public ITestScenario VoidWithGenericParameters_NotConfigured_Throws => Test(() =>
	{
		// Arrange
		var sut = _someStub.Instance;

		// Act
		var result = () => sut.VoidWithGenericParameters(2, true);

		// Assert
		result.Should()
			.ThrowExactly<NotSupportedException>();
	});

	public ITestScenario VoidWithGenericParameters_ConfiguredToThrow_Throws => Test(() =>
	{
		// Arrange
		_someStub
			.Setup(sut => sut.VoidWithGenericParameters<int>(Parameter.Is<int>(), Parameter.Is<bool>()))
			.Throws(new DataMisalignedException());

		var sut = _someStub.Instance;

		// Act
		var result = () => sut.VoidWithGenericParameters(2, true);

		// Assert
		result.Should()
			.ThrowExactly<DataMisalignedException>();
	});

	#endregion
}