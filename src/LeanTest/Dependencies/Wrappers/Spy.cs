using System.Linq.Expressions;

namespace LeanTest.Dependencies.Wrappers
{
    // TODO SpyProxy
    internal class Spy<TService> : ISpy<TService>
    {
        public TService Instance { get; }

        public Spy(TService service)
        {
        }

        public ISpy<TService> Verify(Expression<Func<TService>> member, int times)
        {
            throw new NotImplementedException();
        }

        public ISpy<TService> Verify(Expression<Func<TService, Task>> member, int times)
        {
            throw new NotImplementedException();
        }

        public void VerifyNoOtherCalls()
        {
            throw new NotImplementedException();
        }
    }
}
