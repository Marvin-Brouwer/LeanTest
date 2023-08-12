using System.Linq.Expressions;

namespace LeanTest.Dependencies
{
    public interface IConfigurableDependency<TService, TDependency> : IDependency<TService>
        where TDependency : IConfigurableDependency<TService, TDependency>
    {
        TDependency Setup(Expression<Func<TService>> member);
        TDependency Setup(Expression<Func<TService, Task>> member);
        TDependency Setup<TReturn>(Expression<Func<TService, TReturn>> member, Func<TReturn> returnValue);
        TDependency Setup<TReturn>(Expression<Func<TService, Task<TReturn>>> member, Func<TReturn> returnValue);
    }
}
