namespace ExampleProject.Services;

public interface ISomeThing
{
	bool DoOtherThing();
	bool DoThing(string value);
	bool TestManyParam(string a, bool b, int c, DateTime d, byte e);
}
