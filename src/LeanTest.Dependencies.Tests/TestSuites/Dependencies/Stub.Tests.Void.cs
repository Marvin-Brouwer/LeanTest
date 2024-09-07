using LeanTest.Dependencies.Configuration;
using LeanTest.Dependencies.Providers;
using LeanTest.Dependencies.Tests.Fixtures;
using LeanTest.Dynamic.Invocation;

using System.Diagnostics;
using System.Reflection;

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

	public ITest VoidNoParameters_NotConfigured_Throws => Test(() =>
	{
		// Arrange
		var sut = _someStub.Instance;

		// Act
		var result = () => sut.VoidNoParameters();

		// Assert
		result.Should()
			.ThrowExactly<InvocationNotFoundException>()
			.WithMessage(
				"Requested method \"VoidNoParameters\" was not configured with the requested parameters." + Environment.NewLine +
				"Please make sure to configure your mocks, fakes, stubs, and spies."
			);
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
			.Setup(sut => sut.VoidWithParameters(Parameters<string>().AnyValue))
			.Executes();

		var sut = _someStub.Instance;

		// Act
		var result = () => sut.VoidWithParameters("Test");

		// Assert
		result.Should()
			.NotThrow();
	});

	public ITest VoidWithParameters_ConfiguredReference_Returns => Test(() =>
	{
		// Arrange
		var referenceString = "ref";
		_someStub
			.Setup(sut => sut.VoidWithReference(ref Parameters<string>().AnyValue))
			.Executes((out string x) =>
			{
				x = "Test";
			});

		var sut = _someStub.Instance;

		// Act
		sut.VoidWithReference(ref referenceString);

		// Assert
		referenceString
			.Should()
			.BeEquivalentTo("Test");
	});

	public ITest VoidWithParameters_ConfiguredReadonlyReference_Returns => Test(() =>
	{
		// Arrange
		var referenceString = "ref";
		_someStub
			.Setup(sut => sut.VoidWithReadonlyReference(in Parameters<string>().AnyValue))
			.Executes();

		var sut = _someStub.Instance;

		// Act
		var result = () => sut.VoidWithReadonlyReference(in referenceString);

		// Assert
		result.Should()
			.NotThrow();
	});

    public ITest VoidWithParameters_ConfiguredOutReference_Returns => Test(() =>
    {
        // Arrange
        _someStub
            .Setup(sut => sut.VoidWithOutReference(out Parameters<string>().AnyValue))
            .Executes((out string x) =>
			{
				x = "Test";
			});

		var sut = _someStub.Instance;
		string referenceString;

		// Act
		sut.VoidWithOutReference(out referenceString);

        // Assert
		referenceString
			.Should()
			.BeEquivalentTo("Test");
    });

    public ITest VoidWithParameters_ConfiguredNull_Returns => Test(() =>
	{
		// Arrange
		_someStub
			.Setup(sut => sut.VoidWithParameters(null))
			.Executes();

		var sut = _someStub.Instance;

		// Act
		var result = () => sut.VoidWithParameters(null);

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
			.ThrowExactly<InvocationNotFoundException>();
	});

	public ITest VoidWithParameters_ConfiguredToThrow_Throws => Test(() =>
	{
		// Arrange
		_someStub
			.Setup(sut => sut.VoidWithParameters(Parameters<string>().AnyValue))
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
			.Setup(sut => sut.VoidWithGenericParameters(Parameters<int>().AnyValue, Parameters<bool>().AnyValue))
			.Executes();

		var sut = _someStub.Instance;

		// Act
		var result = () => sut.VoidWithGenericParameters(2, true);

		// Assert
		result.Should()
			.NotThrow();
	});

	public ITest VoidWithGenericParameters_ConfiguredForAnyType_Returns => Test(() =>
	{
		// Arrange
		_someStub
			.Setup(sut => sut.VoidWithGenericParameters(Parameter.Matches(v => v != null && v.Equals(2)), Parameters<bool>().AnyValue))
			.Executes();

		var sut = _someStub.Instance;

		// Act
		var result = () => sut.VoidWithGenericParameters(2, true);

		// Assert
		result.Should()
			.NotThrow();
	});

	public ITest VoidWithGenericParameters_ConfiguredForAnyValue_Returns => Test(() =>
	{
		// Arrange
		_someStub
			.Setup(sut => sut.VoidWithGenericParameters(Parameter.Any, Parameters<bool>().AnyValue))
			.Executes();

		var sut = _someStub.Instance;

		// Act
		var result = () => sut.VoidWithGenericParameters(2, true);

		// Assert
		result.Should()
			.NotThrow();
	});

	public ITest VoidWithGenericParameters_ConfiguredDefault_Returns => Test(() =>
	{
		// Arrange
		_someStub
			.Setup(sut => sut.VoidWithGenericParameters<int>(default, Parameters<bool>().AnyValue))
			.Executes();

		var sut = _someStub.Instance;

		// Act
		var result = () => sut.VoidWithGenericParameters(0, true);

		// Assert
		result.Should()
			.NotThrow();
	});

	public ITest VoidWithGenericParameters_ConfiguredNull_Returns => Test(() =>
	{
		// Arrange
		_someStub
			.Setup(sut => sut.VoidWithGenericParameters(Parameters<int?>().Null, Parameters<bool>().AnyValue))
			.Executes();

		var sut = _someStub.Instance;

		// Act
		var result = () => sut.VoidWithGenericParameters<int?>(null, true);

		// Assert
		result.Should()
			.NotThrow();
	});

	public ITest VoidWithGenericParameters_ConfiguredNotNull_Returns => Test(() =>
	{
		// Arrange
		_someStub
			.Setup(sut => sut.VoidWithGenericParameters(Parameters<int?>().NotNull, Parameters<bool>().AnyValue))
			.Executes();

		var sut = _someStub.Instance;

		// Act
		var result = () => sut.VoidWithGenericParameters<int?>(0, true);

		// Assert
		result.Should()
			.NotThrow();
	});

	public ITest VoidWithGenericParameters_ConfiguredExtension_Returns => Test(() =>
	{
		// Arrange
		_someStub
			.Setup(sut => sut.VoidWithGenericParameters(Parameter.OneSecond(), Parameters<bool>().AnyValue))
			.Executes();

		var sut = _someStub.Instance;

		// Act
		var result = () => sut.VoidWithGenericParameters(TimeSpan.FromMilliseconds(1000), true);

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
			.ThrowExactly<InvocationNotFoundException>();
	});

	public ITest VoidWithGenericParameters_ConfiguredToThrow_Throws => Test(() =>
	{
		// Arrange
		_someStub
			.Setup(sut => sut.VoidWithGenericParameters(Parameters<int>().AnyValue, Parameters<bool>().AnyValue))
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

/// <summary>
/// This is here to illustrate, and test, how one would extend the <see cref="IDynamicParameterExpressionProvider"/>
/// </summary>
internal static class ParameterExtensions
{
	/// <summary>
	/// When parameter matches a TimeSpan of one second
	/// </summary>
	[ParameterMatch<OneSecondMatcher>]
	public static TimeSpan OneSecond(this IDynamicParameterExpressionProvider parameter)
	{
		_ = parameter;
		return default;
	}

	/// <summary>
	/// Matcher for: When parameter matches a TimeSpan of one second
	/// </summary>
	[DebuggerDisplay("When parameter matches a TimeSpan of one second")]
	internal class OneSecondMatcher(ParameterInfo parameterInfo) : ConfiguredParameterExtension(parameterInfo)
	{
		public override bool MatchesRequirements<T>(T? parameterValue) where T : default
		{
			return TimeSpan.FromSeconds(1).Equals(parameterValue);
		}
	}
}