using System.Linq.Expressions;
using System.Reflection;

namespace LeanTest.Tests.Naming;

public readonly record struct For : ITestNamePart
{
	public string Name { get; }

	public For(LambdaExpression methodExpression)
	{
		Name = GetMethodName(methodExpression);
	}

	public Given Given(string value) => new(this, value);

	private static string GetMethodName(LambdaExpression methodExpression)
	{
		if (methodExpression.Body is not UnaryExpression unaryExpression)
		{
			// TODO better exception here
			throw new NotSupportedException();
		}
		// TODO also support properties and constructors?
		if (unaryExpression.Operand is not MethodCallExpression methodCallExpression)
		{
			// TODO better exception here
			throw new NotSupportedException();
		}
		if (methodCallExpression.Object is not ConstantExpression constantExpression)
		{
			// TODO better exception here
			throw new NotSupportedException();
		}
		if (constantExpression.Value is not MethodInfo method)
		{
			// TODO better exception here
			throw new NotSupportedException();
		}

		return method.Name;
	}
}
