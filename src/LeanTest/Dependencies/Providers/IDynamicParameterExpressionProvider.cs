using System.Dynamic;
using System.Linq.Expressions;

namespace LeanTest.Dependencies.Providers;
public interface ITypedParameterExpressionProvider<TParam> : IParameterExpressionProvider
{
	TParam Match(Expression<Func<TParam, bool>> match);
}

public interface IDynamicParameterExpressionProvider: IParameterExpressionProvider
{
	DynamicObject Matches(Expression<Func<DynamicObject, bool>> match);
}

public interface IParameterExpressionProvider;