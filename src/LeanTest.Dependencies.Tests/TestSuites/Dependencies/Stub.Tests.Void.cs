using FluentAssertions;

using LeanTest.Dependencies.Tests.Fixtures;
using LeanTest.Tests;

using System;

namespace LeanTest.Dependencies.Tests.TestSuites.Dependencies;

public sealed class StubTests : TestSuite.UnitTests
{
	private readonly Stub<IExampleService> _someStub;

	public StubTests()
	{
		_someStub = Stub.Of<IExampleService>();
	}

	#region NoParameters

	public ITest VoidNoParameters_Configured_Returns => Test(() =>
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

	public ITest VoidNoParameters_NotConfigured_Throws => Test(async () =>
	{
		// Arrange
		var sut = _someStub.Instance;

		// Act
		var result = () => sut.VoidNoParameters();

		// Assert
		result.Should()
			.ThrowExactly<NotSupportedException>();
	});

	public ITest VoidNoParameters_ConfiguredToThrow_Throws => Test(() =>
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

	public ITest VoidWithParameters_Configured_Returns => Test(() =>
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

	public ITest VoidWithParameters_NotConfigured_Throws => Test(() =>
	{
		// Arrange
		var sut = _someStub.Instance;

		// Act
		var result = () => sut.VoidWithParameters("Test");

		// Assert
		result.Should()
			.ThrowExactly<NotSupportedException>();
	});

	public ITest VoidWithParameters_ConfiguredToThrow_Throws => Test(() =>
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

	public ITest VoidWithGenericParameters_Configured_Returns => Test(() =>
	{
		// Arrange
		_someStub
			.Setup(sut => sut.VoidWithGenericParameters(Parameter.Is<int>(), Parameter.Is<bool>()))
			.Executes();

		var sut = _someStub.Instance;

		// Act
		var result = () => sut.VoidWithGenericParameters(2, true);

		// Assert
		result.Should()
			.NotThrow();
	});

	public ITest VoidWithGenericParameters_NotConfigured_Throws => Test(() =>
	{
		// Arrange
		var sut = _someStub.Instance;

		// Act
		var result = () => sut.VoidWithGenericParameters(2, true);

		// Assert
		result.Should()
			.ThrowExactly<NotSupportedException>();
	});

	public ITest VoidWithGenericParameters_ConfiguredToThrow_Throws => Test(() =>
	{
		// Arrange
		_someStub
			.Setup(sut => sut.VoidWithGenericParameters(Parameter.Is<int>(), Parameter.Is<bool>()))
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