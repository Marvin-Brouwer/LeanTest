using System.Linq.Expressions;
using System.Reflection;

namespace LeanTest.Tests.Naming
{
    public readonly record struct Then(string Value) : ITestName
    {
        private readonly ITestNamePart _parent = default!;

        public Then(When when, string value) : this(value)
        {
            _parent = when;
        }
        public Then(Given given, string value) : this(value)
        {
            _parent = given; 
        }

        public string Name => String.Join("_", _parent.Name, Value);

        public string GetName(Expression methodExpression)
        {
            // TODO GetName, see FluentSerializer
            //((System.Reflection.RuntimeMethodInfo)(new System.Linq.Expressions.Expression.ConstantExpressionProxy(new System.Linq.Expressions.Expression.MethodCallExpressionProxy(new System.Linq.Expressions.Expression.UnaryExpressionProxy((methodExpression as LambdaExpression).Body).Operand).Object).Value)).Name
            var methodName = (((((methodExpression as LambdaExpression).Body as UnaryExpression).Operand as MethodCallExpression).Object as ConstantExpression).Value as MethodInfo)?.Name;

            return String.Join("_", methodName, Name);
        }
    }

}
