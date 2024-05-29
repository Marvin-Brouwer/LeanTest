namespace LeanTest.Exceptions;

// TODO Make abstract and fix all places with expressive classes
public class LeanTestException : Exception
{
	public LeanTestException(string? message) : base(message)
	{
	}
}
