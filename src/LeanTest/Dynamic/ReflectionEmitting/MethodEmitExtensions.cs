using System.Reflection.Emit;
using System.Reflection;

namespace LeanTest.Dynamic.ReflectionEmitting;

internal static class MethodEmitExtensions
{
	internal static void GenerateDynamicImplementationMethods(
		this TypeBuilder typeBuilder,
		Type serviceType,
		// TODO FieldBuilder[] fields 
		FieldBuilder configuredMethodsField)
	{
		foreach (var interfaceMethod in serviceType.GetMethods())
		{
			GenerateDynamicImplementationMethod(
				typeBuilder,
				interfaceMethod,
				configuredMethodsField
			);
		}
	}

	private static void GenerateDynamicImplementationMethod(
		TypeBuilder typeBuilder,
		MethodInfo method,
		// TODO FieldBuilder[] fields 
		FieldBuilder invocationMarshallField)
	{
		var parameters = method.GetParameters();
		var hasParameters = parameters.Length > 0;
		var isVoid = method.ReturnType == typeof(void);

		var attibutes = MethodAttributes.Public
			| MethodAttributes.Final
			| MethodAttributes.HideBySig
			| MethodAttributes.NewSlot
			| MethodAttributes.Virtual;

		var methodBuilder = typeBuilder.DefineMethod(
			method.Name, attibutes, method.ReturnType,
			// TODO remove linq
			parameters.Select(param => param.ParameterType).ToArray()
		);
		if (method.IsGenericMethod)
		{
			// TODO remove linq
			var genericArgumentNames = method.GetGenericMethodDefinition().GetGenericArguments().Select(arg => arg.Name).ToArray();
			methodBuilder.DefineGenericParameters(genericArgumentNames);
		}

		var methodIL = methodBuilder.GetILGenerator();

		// TODO Explain why reflection emit
		methodIL.Emit(OpCodes.Ldarg_0);
		methodIL.Emit(OpCodes.Ldfld, invocationMarshallField);
		methodIL.Emit(OpCodes.Call, ReflectionReferenceConstants.GetCurrentMethod);


		if (hasParameters)
		{
			// Create new array of object
			methodIL.Emit(OpCodes.Ldc_I4, parameters.Length);
			methodIL.Emit(OpCodes.Newarr, typeof(object));
			methodIL.Emit(OpCodes.Dup);

			// Add parameters
			for (int i = 0; i < parameters.Length; i++)
			{
				var parameter = parameters[i]!;
				var shouldBox = parameter.ParameterType.IsValueType;
				var parameterNumber = i + 1;

				methodIL.Emit(OpCodes.Ldc_I4, i);
				methodIL.Emit(OpCodes.Ldarg, parameterNumber);

				if (shouldBox)
					methodIL.Emit(OpCodes.Box, parameter.ParameterType);

				methodIL.Emit(OpCodes.Stelem_Ref);

				if (i < parameters.Length - 1)
					methodIL.Emit(OpCodes.Dup);
			}
		}

		// Making a type generic of TReturn is easier than boxing if necessary
		if (isVoid)
		{
			methodIL.Emit(OpCodes.Call, hasParameters
				? ReflectionReferenceConstants.InvokeVoidWithParameters
				: ReflectionReferenceConstants.InvokeVoidNoParameters);
			methodIL.Emit(OpCodes.Nop);
		}
		else
		{
			var invokeMethod = hasParameters
				? ReflectionReferenceConstants.InvokeGenericWithParameters
				: ReflectionReferenceConstants.InvokeGenericNoParameters!;
			invokeMethod = invokeMethod
					.GetGenericMethodDefinition()!
					.MakeGenericMethod(new[] { method.ReturnType })!;
			methodIL.Emit(OpCodes.Callvirt, invokeMethod);
		}

		methodIL.Emit(OpCodes.Ret);
	}
}
