using LeanTest.Dependencies.Definitions;

namespace LeanTest.Dependencies.Factories;

// TODO steal more from Moq
public interface ITimesFactory
{
	ITimesConstraint Once { get; }
	ITimesConstraint Never { get; }
	ITimesConstraint Exactly(int times);
}