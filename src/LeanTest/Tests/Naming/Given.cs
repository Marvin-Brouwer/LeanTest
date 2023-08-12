namespace LeanTest.Tests.Naming
{
    public readonly record struct Given(string Value) : ITestNamePart
    {
        public string Name => Value;

        public When When(string value) => new(this, value);
        public Then Then(string value) => new(this, value);
    }
}
