using Ardalis.GuardClauses;

using LeanTest.Dependencies.Definitions;

namespace LeanTest.Dependencies.Providers;

internal readonly record struct TimesContstraintProvider : ITimesContstraintProvider
{
	internal static readonly ITimesContstraintProvider Instance = new TimesContstraintProvider();

	public ITimesConstraint Once => TimesConstraint.Once;
	public ITimesConstraint Never => TimesConstraint.Never;


	public ITimesConstraint Exactly(uint amountOfTimes) => TimesConstraint.Exactly(amountOfTimes);
	public ITimesConstraint AtLeast(uint amountOfTimes) => TimesConstraint.AtLeast(amountOfTimes);
	public ITimesConstraint AtMost(uint amountOfTimes) => TimesConstraint.AtMost(amountOfTimes);
	public ITimesConstraint Between(uint leastAmountOfTimes, uint mostAmountOfTimes, bool inclusive) {
		Guard.Against.InvalidInput(mostAmountOfTimes, nameof(mostAmountOfTimes),
			(val) => val > leastAmountOfTimes,
			"Required input mostAmountOfTimes cannot be less than leastAmountOfTimes."
		);
		return TimesConstraint.Between(leastAmountOfTimes, mostAmountOfTimes, inclusive);
	}
}