#pragma warning disable RCS1047 // Allow Async postfix, because overloading doesn't work otherwise

using LeanTest.Dependencies.Configuration;

namespace LeanTest.Dependencies.Async;

public static partial class AsyncMemberSetupExtensions
{
	public static TDependency ExecutesAsync<TDependency>(
		this IMemberSetup<TDependency, Task> memberSetup, Action asyncCallback
	)
		where TDependency : IDependency
	{
		Task SimulatedAsyncCallback()
		{
			try
			{
				asyncCallback();
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException(ex);
			}
		}

		return memberSetup
			.Returns(SimulatedAsyncCallback);
	}

	public static TDependency ExecutesAsync<TDependency>(
		this IMemberSetup<TDependency, ValueTask> memberSetup, Action asyncCallback
	)
		where TDependency : IDependency
	{
		ValueTask SimulatedAsyncCallback()
		{
			try
			{
				asyncCallback();
				return new ValueTask(Task.CompletedTask);
			}
			catch (Exception ex)
			{
				return new ValueTask(Task.FromException(ex));
			}
		}

		return memberSetup
			.Returns(SimulatedAsyncCallback);
	}

	public static TDependency ExecutesAsync<TDependency, T1>(
		this IMemberSetup<TDependency, Task> memberSetup, Action<T1> asyncCallback
	)
		where TDependency : IDependency
	{
		Task SimulatedAsyncCallback(T1 t1)
		{
			try
			{
				asyncCallback(t1);
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException(ex);
			}
		}
		return memberSetup
			.Returns<T1>(SimulatedAsyncCallback);
	}

	public static TDependency ExecutesAsync<TDependency, T1>(
		this IMemberSetup<TDependency, ValueTask> memberSetup, Action<T1> asyncCallback
	)
		where TDependency : IDependency
	{
		ValueTask SimulatedAsyncCallback(T1 t1)
		{
			try
			{
				asyncCallback(t1);
				return new ValueTask(Task.CompletedTask);
			}
			catch (Exception ex)
			{
				return new ValueTask(Task.FromException(ex));
			}
		}
		return memberSetup
			.Returns<T1>(SimulatedAsyncCallback);
	}

	public static TDependency ExecutesAsync<TDependency, T1, T2>(
		this IMemberSetup<TDependency, Task> memberSetup, Action<T1, T2> asyncCallback
	)
		where TDependency : IDependency
	{
		Task SimulatedAsyncCallback(T1 t1, T2 t2)
		{
			try
			{
				asyncCallback(t1, t2);
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException(ex);
			}
		}

		return memberSetup
			.Returns<T1, T2>(SimulatedAsyncCallback);
	}

	public static TDependency ExecutesAsync<TDependency, T1, T2>(
		this IMemberSetup<TDependency, ValueTask> memberSetup, Action<T1, T2> asyncCallback
	)
		where TDependency : IDependency
	{
		ValueTask SimulatedAsyncCallback(T1 t1, T2 t2)
		{
			try
			{
				asyncCallback(t1, t2);
				return new ValueTask(Task.CompletedTask);
			}
			catch (Exception ex)
			{
				return new ValueTask(Task.FromException(ex));
			}
		}

		return memberSetup
			.Returns<T1, T2>(SimulatedAsyncCallback);
	}

	public static TDependency ExecutesAsync<TDependency, T1, T2, T3>(
		this IMemberSetup<TDependency, Task> memberSetup, Action<T1, T2, T3> asyncCallback
	)
		where TDependency : IDependency
	{
		Task SimulatedAsyncCallback(T1 t1, T2 t2, T3 t3)
		{
			try
			{
				asyncCallback(t1, t2, t3);
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException(ex);
			}
		}

		return memberSetup
			.Returns<T1, T2, T3>(SimulatedAsyncCallback);
	}

	public static TDependency ExecutesAsync<TDependency, T1, T2, T3>(
		this IMemberSetup<TDependency, ValueTask> memberSetup, Action<T1, T2, T3> asyncCallback
	)
		where TDependency : IDependency
	{
		ValueTask SimulatedAsyncCallback(T1 t1, T2 t2, T3 t3)
		{
			try
			{
				asyncCallback(t1, t2, t3);
				return new ValueTask(Task.CompletedTask);
			}
			catch (Exception ex)
			{
				return new ValueTask(Task.FromException(ex));
			}
		}

		return memberSetup
			.Returns<T1, T2, T3>(SimulatedAsyncCallback);
	}

	public static TDependency ExecutesAsync<TDependency, T1, T2, T3, T4>(
		this IMemberSetup<TDependency, Task> memberSetup, Action<T1, T2, T3, T4> asyncCallback
	)
		where TDependency : IDependency
	{
		Task SimulatedAsyncCallback(T1 t1, T2 t2, T3 t3, T4 t4)
		{
			try
			{
				asyncCallback(t1, t2, t3, t4);
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException(ex);
			}
		}

		return memberSetup
			.Returns<T1, T2, T3, T4>(SimulatedAsyncCallback);
	}

	public static TDependency ExecutesAsync<TDependency, T1, T2, T3, T4>(
		this IMemberSetup<TDependency, ValueTask> memberSetup, Action<T1, T2, T3, T4> asyncCallback
	)
		where TDependency : IDependency
	{
		ValueTask SimulatedAsyncCallback(T1 t1, T2 t2, T3 t3, T4 t4)
		{
			try
			{
				asyncCallback(t1, t2, t3, t4);
				return new ValueTask(Task.CompletedTask);
			}
			catch (Exception ex)
			{
				return new ValueTask(Task.FromException(ex));
			}
		}

		return memberSetup
			.Returns<T1, T2, T3, T4>(SimulatedAsyncCallback);
	}

	public static TDependency ExecutesAsync<TDependency, T1, T2, T3, T4, T5>(
		this IMemberSetup<TDependency, Task> memberSetup, Action<T1, T2, T3, T4, T5> asyncCallback
	)
		where TDependency : IDependency
	{
		Task SimulatedAsyncCallback(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
		{
			try
			{
				asyncCallback(t1, t2, t3, t4, t5);
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException(ex);
			}
		}

		return memberSetup
			.Returns<T1, T2, T3, T4, T5>(SimulatedAsyncCallback);
	}

	public static TDependency ExecutesAsync<TDependency, T1, T2, T3, T4, T5>(
		this IMemberSetup<TDependency, ValueTask> memberSetup, Action<T1, T2, T3, T4, T5> asyncCallback
	)
		where TDependency : IDependency
	{
		ValueTask SimulatedAsyncCallback(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
		{
			try
			{
				asyncCallback(t1, t2, t3, t4, t5);
				return new ValueTask(Task.CompletedTask);
			}
			catch (Exception ex)
			{
				return new ValueTask(Task.FromException(ex));
			}
		}

		return memberSetup
			.Returns<T1, T2, T3, T4, T5>(SimulatedAsyncCallback);
	}

	// https://github.com/Marvin-Brouwer/LeanTest/issues/4
	public static TDependency ExecutesAsync<TDependency>(
		this IMemberSetup<TDependency, Task> memberSetup, Delegate asyncCallback
	)
		where TDependency : IDependency
	{
		Task SimulatedAsyncCallback(params object?[] p)
		{
			try
			{
				asyncCallback.DynamicInvoke(p);
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException(ex);
			}
		}

		return memberSetup
			.Returns(SimulatedAsyncCallback);
	}

	// https://github.com/Marvin-Brouwer/LeanTest/issues/4
	public static TDependency ExecutesAsync<TDependency>(
		this IMemberSetup<TDependency, ValueTask> memberSetup, Delegate asyncCallback
	)
		where TDependency : IDependency
	{
		ValueTask SimulatedAsyncCallback(params object?[] p)
		{
			try
			{
				asyncCallback.DynamicInvoke(p);
				return new ValueTask(Task.CompletedTask);
			}
			catch(Exception ex)
			{
				return new ValueTask(Task.FromException(ex));
			}
		}

		return memberSetup
			.Returns(SimulatedAsyncCallback);
	}
}

#pragma warning restore RCS1047 // Allow Async postfix, because overloading doesn't work otherwise