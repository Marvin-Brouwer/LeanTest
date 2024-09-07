namespace LeanTest.Dependencies.Configuration;

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

	// https://github.com/Marvin-Brouwer/LeanTest/issues/4
	TDependency Executes<TDelegate>(TDelegate callBack) where TDelegate : Delegate;
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

	// https://github.com/Marvin-Brouwer/LeanTest/issues/4
	TDependency Returns<TDelegate>(TDelegate callBack) where TDelegate : Delegate;
}
