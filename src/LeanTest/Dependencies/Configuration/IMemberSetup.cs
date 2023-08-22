namespace LeanTest.Dependencies.Configuration;


// TODO More parameter options
public interface IMemberSetup<out TDependency> where TDependency : IDependency
{
	TDependency Throws(Func<Exception> exception);
	TDependency Throws(Exception exception);

	TDependency Executes();
	TDependency Executes(Action callBack);
	TDependency Executes<T1>(Action<T1> callBack);
}

// TODO More parameter options
public interface IMemberSetup<TDependency, TReturn> where TDependency : IDependency
{
	TDependency Throws(Func<Exception> exception);
	TDependency Throws(Exception exception);

	TDependency Returns(TReturn returnValue);
	TDependency Returns(Func<TReturn> callBack);
	TDependency Returns<T1>(Func<T1, TReturn> callBack);
}
