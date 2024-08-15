namespace LeanTest.Exceptions;

// TODO serializable // TODO is this even used?
public sealed class UnreachableCodeException: NotSupportedException
{
	public string? Why { get; init; }

	public UnreachableCodeException() : base("Unreachable code reached")
	{
		
	}
}
