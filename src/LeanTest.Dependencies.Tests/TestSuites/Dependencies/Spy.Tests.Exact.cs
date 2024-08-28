using LeanTest.Dependencies.Definitions;
using LeanTest.Dependencies.Tests.Fixtures;

namespace LeanTest.Dependencies.Tests.TestSuites.Dependencies;


/// <inheritdoc cref="SpyTests"/>
/// <tests>
/// Tests exact invocations, like Once and Never
/// </tests>
public sealed partial class SpyTests : TestSuite.UnitTests
{
	#region Once

	public ITest VerifyOnce_InvokedOnce_Passes => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(true));

		// Act
		sut.Instance.VoidNoParameters();

		// Assert
		sut
			.VerifyOnce(spy => spy.VoidNoParameters())
			.VerifyNoOtherCalls();
	});

	public ITest Verify_Once_InvokedOnce_Passes => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(true));

		// Act
		sut.Instance.VoidNoParameters();

		// Assert
		sut
			.Verify(Times.Once, spy => spy.VoidNoParameters())
			.VerifyNoOtherCalls();
	});

	public ITest VerifyOnce_InvokedOnce_Faillure_Passes => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(false));

		// Act
		try
		{
			sut.Instance.VoidNoParameters();
		}
		catch
		{
			//
		}

		// Assert
		sut
			.VerifyOnce(spy => spy.VoidNoParameters())
			.VerifyNoOtherCalls();
	});

	public ITest VerifyOnce_InvokedTwice_Throws => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(true));

		// Act
		sut.Instance.VoidNoParameters();
		sut.Instance.VoidNoParameters();
		var result = () => sut
			.VerifyOnce(spy => spy.VoidNoParameters());

		// Assert
		result.Should()
			.ThrowExactly<ConstraintVerficationFaillure>()
			.WithMessage("IExampleService.VoidNoParameters was expected to be called once. However, \"2\" were counted.");
	});

	#endregion

	#region Exactly

	public ITest VerifyExactly_InvokedOnce_Passes => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(true));

		// Act
		sut.Instance.VoidNoParameters();

		// Assert
		sut
			.VerifyExactly(1, spy => spy.VoidNoParameters())
			.VerifyNoOtherCalls();
	});

	public ITest Verify_Exactly_InvokedOnce_Passes => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(true));

		// Act
		sut.Instance.VoidNoParameters();

		// Assert
		sut
			.Verify(Times.Exactly(1), spy => spy.VoidNoParameters())
			.VerifyNoOtherCalls();
	});

	public ITest VerifyExactly_Twice_InvokedOnce_Throws => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(true));

		// Act
		sut.Instance.VoidNoParameters();
		var result = () => sut
			.VerifyExactly(2, spy => spy.VoidNoParameters());

		// Assert
		result.Should()
			.ThrowExactly<ConstraintVerficationFaillure>()
			.WithMessage("IExampleService.VoidNoParameters was expected to be called excactly \"2\" time(s). However, \"1\" were counted.");
	});

	public ITest VerifyExactly_Zero_InvokedOnce_Throws => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(true));

		// Act
		sut.Instance.VoidNoParameters();
		var result = () => sut
			.VerifyExactly(0, spy => spy.VoidNoParameters());

		// Assert
		result.Should()
			.ThrowExactly<ConstraintVerficationFaillure>()
			.WithMessage("IExampleService.VoidNoParameters was expected to be called excactly \"0\" time(s). However, \"1\" were counted.");
	});

	#endregion

	#region Never

	public ITest VerifyNever_InvokedOnce_Throws => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(true));

		// Act
		sut.Instance.VoidNoParameters();
		var result = () => sut
			.Verify(Times.Never, spy => spy.VoidNoParameters());

		// Assert
		result.Should()
			.ThrowExactly<ConstraintVerficationFaillure>()
			.WithMessage("IExampleService.VoidNoParameters was expected to never be called. However, \"1\" were counted.");
	});

	public ITest Verify_Never_InvokedOnce_Throws => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(true));

		// Act
		sut.Instance.VoidNoParameters();
		var result = () => sut
			.VerifyNever(spy => spy.VoidNoParameters());

		// Assert
		result.Should()
			.ThrowExactly<ConstraintVerficationFaillure>()
			.WithMessage("IExampleService.VoidNoParameters was expected to never be called. However, \"1\" were counted.");
	});

	#endregion
}

