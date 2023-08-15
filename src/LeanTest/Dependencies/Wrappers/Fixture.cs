namespace LeanTest.Dependencies.Wrappers;

internal readonly record struct Fixture<TClass> : IFixture<TClass>
	where TClass : notnull
{
	private readonly Func<TClass> _initialValueFunction;
	private List<Func<TClass, TClass>> _mutations;

	public Fixture(Func<TClass> initialValue)
	{
		_initialValueFunction = initialValue;
		_mutations = new ();
	}

	public TClass Instance
	{
		get
		{
			var instance = _initialValueFunction();
			foreach(var mutation in _mutations)
			{
				instance = mutation(instance);
			}
			return instance;
		}
	}

	public IFixture<TClass> AddMutation(Action<TClass> mutation)
	{
		_mutations.Add((instance) =>
		{
			mutation(instance);
			return instance;
		});

		return this;
	}

	public IFixture<TClass> AddMutation(Func<TClass, TClass> mutation)
	{
		_mutations.Add(mutation);
		return this;
	}
}