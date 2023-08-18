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
		var newTypeAttribute = TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed;
		return moduleBuilder
			.DefineType(newTypeName, newTypeAttribute,
			!serviceType.IsInterface ? serviceType : null,
			serviceType.IsInterface ? new Type[] { serviceType } : null
		);
	}

	internal static void GenerateConstructor(this TypeBuilder typeBuilder, params FieldBuilder[] fields)
	{
		// TODO private base constructor?
		var baseConstructor = typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
		GenerateConstructorOverload(typeBuilder, baseConstructor, fields);
	}

	private static void GenerateConstructorOverload(
		TypeBuilder typeBuilder,
		ConstructorBuilder baseConstructor,
		FieldBuilder[] fields)
	{
		var wrapperCtor = typeBuilder.DefineConstructor(
			MethodAttributes.Public,
			CallingConventions.Standard,
			// TODO remove linq
			parameterTypes: fields.Select(field => field.FieldType).ToArray()
		);

		var ctorIL = wrapperCtor.GetILGenerator();

		ctorIL.Emit(OpCodes.Ldarg_0);
		ctorIL.Emit(OpCodes.Call, baseConstructor);
		ctorIL.Emit(OpCodes.Nop);
		ctorIL.Emit(OpCodes.Nop);
		for (int i = 0; i < fields.Length; i++)
		{
			FieldBuilder? field = fields[i];

			ctorIL.Emit(OpCodes.Ldarg_0);
			ctorIL.Emit(OpCodes.Ldarg, i + 1);
			ctorIL.Emit(OpCodes.Stfld, field);
		}
		ctorIL.Emit(OpCodes.Ret);
	}
	internal static FieldBuilder GeneratePrivateField(this TypeBuilder typeBuilder, string fieldName, Type fieldType) =>
		typeBuilder.DefineField($"_{fieldName}", fieldType, FieldAttributes.Private | FieldAttributes.InitOnly);
}
