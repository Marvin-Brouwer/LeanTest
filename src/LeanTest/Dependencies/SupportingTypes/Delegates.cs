namespace LeanTest.Dependencies.SupportingTypes;

public delegate void DynamicAction(params object?[] parameters);
public delegate TReturn DynamicFunction<TReturn>(params object?[] parameters);
