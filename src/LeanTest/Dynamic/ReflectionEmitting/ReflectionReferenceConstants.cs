using LeanTest.Dynamic.Invocation;

using System.Reflection;

namespace LeanTest.Dynamic.ReflectionEmitting;

internal readonly record struct ReflectionReferenceConstants
{
	internal static readonly MethodInfo GetCurrentMethod = typeof(MethodBase)
		.GetMethod(nameof(MethodBase.GetCurrentMethod))!;

	internal static readonly MethodInfo ObjectArrayEmpty = typeof(Array)
		.GetMethod(nameof(Array.Empty))!
		.GetGenericMethodDefinition()!
		.MakeGenericMethod(new[] { typeof(object) })!;

	// TODO remove linq
	internal static readonly MethodInfo InvokeStubGeneric = typeof(InvocationMarshall)
		.GetMethods()
		.First(method => method.Name == nameof(InvocationMarshall.InvokeStub)
			&& method.IsGenericMethod
		)!;
	internal static readonly MethodInfo InvokeStubVoid = typeof(InvocationMarshall)
		.GetMethods()
		.First(method => method.Name == nameof(InvocationMarshall.InvokeStub)
			&& !method.IsGenericMethod
		)!;
}
