#pragma warning disable RCS1047 // Allow Async postfix, because overloading doesn't work otherwise

using LeanTest.Dependencies.Configuration;

namespace LeanTest.Dependencies.Async;

// TODO More parameter options
public static class AsyncMemberSetupExtensions
{
	public static IMemberSetup<TDependency, Task> ExecutesAsync<TDependency>(
		this IMemberSetup<TDependency, Task> memberSetup, Action asyncCallback
	) where TDependency : IDependency
	{
		memberSetup.Returns(() => {
			asyncCallback();
			return Task.CompletedTask;
		});
		return memberSetup;
	}
	public static IMemberSetup<TDependency, ValueTask> ExecutesAsync<TDependency>(
		this IMemberSetup<TDependency, ValueTask> memberSetup, Action asyncCallback
	) where TDependency : IDependency
	{
		memberSetup.Returns(() => {
			asyncCallback();
			return new ValueTask(Task.CompletedTask);
		});
		return memberSetup;
	}
	public static IMemberSetup<TDependency, Task> ExecutesAsync<TDependency,T1>(
		this IMemberSetup<TDependency, Task> memberSetup, Action<T1> asyncCallback
	) where TDependency : IDependency
	{
		Task FakeAsync(T1 t1) {
			asyncCallback(t1);
			return Task.CompletedTask;
		}
		memberSetup.Returns<T1>(FakeAsync);
		return memberSetup;
	}
	public static IMemberSetup<TDependency, ValueTask> ExecutesAsync<TDependency, T1>(
		this IMemberSetup<TDependency, ValueTask> memberSetup, Action<T1> asyncCallback
	) where TDependency : IDependency
	{
		ValueTask FakeAsync(T1 t1)
		{
			asyncCallback(t1);
			return new ValueTask(Task.CompletedTask);
		}
		memberSetup.Returns<T1>(FakeAsync);
		return memberSetup;
	}

	public static IMemberSetup<TDependency, Task<TReturn>> ReturnsAsync<TDependency, TReturn>(
		this IMemberSetup<TDependency, Task<TReturn>> memberSetup, Func<TReturn> asyncCallback
	) where TDependency : IDependency
	{
		Task<TReturn> FakeAsync() => Task.FromResult(asyncCallback());
		memberSetup.Returns(FakeAsync);
		return memberSetup;
	}
	public static IMemberSetup<TDependency, ValueTask<TReturn>> ReturnsAsync<TDependency, TReturn>(
		this IMemberSetup<TDependency, ValueTask<TReturn>> memberSetup, Func<TReturn> asyncCallback
	) where TDependency : IDependency
	{
		ValueTask<TReturn> FakeAsync() => new ValueTask<TReturn>(Task.FromResult(asyncCallback()));
		memberSetup.Returns(FakeAsync);
		return memberSetup;
	}

	public static IMemberSetup<TDependency, Task<TReturn>> ReturnsAsync<TDependency, T1, TReturn>(
		this IMemberSetup<TDependency, Task<TReturn>> memberSetup, Func<T1, TReturn> asyncCallback
	) where TDependency : IDependency
	{
		Task<TReturn> FakeAsync(T1 t1) => Task.FromResult(asyncCallback(t1));
		memberSetup.Returns<T1>(FakeAsync);
		return memberSetup;
	}
	public static IMemberSetup<TDependency, ValueTask<TReturn>> ReturnsAsync<TDependency, T1, TReturn>(
		this IMemberSetup<TDependency, ValueTask<TReturn>> memberSetup, Func<T1, TReturn> asyncCallback
	) where TDependency : IDependency
	{
		ValueTask<TReturn> FakeAsync(T1 t1) => new ValueTask<TReturn>(Task.FromResult(asyncCallback(t1)));
		memberSetup.Returns<T1>(FakeAsync);
		return memberSetup;
	}
}

#pragma warning restore RCS1047 // Allow Async postfix, because overloading doesn't work otherwise