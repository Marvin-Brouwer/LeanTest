using System.Reflection.Emit;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace LeanTest.Dynamic.ReflectionEmitting;

internal static class ClassEmitExtensions
{

	internal static TypeBuilder GenerateRuntimeType(this ModuleBuilder moduleBuilder, Type serviceType, string dependencyType)
	{
		// TODO see if namespace is necessary
		var newTypeName = $"{moduleBuilder.Assembly.GetName().Name}.Runtime{dependencyType}<{serviceType.Name}>";
		var newTypeAttribute = TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.Sealed;
		return moduleBuilder
			.DefineType(newTypeName, newTypeAttribute,
			!serviceType.IsInterface ? serviceType : null,
			serviceType.IsInterface ? new Type[] { serviceType } : null
		);
	}

	internal static void GenerateConstructor(this TypeBuilder typeBuilder, params FieldBuilder[] fields)
	{
		// #1: Figure out why IL output breaks for constructor when adding parameters
#if DEBUG
		typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
		_ = fields;
#else
		var baseConstructor = typeBuilder.DefineDefaultConstructor(MethodAttributes.Private);
		GenerateConstructorOverload(typeBuilder, baseConstructor, fields);
#endif
	}

	private static void GenerateConstructorOverload(
		TypeBuilder typeBuilder,
		ConstructorBuilder baseConstructor,
		FieldBuilder[] fields)
	{
		var wrapperCtor = typeBuilder.DefineConstructor(
			MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName |
			MethodAttributes.Final,
			CallingConventions.Standard,
			// #1: Figure out why IL output breaks for constructor when adding parameters
			// TODO remove linq
			parameterTypes: fields.Select(field => field.FieldType).ToArray()
		);

		var ctorIL = wrapperCtor.GetILGenerator();

		// Call base
		ctorIL.Emit(OpCodes.Ldarg_0);
		ctorIL.Emit(OpCodes.Call, baseConstructor);
		ctorIL.Emit(OpCodes.Nop);
		// Load parameters into fields
		ctorIL.Emit(OpCodes.Nop);
		for (int i = 0; i < fields.Length; i++)
		{
			var field = fields[i];
			wrapperCtor.DefineParameter(i + 1, ParameterAttributes.In, field.Name.TrimStart('_'));

			ctorIL.Emit(OpCodes.Ldarg_0);
			ctorIL.Emit(OpCodes.Ldarg, i + 1);
			ctorIL.Emit(OpCodes.Stfld, field);
		}
		// End constructor
		ctorIL.Emit(OpCodes.Ret);
	}
	internal static FieldBuilder GeneratePrivateField(this TypeBuilder typeBuilder, string fieldName, Type fieldType) =>
		typeBuilder.DefineField($"_{fieldName}", fieldType, FieldAttributes.Private | FieldAttributes.InitOnly);
}
