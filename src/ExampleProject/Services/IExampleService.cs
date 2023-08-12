namespace ExampleProject.Services;

public interface IExampleService
{
	Task<string> DoThing(string thing);
}