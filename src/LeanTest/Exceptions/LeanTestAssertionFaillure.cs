#if (!NET8_0_OR_GREATER)
using System.Runtime.Serialization;
#endif

namespace LeanTest.Exceptions;

// TODOC
#if (!NET8_0_OR_GREATER)
[Serializable]
#endif
public abstract class LeanTestAssertionFaillure: LeanTestException
{
	public LeanTestAssertionFaillure(string? message) : base(message) { }

#if (!NET8_0_OR_GREATER)
	/// <inheritdoc />
	protected LeanTestAssertionFaillure(in SerializationInfo info, in StreamingContext context) : base(info, context) { }
	/// <inheritdoc />
	protected LeanTestAssertionFaillure(in string message) : base(message) { }
#endif
}
