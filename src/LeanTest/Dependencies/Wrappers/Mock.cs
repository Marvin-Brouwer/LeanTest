using System.Linq.Expressions;

namespace LeanTest.Dependencies.Wrappers
{
    // TODO MockProxy
    internal class Mock<TService> : IMock<TService>
    {
        private readonly ISpy<TService> _spy;
        private readonly IStub<TService> _stub;

        public TService Instance { get; }

        public IMock<TService> Setup(Expression<Func<TService>> member)
        {
            _stub.Setup(member);
            return this;
        }

        public IMock<TService> Setup(Expression<Func<TService, Task>> member)
        {
            _stub.Setup(member);
            return this;
        }

        public IMock<TService> Setup<TReturn>(Expression<Func<TService, TReturn>> member, Func<TReturn> returnValue)
        {
            _stub.Setup(member, returnValue);
            return this;
        }

        public IMock<TService> Setup<TReturn>(Expression<Func<TService, Task<TReturn>>> member, Func<TReturn> returnValue)
        {
            _stub.Setup(member, returnValue);
            return this;
        }

        public IMock<TService> Verify(Expression<Func<TService>> member, int times)
        {
            _spy.Verify(member, times);
            return this;
        }

        public IMock<TService> Verify(Expression<Func<TService, Task>> member, int times)
        {
            _spy.Verify(member, times);
            return this;
        }

        public void VerifyNoOtherCalls()
        {
            _spy.VerifyNoOtherCalls();
        }
    }
}
