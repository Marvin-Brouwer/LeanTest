using LeanTest.Exceptions;

namespace LeanTest.Dependencies.Definitions;

#if (!NET8_0_OR_GREATER)
[Serializable]
#endif
public sealed class ConstraintVerficationFaillure : LeanTestAssertionFaillure
{
	internal ConstraintVerficationFaillure(string? message) : base(message) { }
}
