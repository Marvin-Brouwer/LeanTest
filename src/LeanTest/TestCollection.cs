using System.Collections;

using LeanTest.Tests;

namespace LeanTest
{
    public sealed class TestCollection : IReadOnlyCollection<ITestScenario>
    {
        private readonly ITestScenario[] _scenarios;
        public TestCollection(params ITestScenario[] scenarios)
        {
            _scenarios = scenarios;
        }

        public int Count => ((IReadOnlyCollection<ITestScenario>)_scenarios).Count;

        public IEnumerator<ITestScenario> GetEnumerator()
        {
            return ((IEnumerable<ITestScenario>)_scenarios).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _scenarios.GetEnumerator();
        }
    }
}
