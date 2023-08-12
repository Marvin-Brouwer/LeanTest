using System.Linq.Expressions;

namespace LeanTest.Dependencies
{
    // TODO Times class
    // TODO Migrate to Fluent assertion instead of verify
    public interface IVerifyableDependency<TService, TDependency> : IDependency<TService>
        where TDependency : IVerifyableDependency<TService, TDependency>
    {
        TDependency Verify(Expression<Func<TService>> member, int times);
        TDependency Verify(Expression<Func<TService, Task>> member, int times);
        void VerifyNoOtherCalls();
    }
}
