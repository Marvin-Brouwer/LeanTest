namespace LeanTest.Tests.Naming
{
    public readonly record struct When(Given Given) : ITestNamePart
    {
        private readonly List<string> _values = new();

        private IEnumerable<string> Values()
        {
            yield return Given.Name;
            foreach (var value in _values) yield return value;
        }
        public string Name => String.Join("_", Values());


        public When(Given given, string value) : this(given)
        {
            _values.Add(value);
        }

        public When And(string value)
        {
            _values.Add(value);
            return this;
        }

        public Then Then(string value) => new Then(this, value);
    }
}
