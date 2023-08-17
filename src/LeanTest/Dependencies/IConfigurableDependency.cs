using System.Linq.Expressions;

namespace LeanTest.Dependencies;

public interface IConfigurableDependency<TService, TDependency> : IDependency<TService>
	where TDependency : IConfigurableDependency<TService, TDependency>
{
	IMemberSetup<TDependency> Setup(Expression<Action<TService>> member);
	IMemberSetup<TDependency, TReturn> Setup<TReturn>(Expression<Func<TService, TReturn>> member);
}
