using LeanTest.Dependencies.Definitions;

namespace LeanTest.Dependencies.Providers;

// TODO steal more from Moq
public interface ITimesContstraintProvider
{
	ITimesConstraint Once { get; }
	ITimesConstraint Never { get; }
	ITimesConstraint Exactly(int times);
}