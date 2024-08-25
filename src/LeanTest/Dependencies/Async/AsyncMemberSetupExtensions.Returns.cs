#pragma warning disable RCS1047 // Allow Async postfix, because overloading doesn't work otherwise

using LeanTest.Dependencies.Configuration;
using LeanTest.Dependencies.SupportingTypes;

namespace LeanTest.Dependencies.Async;

public static partial class AsyncMemberSetupExtensions
{
	public static TDependency ReturnsAsync<TDependency, TReturn>(
		this IMemberSetup<TDependency, Task<TReturn>> memberSetup, Func<TReturn> asyncCallback
	)
		where TDependency : IDependency
	{
		Task<TReturn> SimulatedAsyncCallback() {
			try {
				return Task.FromResult(asyncCallback());
			}
			catch (Exception ex)
			{
				return Task.FromException<TReturn>(ex);
			}
		}

		return memberSetup
			.Returns(SimulatedAsyncCallback);
	}

	public static TDependency ReturnsAsync<TDependency, TReturn>(
		this IMemberSetup<TDependency, ValueTask<TReturn>> memberSetup, Func<TReturn> asyncCallback
	)
		where TDependency : IDependency
	{
		ValueTask<TReturn> SimulatedAsyncCallback()
		{
			try
			{
				return new ValueTask<TReturn>(Task.FromResult(asyncCallback()));
			}
			catch (Exception ex)
			{
				return new ValueTask<TReturn>(Task.FromException<TReturn>(ex));
			}
		}

		return memberSetup
			.Returns(SimulatedAsyncCallback);
	}

	public static TDependency ReturnsAsync<TDependency, T1, TReturn>(
		this IMemberSetup<TDependency, Task<TReturn>> memberSetup, Func<T1, TReturn> asyncCallback
	)
		where TDependency : IDependency
	{
		Task<TReturn> SimulatedAsyncCallback(T1 t1)
		{
			try
			{
				return Task.FromResult(asyncCallback(t1));
			}
			catch (Exception ex)
			{
				return Task.FromException<TReturn>(ex);
			}
		}

		return memberSetup
			.Returns<T1>(SimulatedAsyncCallback);
	}

	public static TDependency ReturnsAsync<TDependency, T1, TReturn>(
		this IMemberSetup<TDependency, ValueTask<TReturn>> memberSetup, Func<T1, TReturn> asyncCallback
	)
		where TDependency : IDependency
	{
		ValueTask<TReturn> SimulatedAsyncCallback(T1 t1)
		{
			try
			{
				var task = Task.FromResult(asyncCallback(t1));
				return new ValueTask<TReturn>(task);
			}
			catch (Exception ex)
			{
				return new ValueTask<TReturn>(Task.FromException<TReturn>(ex));
			}
		}

		return memberSetup
			.Returns<T1>(SimulatedAsyncCallback);
	}

	public static TDependency ReturnsAsync<TDependency, T1, T2, TReturn>(
		this IMemberSetup<TDependency, Task<TReturn>> memberSetup, Func<T1, T2, TReturn> asyncCallback
	)
		where TDependency : IDependency
	{
		Task<TReturn> SimulatedAsyncCallback(T1 t1, T2 t2)
		{
			try
			{
				return Task.FromResult(asyncCallback(t1, t2));
			}
			catch (Exception ex)
			{
				return Task.FromException<TReturn>(ex);
			}
		}

		return memberSetup
			.Returns<T1, T2>(SimulatedAsyncCallback);
	}

	public static TDependency ReturnsAsync<TDependency, T1, T2, TReturn>(
		this IMemberSetup<TDependency, ValueTask<TReturn>> memberSetup, Func<T1, T2, TReturn> asyncCallback
	)
		where TDependency : IDependency
	{
		ValueTask<TReturn> SimulatedAsyncCallback(T1 t1, T2 t2)
		{
			try
			{
				var task = Task.FromResult(asyncCallback(t1, t2));
				return new ValueTask<TReturn>(task);
			}
			catch (Exception ex)
			{
				return new ValueTask<TReturn>(Task.FromException<TReturn>(ex));
			}
		}

		return memberSetup
			.Returns<T1, T2>(SimulatedAsyncCallback);
	}

	public static TDependency ReturnsAsync<TDependency, T1, T2, T3, TReturn>(
		this IMemberSetup<TDependency, Task<TReturn>> memberSetup, Func<T1, T2, T3, TReturn> asyncCallback
	)
		where TDependency : IDependency
	{
		Task<TReturn> SimulatedAsyncCallback(T1 t1, T2 t2, T3 t3)
		{
			try
			{
				return Task.FromResult(asyncCallback(t1, t2, t3));
			}
			catch (Exception ex)
			{
				return Task.FromException<TReturn>(ex);
			}
		}

		return memberSetup
			.Returns<T1, T2, T3>(SimulatedAsyncCallback);
	}

	public static TDependency ReturnsAsync<TDependency, T1, T2, T3, TReturn>(
		this IMemberSetup<TDependency, ValueTask<TReturn>> memberSetup, Func<T1, T2, T3, TReturn> asyncCallback
	)
		where TDependency : IDependency
	{
		ValueTask<TReturn> SimulatedAsyncCallback(T1 t1, T2 t2, T3 t3)
		{
			try
			{
				var task = Task.FromResult(asyncCallback(t1, t2, t3));
				return new ValueTask<TReturn>(task);
			}
			catch (Exception ex)
			{
				return new ValueTask<TReturn>(Task.FromException<TReturn>(ex));
			}
		}

		return memberSetup
			.Returns<T1, T2, T3>(SimulatedAsyncCallback);
	}

	public static TDependency ReturnsAsync<TDependency, T1, T2, T3, T4, TReturn>(
		this IMemberSetup<TDependency, Task<TReturn>> memberSetup, Func<T1, T2, T3, T4, TReturn> asyncCallback
	)
		where TDependency : IDependency
	{
		Task<TReturn> SimulatedAsyncCallback(T1 t1, T2 t2, T3 t3, T4 t4)
		{
			try
			{
				return Task.FromResult(asyncCallback(t1, t2, t3, t4));
			}
			catch (Exception ex)
			{
				return Task.FromException<TReturn>(ex);
			}
		}

		return memberSetup
			.Returns<T1, T2, T3, T4>(SimulatedAsyncCallback);
	}

	public static TDependency ReturnsAsync<TDependency, T1, T2, T3, T4, TReturn>(
		this IMemberSetup<TDependency, ValueTask<TReturn>> memberSetup, Func<T1, T2, T3, T4, TReturn> asyncCallback
	)
		where TDependency : IDependency
	{
		ValueTask<TReturn> SimulatedAsyncCallback(T1 t1, T2 t2, T3 t3, T4 t4)
		{
			try
			{
				var task = Task.FromResult(asyncCallback(t1, t2, t3, t4));
				return new ValueTask<TReturn>(task);
			}
			catch (Exception ex)
			{
				return new ValueTask<TReturn>(Task.FromException<TReturn>(ex));
			}
		}

		return memberSetup
			.Returns<T1, T2, T3, T4>(SimulatedAsyncCallback);
	}

	public static TDependency ReturnsAsync<TDependency, T1, T2, T3, T4, T5, TReturn>(
		this IMemberSetup<TDependency, Task<TReturn>> memberSetup, Func<T1, T2, T3, T4, T5, TReturn> asyncCallback
	)
		where TDependency : IDependency
	{
		Task<TReturn> SimulatedAsyncCallback(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
		{
			try
			{
				return Task.FromResult(asyncCallback(t1, t2, t3, t4, t5));
			}
			catch (Exception ex)
			{
				return Task.FromException<TReturn>(ex);
			}
		}

		return memberSetup
			.Returns<T1, T2, T3, T4, T5>(SimulatedAsyncCallback);
	}

	public static TDependency ReturnsAsync<TDependency, T1, T2, T3, T4, T5, TReturn>(
		this IMemberSetup<TDependency, ValueTask<TReturn>> memberSetup, Func<T1, T2, T3, T4, T5, TReturn> asyncCallback
	)
		where TDependency : IDependency
	{
		ValueTask<TReturn> SimulatedAsyncCallback(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
		{
			try
			{
				var task = Task.FromResult(asyncCallback(t1, t2, t3, t4, t5));
				return new ValueTask<TReturn>(task);
			}
			catch (Exception ex)
			{
				return new ValueTask<TReturn>(Task.FromException<TReturn>(ex));
			}
		}

		return memberSetup
			.Returns<T1, T2, T3, T4, T5> (SimulatedAsyncCallback);
	}

	// https://github.com/Marvin-Brouwer/LeanTest/issues/4
	public static TDependency ReturnsAsync<TDependency, TReturn>(
		this IMemberSetup<TDependency, Task<TReturn>> memberSetup, DynamicFunction<TReturn> asyncCallback
	)
		where TDependency : IDependency
	{
		Task<TReturn> SimulatedAsyncCallback(params object?[] p)
		{
			try
			{
				return Task.FromResult(asyncCallback(p));
			}
			catch (Exception ex)
			{
				return Task.FromException<TReturn>(ex);
			}
		}

		return memberSetup
			.Returns(SimulatedAsyncCallback);
	}

	// https://github.com/Marvin-Brouwer/LeanTest/issues/4
	public static TDependency ReturnsAsync<TDependency, TReturn>(
		this IMemberSetup<TDependency, ValueTask<TReturn>> memberSetup, DynamicFunction<TReturn> asyncCallback
	)
		where TDependency : IDependency
	{
		ValueTask<TReturn> SimulatedAsyncCallback(params object?[] p)
		{
			try
			{
				var task = Task.FromResult(asyncCallback(p));
				return new ValueTask<TReturn>(task);
			}
			catch (Exception ex)
			{
				return new ValueTask<TReturn>(Task.FromException<TReturn>(ex));
			}
		}

		return memberSetup
			.Returns(SimulatedAsyncCallback);
	}
}

#pragma warning restore RCS1047 // Allow Async postfix, because overloading doesn't work otherwise