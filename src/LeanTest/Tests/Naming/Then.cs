using System.Text.RegularExpressions;

namespace LeanTest.Tests.Naming;

public readonly record struct Then(string Value) : ITestName
{
	private readonly ITestNamePart _parent = default!;

	public Then(When when, string value) : this(value)
	{
		_parent = when;
	}
	public Then(Given given, string value) : this(value)
	{
		_parent = given;
	}

	public string Name => String.Join("_", _parent.Name, Value);


	private static readonly Regex WhiteSpacePattern = new (@"\s+",
		RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase
	);
	public string GetNormalizedName()
	{
		return WhiteSpacePattern.Replace(Name, "_");
	}
}