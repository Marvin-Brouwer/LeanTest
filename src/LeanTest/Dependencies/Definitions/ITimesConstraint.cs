namespace LeanTest.Dependencies.Definitions;

public interface ITimesConstraint
{
	Exception? VerifyInvocations(uint invocationCount, string name);
}
