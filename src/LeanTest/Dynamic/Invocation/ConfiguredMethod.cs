using System.Linq.Expressions;
using System.Reflection;

namespace LeanTest.Dynamic.Invocation;

internal readonly record struct ConfiguredMethod(
	MethodInfo Method, ParameterInfo[] Parameters, Type? ReturnType, Delegate? ReturnDelegate
)
{
	internal static ConfiguredMethod FromExpression(LambdaExpression member, Type? returnType, Delegate? returnDelegate = null)
	{
		if (member.Body is not MethodCallExpression methodExpression)
		{
			// TODO, better exception
			throw new NotSupportedException();
		}
		//new System.Linq.Expressions.Expression.MethodCallExpressionProxy(new System.Linq.Expressions.Expression.LambdaExpressionProxy(member).Body).Method
		// TODO figure these ouy
		var method = methodExpression.Method;
		var parameters = method.GetParameters();

		return new ConfiguredMethod(
			method,
			parameters,
			returnType,
			returnDelegate
		);
	}
};