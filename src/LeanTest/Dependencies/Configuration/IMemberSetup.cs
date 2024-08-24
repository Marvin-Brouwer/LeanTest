namespace LeanTest.Dependencies.Configuration;

public delegate void DynamicAction(params object?[] parameters);
public delegate TReturn DynamicFunction<TReturn>(params object?[] parameters);

public interface IMemberSetup<out TDependency> where TDependency : IDependency
{
	TDependency Throws<TException>(Func<TException> exception) where TException : Exception;
	TDependency Throws<TException>(TException exception) where TException : Exception;

	TDependency Executes();
	TDependency Executes(Action callBack);
	TDependency Executes<T1>(Action<T1> callBack);
	TDependency Executes<T1, T2>(Action<T1, T2> callBack);
	TDependency Executes<T1, T2, T3>(Action<T1, T2, T3> callBack);
	TDependency Executes<T1, T2, T3, T4>(Action<T1, T2, T3, T4> callBack);
	TDependency Executes<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> callBack);

	// TODO-analyzer to prevent this under 5 params  -> Convert to GitHub task
	TDependency Executes(DynamicAction callBack);
}

public interface IMemberSetup<TDependency, TReturn> where TDependency : IDependency
{
	TDependency Throws<TException>(Func<TException> exception) where TException : Exception;
	TDependency Throws<TException>(TException exception) where TException : Exception;

	TDependency Returns(TReturn returnValue);
	TDependency Returns(Func<TReturn> callBack);
	TDependency Returns<T1>(Func<T1, TReturn> callBack);
	TDependency Returns<T1, T2>(Func<T1, T2, TReturn> callBack);
	TDependency Returns<T1, T2, T3>(Func<T1, T2, T3, TReturn> callBack);
	TDependency Returns<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TReturn> callBack);
	TDependency Returns<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, TReturn> callBack);

	// TODO-analyzer to prevent this under 5 params -> Convert to GitHub task
	TDependency Returns(DynamicFunction<TReturn> callBack);
}
