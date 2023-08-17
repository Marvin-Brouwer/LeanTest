namespace ExampleProject.Services;

public interface ISomeThing
{
	void SomeAction(bool input);
	bool DoOtherThing();
	bool DoThing(string value);
	string DoString(string value);
	bool TestManyParam(string a, bool b, int c, DateTime d, byte e);
}
