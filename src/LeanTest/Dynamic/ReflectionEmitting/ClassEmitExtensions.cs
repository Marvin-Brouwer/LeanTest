using System.Reflection.Emit;
using System.Reflection;
using Microsoft.CodeAnalysis;
using LeanTest.Dynamic.Invocation;

namespace LeanTest.Dynamic.ReflectionEmitting;

internal static class ClassEmitExtensions
{

	internal static TypeBuilder GenerateRuntimeType(this ModuleBuilder moduleBuilder, Type serviceType)
	{
		var newTypeName = $"{moduleBuilder.Assembly.GetName().Name}.Runtime{serviceType.Name.CleanClassName()}";
		var newTypeAttribute = TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.Sealed;
		return moduleBuilder
			.DefineType(newTypeName, newTypeAttribute,
			!serviceType.IsInterface ? serviceType : null,
			serviceType.IsInterface ? new Type[] { serviceType } : null
		);
	}

	internal static void GenerateConstructor(this TypeBuilder typeBuilder, FieldBuilder interceptorField)
	{
		// #1: Figure out why IL output breaks for constructor when adding parameters
#if DEBUG
		typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
		_ = interceptorField;
#else
		var baseConstructor = typeBuilder.DefineDefaultConstructor(MethodAttributes.Private);
		GenerateConstructorOverload(typeBuilder, baseConstructor, interceptorField);
#endif
	}

	private static void GenerateConstructorOverload(
		TypeBuilder typeBuilder,
		ConstructorBuilder baseConstructor,
		FieldBuilder interceptorField)
	{
		var wrapperCtor = typeBuilder.DefineConstructor(
			MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName |
			MethodAttributes.Final,
			CallingConventions.Standard,
			// #1: Figure out why IL output breaks for constructor when adding parameters
			parameterTypes: new[] { typeof (IInvokeInterceptor) } 
		);

		var ctorIL = wrapperCtor.GetILGenerator();

		// Call base
		ctorIL.Emit(OpCodes.Ldarg_0);
		ctorIL.Emit(OpCodes.Call, baseConstructor);
		ctorIL.Emit(OpCodes.Nop);
		// Load interceptor into interceptorField
		ctorIL.Emit(OpCodes.Nop);
		// TODO, is this necessary?
		wrapperCtor.DefineParameter(1, ParameterAttributes.In, interceptorField.Name.TrimStart('_'));
		ctorIL.Emit(OpCodes.Ldarg_0);
		ctorIL.Emit(OpCodes.Ldarg, 1);
		ctorIL.Emit(OpCodes.Stfld, interceptorField);
		// End constructor
		ctorIL.Emit(OpCodes.Ret);
	}

	internal static FieldBuilder GeneratePrivateField(this TypeBuilder typeBuilder, string fieldName, Type fieldType) =>
		typeBuilder.DefineField($"_{fieldName}", fieldType, FieldAttributes.Private | FieldAttributes.InitOnly);
}
