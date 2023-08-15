using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeanTest.Dependencies;

public interface IFixture<TClass> : IDependency<TClass>
{
	IFixture<TClass> AddMutation(Action<TClass> mutation);
	IFixture<TClass> AddMutation(Func<TClass, TClass> mutation);
}
