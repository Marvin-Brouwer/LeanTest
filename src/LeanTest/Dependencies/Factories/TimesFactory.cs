using LeanTest.Dependencies.Definitions;

namespace LeanTest.Dependencies.Factories;

internal readonly record struct TimesFactory : ITimesFactory
{
	internal static readonly ITimesFactory Instance = new TimesFactory();

	public ITimesConstraint Once => throw new NotImplementedException();

	public ITimesConstraint Never => throw new NotImplementedException();

	public ITimesConstraint Exactly(int times)
	{
		throw new NotImplementedException();
	}
}