namespace LeanTest.Dependencies;

public sealed class Fixture<TClass> :
	IDependency<TClass>
{
	private readonly Func<TClass> _initialValueFunction;
	private readonly List<Func<TClass, TClass>> _mutations;

	public Fixture(Func<TClass> initialValue)
	{
		_initialValueFunction = initialValue;
		_mutations = new();
	}

	/// <inheritdoc />
	/// <remarks>
	/// <b>This will apply all mutations on a new instance very time you call this.</b>
	/// </remarks>
	public TClass Instance
	{
		get
		{
			var instance = _initialValueFunction();
			foreach (var mutation in _mutations)
			{
				instance = mutation(instance);
			}
			return instance;
		}
	}

	public Fixture<TClass> AddMutation(Action<TClass> mutation)
	{
		_mutations.Add((instance) =>
		{
			mutation(instance);
			return instance;
		});

		return this;
	}

	public Fixture<TClass> AddMutation(Func<TClass, TClass> mutation)
	{
		_mutations.Add(mutation);
		return this;
	}
}