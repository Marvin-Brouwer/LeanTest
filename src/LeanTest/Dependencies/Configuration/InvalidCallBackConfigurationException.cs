using LeanTest.Exceptions;

using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace LeanTest.Dependencies.Configuration;

#if (!NET8_0_OR_GREATER)
[Serializable]
#endif
public sealed class InvalidCallBackConfigurationException : LeanTestException
{
	internal static InvalidCallBackConfigurationException For<T>(LambdaExpression member) => new("method", member, typeof(T));

	public LambdaExpression TargetMemberExpression { get; }
	public Type UsedType { get; }

	private InvalidCallBackConfigurationException(string configureType, LambdaExpression member, Type type)
		: base($"An attempt was made to configure a {configureType} with an improper lambda expression")
	{
		TargetMemberExpression = member;
		UsedType = type;
	}

#if (!NET8_0_OR_GREATER)
	#region Serializable
	private InvalidCallBackConfigurationException(in SerializationInfo info, in StreamingContext context) : base(in info, in context)
	{
		TargetMemberExpression = (LambdaExpression)info.GetValue(nameof(TargetMemberExpression), typeof(LambdaExpression))!;
		UsedType = (Type)info.GetValue(nameof(UsedType), typeof(Type))!;
	}

	/// <inheritdoc />
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue(nameof(TargetMemberExpression), TargetMemberExpression);
		info.AddValue(nameof(UsedType), UsedType);

		base.GetObjectData(info, context);
	}
	#endregion Serializable
#endif
}
