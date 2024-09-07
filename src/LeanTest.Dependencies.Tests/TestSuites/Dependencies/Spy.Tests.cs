using LeanTest.Dependencies.Definitions;
using LeanTest.Dependencies.Tests.Fixtures;


namespace LeanTest.Dependencies.Tests.TestSuites.Dependencies;

/// <summary>
/// Test suite for <see cref="Spy{TService}"/>, testing verification methods.
/// </summary>
/// <tests>
/// Tests NoCalls and NoOtherCalls
/// </tests>
public sealed partial class SpyTests : TestSuite.UnitTests
{
	public ITest VerifyNoOtherCalls_NoCalls_Passes => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(true));

		// Act
		var result = () => sut
			.VerifyNoOtherCalls();

		// Assert
		result.Should()
			.NotThrow();
	});

	public ITest VerifyNoOtherCalls_SomeCalls_Throws => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(true));

		// Act
		sut.Instance.VoidNoParameters();
		var result = () => sut
			.VerifyNoOtherCalls();

		// Assert
		result.Should()
			.ThrowExactly<ConstraintVerficationFaillure>()
			.WithMessage("No other calls were expected. However, the records still contain some unverified invocations.");
	});

	public ITest VerifyNoCalls_NoCalls_Passes => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(true));

		// Act
		var result = () => sut
			.VerifyNoCalls();

		// Assert
		result.Should()
			.NotThrow();
	});

	public ITest VerifyNoCalls_SomeCalls_Throws => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(true));

		// Act
		sut.Instance.VoidNoParameters();
		var result = () => sut
			.VerifyNoCalls();

		// Assert
		result.Should()
			.ThrowExactly<ConstraintVerficationFaillure>()
			.WithMessage("No calls were expected. However, at least some invocations were recorded.");
	});

	public ITest SpyOnOutVariable_ShouldSetVariableValue => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(true));

		// Act
		sut.Instance.VoidWithOutReference(out var someString);

		// Assert
		someString
			.Should()
			.BeEquivalentTo("referenced");
	});

	public ITest SpyOnRefVariable_ShouldSetVariableValue => Test(() =>
	{
		// Arrange
		var sut = Spy.On<IExampleService>(new ExampleService(true));

		// Act
		string someString = "before reference";
		sut.Instance.VoidWithReference(ref someString);

		// Assert
		someString
			.Should()
			.BeEquivalentTo("referenced");
	});

	private sealed class ExampleService : IExampleService
	{
		private readonly bool _success;

		public ExampleService(bool success)
		{
			_success = success;
		}

		public T GenericNoParameters<T>()
		{
			if (!_success) throw new NotImplementedException();
			return default(T)!;
		}

		public T GenericWithGenericParameters<T, U>(U something, int someNumber)
		{
			if (!_success) throw new NotImplementedException();
			return default(T)!;
		}

		public T GenericWithParameters<T>(bool someBoolean)
		{
			if (!_success) throw new NotImplementedException();
			return default(T)!;
		}

		public int ReturnsNoParameters()
		{
			if (!_success) throw new NotImplementedException();
			return 99;
		}

		public int ReturnsWithGenericParameters<T>(T something, string someString)
		{
			if (!_success) throw new NotImplementedException();
			return 99;
		}

		public int ReturnsWithParameters(bool someBoolean)
		{
			if (!_success) throw new NotImplementedException();
			return 99;
		}

		public void VoidNoParameters()
		{
			if (!_success) throw new NotImplementedException();
		}

		public void VoidWithGenericParameters<T>(T something, bool someBoolean)
		{
			if (!_success) throw new NotImplementedException();
		}

		public void VoidWithOutReference(out string someString)
		{
			if (!_success) throw new NotImplementedException();
			someString = "referenced";
		}

		public void VoidWithParameters(string? someString)
		{
			if (!_success) throw new NotImplementedException();
		}

		public void VoidWithReadonlyReference(in string someString)
		{
			if (!_success) throw new NotImplementedException();
			_ = someString;
		}

		public void VoidWithReference(ref string someString)
		{
			if (!_success) throw new NotImplementedException();
			someString = "referenced";
		}
	}
}

