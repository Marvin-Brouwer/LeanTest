namespace LeanTest.Example.Services;

public interface IExampleService
{
	Task<string> DoThing(string thing);
}