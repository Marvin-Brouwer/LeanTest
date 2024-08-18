#if (!NET8_0_OR_GREATER)
using System.Runtime.Serialization;
#endif

namespace LeanTest.Exceptions;

#if (!NET8_0_OR_GREATER)
[Serializable]
#endif
public abstract class LeanTestException : Exception
{
	protected LeanTestException(string? message) : base(message) { }
	private LeanTestException() { }

#if (!NET8_0_OR_GREATER)
	/// <inheritdoc />
	protected LeanTestException(in SerializationInfo info, in StreamingContext context) : base(info, context) { }
	/// <inheritdoc />
	protected LeanTestException(in string message) : base(message) { }
#endif
}
