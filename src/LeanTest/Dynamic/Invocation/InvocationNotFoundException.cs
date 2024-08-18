using LeanTest.Exceptions;

using System.Reflection;
using System.Runtime.Serialization;

namespace LeanTest.Dynamic.Invocation;

#if (!NET8_0_OR_GREATER)
[Serializable]
#endif
public sealed class InvocationNotFoundException : LeanTestException
{
	internal InvocationNotFoundException(MethodBase methodInfo, object?[] parameters, Type returnType)
		: base($"Requested method \"{methodInfo.Name}\" was not configured with the requested parameters. {Environment.NewLine} " +
			$"Please make sure to configure your mocks, fakes, stubs, and spies.")
	{
		MethodInfo = methodInfo;
		Parameters = parameters;
		ReturnType = returnType;
	}

	public InvocationNotFoundException(MethodBase methodInfo, object?[] parameters)
		: this(methodInfo, parameters, typeof(void)) { }

	public MethodBase MethodInfo { get; }
	public object?[] Parameters { get; }
	public Type ReturnType { get; }

#if (!NET8_0_OR_GREATER)
	#region Serializable
	private InvocationNotFoundException(in SerializationInfo info, in StreamingContext context) : base(in info, in context)
	{
		MethodInfo = (MethodBase)info.GetValue(nameof(MethodInfo), typeof(MethodBase))!;
		Parameters = (object?[])info.GetValue(nameof(Parameters), typeof(object?[]))!;
		ReturnType = (Type)info.GetValue(nameof(ReturnType), typeof(Type))!;
	}

	/// <inheritdoc />
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue(nameof(MethodInfo), MethodInfo);
		info.AddValue(nameof(Parameters), Parameters);
		info.AddValue(nameof(ReturnType), ReturnType);

		base.GetObjectData(info, context);
	}
	#endregion Serializable
#endif
}
