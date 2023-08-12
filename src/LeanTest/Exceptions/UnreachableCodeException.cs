namespace LeanTest.Exceptions;

// TODO serializable
public sealed class UnreachableCodeException: NotSupportedException
{
	public string? Why { get; init; }

	public UnreachableCodeException() : base("Unreachable code reached")
	{
		
	}
}
