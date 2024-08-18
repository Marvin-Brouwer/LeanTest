using System.Diagnostics;
using System.Reflection;

namespace LeanTest.Dependencies.Configuration;

internal abstract record MethodDefinition(
	MethodBase Method, ParameterInfo[] Parameters, Type ReturnType
)
{
	internal static MethodDefinition FromMethodInfo(MethodBase methodInfo)
	{
		return new CannedMethodDefinition(methodInfo);
	}
}

internal sealed record CannedMethodDefinition(MethodBase Method)
	: MethodDefinition(Method, Method.GetParameters(), GetReturnType(Method))
{
	private static Type GetReturnType(MethodBase method)
	{
		// We are pretty sure this will never happen. However, if it does, we'll need to blow up.
		// We don't know why this may happen or why a MethodBase doesn't have a ReturnType.
		// So, we'll need to reproduce and debug to see what happened.
		if (method is not MethodInfo methodInfo) throw new UnreachableException();
		return methodInfo.ReturnType;
	}
}