using LeanTest.Dependencies.Definitions;

namespace LeanTest.Dependencies.Providers;

public interface ITimesContstraintProvider
{
	ITimesConstraint Once { get; }
	ITimesConstraint Never { get; }
	ITimesConstraint Exactly(uint amountOfTimes);
	ITimesConstraint AtLeast(uint amountOfTimes);
	ITimesConstraint AtMost(uint amountOfTimes);
	ITimesConstraint Between(uint leastAmountOfTimes, uint mostAmountOfTimes, bool inclusive);
}