using LeanTest.Exceptions;

using static LeanTest.Dependencies.Definitions.TimesConstraint;

namespace LeanTest.Dependencies.Definitions;

internal sealed class TimesConstraint(Check InvocationCheck, Warning FaillureWarning) : ITimesConstraint
{
	internal delegate bool Check(uint invocationCount);
	internal delegate string Warning(uint invocationCount, string name);

	public static TimesConstraint Once = new(InvokedExactly(1), WarnExactlyOnce);
	private static string WarnExactlyOnce(uint invocationCount, string name) =>
		$"{name} was expected to be called once. However, \"{invocationCount}\" were counted.";

	public static TimesConstraint Never = new(InvokedExactly(0), WarnNever);
	private static string WarnNever(uint invocationCount, string name) =>
		$"{name} was expected to never be called. However, \"{invocationCount}\" were counted.";

	public static TimesConstraint Exactly(uint amountOfTimes) => new(InvokedExactly(amountOfTimes), WarnInvokedExactly(amountOfTimes));
	private static Check InvokedExactly(uint amountOfTimes) => (uint invocationCount) => invocationCount == amountOfTimes;
	private static Warning WarnInvokedExactly(uint amountOfTimes) => (uint invocationCount, string name) =>
		$"{name} was expected to be called excactly \"{amountOfTimes}\" time(s). However, \"{invocationCount}\" were counted.";

	public static TimesConstraint AtLeast(uint amountOfTimes) => new(InvokedAtLeast(amountOfTimes), WarnInvokedAtLeast(amountOfTimes));
	private static Check InvokedAtLeast(uint amountOfTimes) => (uint invocationCount) => invocationCount >= amountOfTimes;
	private static Warning WarnInvokedAtLeast(uint amountOfTimes) => (uint invocationCount, string name) =>
		$"{name} was expected to be called at least \"{amountOfTimes}\" time(s). However, \"{invocationCount}\" were counted.";

	public static TimesConstraint AtMost(uint amountOfTimes) => new(InvokedAtMost(amountOfTimes), WarnInvokedAtMost(amountOfTimes));
	private static Check InvokedAtMost(uint amountOfTimes) => (uint invocationCount) => invocationCount <= amountOfTimes;
	private static Warning WarnInvokedAtMost(uint amountOfTimes) => (uint invocationCount, string name) =>
		$"{name} was expected to be called at most \"{amountOfTimes}\" time(s). However, \"{invocationCount}\" were counted.";

	public static TimesConstraint Between(uint leastAmountOfTimes, uint mostAmountOfTimes, bool inclusive) => inclusive
		? new(
			InvokedBetweenInclusive(leastAmountOfTimes, mostAmountOfTimes),
			WarnInvokedBetweenInclusive(leastAmountOfTimes, mostAmountOfTimes)
		) 
		: new(
			InvokedBetweenExclusive(leastAmountOfTimes, mostAmountOfTimes),
			WarnInvokedBetweenExclusive(leastAmountOfTimes, mostAmountOfTimes)
		);
	private static Check InvokedBetweenInclusive(uint leastAmountOfTimes, uint mostAmountOfTimes) => (uint invocationCount) =>
		invocationCount >= leastAmountOfTimes &&
		invocationCount <= mostAmountOfTimes;
	private static Check InvokedBetweenExclusive(uint leastAmountOfTimes, uint mostAmountOfTimes) => (uint invocationCount) =>
		invocationCount > leastAmountOfTimes &&
		invocationCount < mostAmountOfTimes;
	private static Warning WarnInvokedBetween(uint from, uint to, string inclusivity) => (uint invocationCount, string name) =>
		$"{name} was expected to be called between \"{from}\" and \"{to}\" time(s) (${inclusivity}). However, \"{invocationCount}\" were counted.";
	private static Warning WarnInvokedBetweenInclusive(uint leastAmountOfTimes, uint mostAmountOfTimes) =>
		WarnInvokedBetween(leastAmountOfTimes, mostAmountOfTimes, "inclusive");
	private static Warning WarnInvokedBetweenExclusive(uint leastAmountOfTimes, uint mostAmountOfTimes) =>
		WarnInvokedBetween(leastAmountOfTimes, mostAmountOfTimes, "exclusive");

	public Exception? VerifyInvocations(uint invocationCount, string name)
	{
		if (InvocationCheck(invocationCount)) return null;
		// TODO Custom exception
		return new LeanTestException(FaillureWarning(invocationCount, name));
	}
}