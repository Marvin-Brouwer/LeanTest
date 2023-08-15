using LeanTest.Dependencies.Definitions;

namespace LeanTest.Dependencies.Providers;

internal readonly record struct TimesContstraintProvider : ITimesContstraintProvider
{
	internal static readonly ITimesContstraintProvider Instance = new TimesContstraintProvider();

	public ITimesConstraint Once => throw new NotImplementedException();

	public ITimesConstraint Never => throw new NotImplementedException();

	public ITimesConstraint Exactly(int times)
	{
		throw new NotImplementedException();
	}
}