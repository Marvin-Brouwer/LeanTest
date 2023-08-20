using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LeanTest.Dynamic.ReflectionEmitting;
internal static class ParameterExtensions
{
	internal static Type[] GetParameterTypes(this ParameterInfo[] parameters)
	{
		var types = new Type[parameters.Length];
		for (int i = 0; i < parameters.Length; i++)
		{
			var parameter = parameters[i];
			types[i] = parameter.ParameterType;
		}

		return types;
	}
	internal static string[] GetGenericArgumentNames(this MethodInfo method)
	{
		var names = method.GetGenericArguments();
		var types = new string[names.Length];
		for (int i = 0; i < names.Length; i++)
		{
			var argument = names[i];
			types[i] = argument.Name;
		}

		return types;
	}
}
