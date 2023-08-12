using FluentAssertions;

using LeanTest.Dependencies;
using LeanTest.Example.Services;

namespace LeanTest.Example.Tests.TestSuites.Services;

// TODO figure out if this can be forced with an attribute or an analyzer
#pragma warning disable RCS1032 // Remove redundant parentheses.
#pragma warning disable IDE0055 // Remove redundant parentheses.
#pragma warning disable RCS1021 // Convert lambda expression body to expression body.
public sealed record ExampleServiceTests : TestSuite<ExampleService>
{
    private readonly IStub<ISomeThing> _someStub;
    private readonly ISpy<ISomeThing> _someSpy;
    private readonly IMock<ISomeThing> _someMock;
    private readonly IServiceOutOfScope _outOfScopeDummy;

    public ExampleServiceTests()
    {
        _someStub = Stub.Of<ISomeThing>();
        _someSpy = Spy.On<ISomeThing>(new SomeThing());
        _someMock = Mock.Of<ISomeThing>();

        _outOfScopeDummy = Dummy.Of<IServiceOutOfScope>();
    }

    public override TestCollection Tests => new(
        Test(sut => sut.DoThing, Given("Something").When("Condition").Then("Result"),
            Arrange(() =>
            {
                _someStub
                    .Setup(x => x.DoThing(Parameter.Is<string>()), () => true)
                    .Setup(x => x.DoOtherThing(), () => true);

                var sut = new ExampleService(_someStub.Instance, _outOfScopeDummy, TestOutputLogger);
                var input = "SomeString";
                var expected = "SomeString";

                return (sut, input, expected);
            }),
            Act(async (ExampleService sut, string input) =>
			(
                await sut.DoThing(input)
            )),
            Assert((string result, string expected) =>
            {
                result.Should().Be(expected);

				_someSpy
					.VerifyOnce(x => x.DoThing(Parameter.Is<string>()))
					.VerifyNoOtherCalls();
            })
        )
    );
}