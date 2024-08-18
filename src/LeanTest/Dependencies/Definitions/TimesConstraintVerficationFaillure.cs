using LeanTest.Exceptions;

namespace LeanTest.Dependencies.Definitions;

#if (!NET8_0_OR_GREATER)
[Serializable]
#endif
public sealed class TimesConstraintVerficationFaillure : LeanTestAssertionFaillure
{
	internal TimesConstraintVerficationFaillure(string? message) : base(message) { }
}
