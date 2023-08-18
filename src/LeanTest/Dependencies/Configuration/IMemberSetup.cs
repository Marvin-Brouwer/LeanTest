namespace LeanTest.Dependencies.Configuration;

public interface IMemberSetup<TDependency> where TDependency : IDependency
{
	TDependency Executes(Action callBack);
	TDependency Executes();
}

// TODO Task options? maybe valueTask options, perhaps have an extension for that
// TODO More parameter options
public interface IMemberSetup<TDependency, TReturn> where TDependency : IDependency
{
	TDependency Returns(TReturn returnValue);
	TDependency Returns(Func<TReturn> callBack);
	TDependency Returns<T1>(Func<T1, TReturn> callBack);
}
