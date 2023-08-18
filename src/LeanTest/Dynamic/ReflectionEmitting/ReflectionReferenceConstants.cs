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
	internal static readonly MethodInfo InvokeGenericNoParameters = typeof(IInvocationMarshall)
		.GetMethods()
		.First(method => method.Name == nameof(IInvocationMarshall.RequestInvoke)
			&& method.IsGenericMethod
			&& method.GetParameters().Length == 1
		)!;
	internal static readonly MethodInfo InvokeGenericWithParameters = typeof(IInvocationMarshall)
		.GetMethods()
		.First(method => method.Name == nameof(IInvocationMarshall.RequestInvoke)
			&& method.IsGenericMethod
			&& method.GetParameters().Length > 1
		)!;
	internal static readonly MethodInfo InvokeVoidNoParameters = typeof(IInvocationMarshall)
		.GetMethods()
		.First(method => method.Name == nameof(IInvocationMarshall.RequestInvoke)
			&& !method.IsGenericMethod
			&& method.GetParameters().Length == 1
		)!;
	internal static readonly MethodInfo InvokeVoidWithParameters = typeof(IInvocationMarshall)
		.GetMethods()
		.First(method => method.Name == nameof(IInvocationMarshall.RequestInvoke)
			&& !method.IsGenericMethod
			&& method.GetParameters().Length > 1
		)!;
}
