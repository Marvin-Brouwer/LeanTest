

using LeanTest.Dependencies.Definitions;
using LeanTest.Dependencies.Tests.Fixtures;


namespace LeanTest.Dependencies.Tests.TestSuites.Dependencies;

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

		public void VoidWithParameters(string someString)
		{
			if (!_success) throw new NotImplementedException();
		}
	}
}

