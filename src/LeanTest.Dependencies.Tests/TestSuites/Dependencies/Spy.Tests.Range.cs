using LeanTest.Dependencies.Definitions;
using LeanTest.Dependencies.Tests.Fixtures;

namespace LeanTest.Dependencies.Tests.TestSuites.Dependencies;


/// <inheritdoc cref="SpyTests"/>
/// <tests>
/// Tests range invocations, like Between and AtMost
/// </tests>
public sealed partial class SpyTests : TestSuite.UnitTests
{
	#region Between

	public ITest VerifyBetweenZeroAndTwoExclusive_InvokedOnce_Passes => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(true));

		// Act
		sut.Instance.VoidNoParameters();

		// Assert
		sut
			.VerifyBetween(0, 2, spy => spy.VoidNoParameters())
			.VerifyNoOtherCalls();
	});

	public ITest Verify_BetweenOneAndTwoInclusive_InvokedOnce_Passes => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(true));

		// Act
		sut.Instance.VoidNoParameters();

		// Assert
		sut
			.Verify(Times.Between(1, 2, true), spy => spy.VoidNoParameters())
			.VerifyNoOtherCalls();
	});

	public ITest VerifyBetweenOneAndTwoInclusive_InvokedOnce_Passes => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(true));

		// Act
		sut.Instance.VoidNoParameters();

		// Assert
		sut
			.VerifyBetween(1, 2, true, spy => spy.VoidNoParameters())
			.VerifyNoOtherCalls();
	});

	public ITest Verify_BetweenOneAndTwoInclusive_InvokedNever_Throws => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(true));

		// Act
		var result = () => sut.Verify(Times.Between(1, 2, true), spy => spy.VoidNoParameters());

		// Assert
		result.Should()
			.ThrowExactly<ConstraintVerficationFaillure>()
			.WithMessage("IExampleService.VoidNoParameters was expected to be called between \"1\" and \"2\" time(s) (inclusive). However, \"0\" were counted.");
	});

	public ITest Verify_BetweenOneAndTwoInclusive_InvokedThreeTimes_Throws => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(true));
		sut.Instance.VoidNoParameters();
		sut.Instance.VoidNoParameters();
		sut.Instance.VoidNoParameters();

		// Act
		var result = () => sut.Verify(Times.Between(1, 2, true), spy => spy.VoidNoParameters());

		// Assert
		result.Should()
			.ThrowExactly<ConstraintVerficationFaillure>()
			.WithMessage("IExampleService.VoidNoParameters was expected to be called between \"1\" and \"2\" time(s) (inclusive). However, \"3\" were counted.");
	});

	public ITest Verify_BetweenOneAndTwoInclusive_InvokedTwice_Passes => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(true));

		// Act
		sut.Instance.VoidNoParameters();
		sut.Instance.VoidNoParameters();

		// Assert
		sut
			.Verify(Times.Between(1, 2, true), spy => spy.VoidNoParameters())
			.VerifyNoOtherCalls();
	});

	public ITest Verify_BetweenOneAndTwoExclusive_InvokedTwice_Throws => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(true));
		sut.Instance.VoidNoParameters();
		sut.Instance.VoidNoParameters();

		// Act
		var result = () => sut.Verify(Times.Between(1, 2, false), spy => spy.VoidNoParameters());

		// Assert
		result.Should()
			.ThrowExactly<ConstraintVerficationFaillure>()
			.WithMessage("IExampleService.VoidNoParameters was expected to be called between \"1\" and \"2\" time(s) (exclusive). However, \"2\" were counted.");
	});

	public ITest VerifyBetween_IncorrectConfiguration_Throws => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(true));

		// Act
		var result = () => sut.VerifyBetween(9, 3, spy => spy.VoidNoParameters());

		// Assert
		result.Should()
			.ThrowExactly<ArgumentException>()
			.WithMessage("Required input mostAmountOfTimes cannot be less than leastAmountOfTimes. (Parameter 'mostAmountOfTimes')");
	});

	#endregion Between

	#region AtMost

	public ITest VerifyAtMostTwo_InvokedOnce_Passes => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(true));

		// Act
		sut.Instance.VoidNoParameters();

		// Assert
		sut
			.VerifyAtMost(2, spy => spy.VoidNoParameters())
			.VerifyNoOtherCalls();
	});

	public ITest Verify_AtMostTwo_InvokedOnce_Passes => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(true));

		// Act
		sut.Instance.VoidNoParameters();

		// Assert
		sut
			.Verify(Times.AtMost(2), spy => spy.VoidNoParameters())
			.VerifyNoOtherCalls();
	});

	public ITest VerifyAtMostTwo_InvokedThreeTimes_Throws => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(true));
		sut.Instance.VoidNoParameters();
		sut.Instance.VoidNoParameters();
		sut.Instance.VoidNoParameters();

		// Act
		var result = () => sut.VerifyAtMost(2, spy => spy.VoidNoParameters());

		// Assert
		result.Should()
			.ThrowExactly<ConstraintVerficationFaillure>()
			.WithMessage("IExampleService.VoidNoParameters was expected to be called at most \"2\" time(s). However, \"3\" were counted.");
	});

	public ITest Verify_AtMostTwo_InvokedThreeTimes_Throws => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(true));
		sut.Instance.VoidNoParameters();
		sut.Instance.VoidNoParameters();
		sut.Instance.VoidNoParameters();

		// Act
		var result = () => sut.Verify(Times.AtMost(2), spy => spy.VoidNoParameters());

		// Assert
		result.Should()
			.ThrowExactly<ConstraintVerficationFaillure>()
			.WithMessage("IExampleService.VoidNoParameters was expected to be called at most \"2\" time(s). However, \"3\" were counted.");
	});

	public ITest Verify_AtMostTwo_InvokedNever_Passes => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(true));

		// Act
		// DoNothing()

		// Assert
		sut
			.Verify(Times.AtMost(2), spy => spy.VoidNoParameters())
			.VerifyNoOtherCalls();

	});

	#endregion Between

	#region AtLeast

	public ITest VerifyAtLeastTwo_InvokedTwice_Passes => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(true));

		// Act
		sut.Instance.VoidNoParameters();
		sut.Instance.VoidNoParameters();

		// Assert
		sut
			.VerifyAtLeast(2, spy => spy.VoidNoParameters())
			.VerifyNoOtherCalls();
	});

	public ITest Verify_AtLeastTwo_InvokedTwice_Passes => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(true));

		// Act
		sut.Instance.VoidNoParameters();
		sut.Instance.VoidNoParameters();

		// Assert
		sut
			.Verify(Times.AtLeast(2), spy => spy.VoidNoParameters())
			.VerifyNoOtherCalls();
	});

	public ITest VerifyAtLeastTwo_InvokedOnce_Throws => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(true));
		sut.Instance.VoidNoParameters();

		// Act
		var result = () => sut.VerifyAtLeast(2, spy => spy.VoidNoParameters());

		// Assert
		result.Should()
			.ThrowExactly<ConstraintVerficationFaillure>()
			.WithMessage("IExampleService.VoidNoParameters was expected to be called at least \"2\" time(s). However, \"1\" were counted.");
	});

	public ITest Verify_AtLeastTwo_InvokedOnce_Throws => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(true));
		sut.Instance.VoidNoParameters();

		// Act
		var result = () => sut.Verify(Times.AtLeast(2), spy => spy.VoidNoParameters());

		// Assert
		result.Should()
			.ThrowExactly<ConstraintVerficationFaillure>()
			.WithMessage("IExampleService.VoidNoParameters was expected to be called at least \"2\" time(s). However, \"1\" were counted.");
	});

	#endregion Between
}

