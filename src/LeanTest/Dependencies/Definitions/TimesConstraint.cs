using System.Diagnostics;

namespace LeanTest.Dependencies.Definitions;

[DebuggerDisplay("{_debugDisplay}")]
internal sealed class TimesConstraint : ITimesConstraint
{	

	internal delegate bool Check(uint invocationCount);
	internal delegate string Warning(uint invocationCount, string name);

	private readonly Check _invocationCheck;
	private readonly Warning _faillureWarning;
	private readonly string _debugDisplay;

	public TimesConstraint(Check invocationCheck, Warning faillureWarning, string debugDisplay)
	{
		_invocationCheck = invocationCheck;
		_faillureWarning = faillureWarning;
		_debugDisplay = debugDisplay;
	}

	public static TimesConstraint Once = new(InvokedExactly(1), WarnExactlyOnce, nameof(Once));
	private static string WarnExactlyOnce(uint invocationCount, string name) =>
		$"{name} was expected to be called once. However, \"{invocationCount}\" were counted.";

	public static TimesConstraint Never = new(InvokedExactly(0), WarnNever, nameof(Never));
	private static string WarnNever(uint invocationCount, string name) =>
		$"{name} was expected to never be called. However, \"{invocationCount}\" were counted.";

	public static TimesConstraint Exactly(uint amountOfTimes) => new(InvokedExactly(amountOfTimes), WarnInvokedExactly(amountOfTimes), $"Exactly {amountOfTimes}");
	private static Check InvokedExactly(uint amountOfTimes) => (uint invocationCount) => invocationCount == amountOfTimes;
	private static Warning WarnInvokedExactly(uint amountOfTimes) => (uint invocationCount, string name) =>
		$"{name} was expected to be called excactly \"{amountOfTimes}\" time(s). However, \"{invocationCount}\" were counted.";

	public static TimesConstraint AtLeast(uint amountOfTimes) => new(InvokedAtLeast(amountOfTimes), WarnInvokedAtLeast(amountOfTimes), $"At least {amountOfTimes}");
	private static Check InvokedAtLeast(uint amountOfTimes) => (uint invocationCount) => invocationCount >= amountOfTimes;
	private static Warning WarnInvokedAtLeast(uint amountOfTimes) => (uint invocationCount, string name) =>
		$"{name} was expected to be called at least \"{amountOfTimes}\" time(s). However, \"{invocationCount}\" were counted.";

	public static TimesConstraint AtMost(uint amountOfTimes) => new(InvokedAtMost(amountOfTimes), WarnInvokedAtMost(amountOfTimes), $"At most {amountOfTimes}");
	private static Check InvokedAtMost(uint amountOfTimes) => (uint invocationCount) => invocationCount <= amountOfTimes;
	private static Warning WarnInvokedAtMost(uint amountOfTimes) => (uint invocationCount, string name) =>
		$"{name} was expected to be called at most \"{amountOfTimes}\" time(s). However, \"{invocationCount}\" were counted.";

	public static TimesConstraint Between(uint leastAmountOfTimes, uint mostAmountOfTimes, bool inclusive) => inclusive
		? new(
			InvokedBetweenInclusive(leastAmountOfTimes, mostAmountOfTimes),
			WarnInvokedBetweenInclusive(leastAmountOfTimes, mostAmountOfTimes),
			$"Between {leastAmountOfTimes} and {mostAmountOfTimes} (inclusive)"
		) 
		: new(
			InvokedBetweenExclusive(leastAmountOfTimes, mostAmountOfTimes),
			WarnInvokedBetweenExclusive(leastAmountOfTimes, mostAmountOfTimes),
			$"Between {leastAmountOfTimes} and {mostAmountOfTimes} (exclusive)"
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
		if (_invocationCheck(invocationCount)) return null;
		return new TimesConstraintVerficationFaillure(_faillureWarning(invocationCount, name));
	}
}