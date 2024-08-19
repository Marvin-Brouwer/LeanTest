namespace LeanTest.Dependencies.Configuration;

internal sealed class ConfiguredParametersCollection
{
	private readonly IReadOnlyList<ConfiguredParameter> _parameters;
	public int Specificity { get; }
	public int Length => _parameters.Count;

	public ConfiguredParametersCollection(IReadOnlyList<ConfiguredParameter> parameters, int specificity)
	{
		_parameters = parameters;
		Specificity = specificity;
	}

	/// <summary>
	/// Cross-reference each parameter with the respective <see cref="ConfiguredParameter"/>s
	/// </summary>
	public bool ParametersMatch(object?[] parameters)
	{
		for (var i = 0; i < _parameters.Count; i++)
		{
			var parameterFilter = _parameters[i];
			var parameterValue = parameters[i];
			if (!parameterFilter.MatchesRequirements(parameterValue)) return false;
		}

		return true;
	}
}
