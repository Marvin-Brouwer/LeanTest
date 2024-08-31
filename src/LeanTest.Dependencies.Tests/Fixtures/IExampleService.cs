namespace LeanTest.Dependencies.Tests.Fixtures;

public interface IExampleService
{
	public void VoidNoParameters();
	public void VoidWithParameters(string? someString);
	public void VoidWithGenericParameters<T>(T something, bool someBoolean);
	public int ReturnsNoParameters();
	public int ReturnsWithParameters(bool someBoolean);
	public int ReturnsWithGenericParameters<T>(T something, string someString);
	public T GenericNoParameters<T>();
	public T GenericWithParameters<T>(bool someBoolean);
	public T GenericWithGenericParameters<T, U>(U something, int someNumber);
}
