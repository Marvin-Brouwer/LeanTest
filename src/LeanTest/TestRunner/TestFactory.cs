using LeanTest.Extensions;
using LeanTest.Indexing;
using LeanTest.Tests;

using Microsoft.Extensions.Logging;

using System.Reflection;

namespace LeanTest.TestRunner
{
    internal class TestFactory
    {
        private readonly ILoggerFactory _loggerFactory;

        public TestFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public IEnumerable<ITestScenario> InitializeScenarios(Assembly assembly, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) yield break;
            var assemblySenarios = InitializeScenariosForAssembly(assembly, cancellationToken);
            if (cancellationToken.IsCancellationRequested) yield break;

            foreach (var scenario in assemblySenarios)
            {
                if (cancellationToken.IsCancellationRequested) yield break;
                yield return scenario;
            }
        }

        private IEnumerable<ITestScenario> InitializeScenariosForAssembly(Assembly assembly, CancellationToken cancellationToken)
        {
            var suiteTypes = SuiteIndexer.IndexTestSuites(assembly, cancellationToken);
            var suites = InitializeSuites(suiteTypes, cancellationToken);
            if (cancellationToken.IsCancellationRequested) yield break;

            // Enumerate tests after logger's been inserted
            foreach (var suite in suites)
            {
                if (cancellationToken.IsCancellationRequested) yield break;
                foreach (var test in suite.Tests)
                {
                    if (cancellationToken.IsCancellationRequested) yield break;
                    yield return test;
                }
            }
        }

        private IEnumerable<ITestSuite> InitializeSuites(IEnumerable<Type> suiteTypes, CancellationToken cancellationToken)
        {
            foreach (var suiteType in suiteTypes)
            {
                if (cancellationToken.IsCancellationRequested) yield break;
                yield return InitializeSuite(suiteType);
            }
        }




        private ITestSuite InitializeSuite(Type suiteType)
        {
            var instance = Activator.CreateInstance(suiteType) as ITestSuite;
            if (instance is null) throw new NotSupportedException("Impossible code reached");

            return instance
                .InjectLogger(_loggerFactory);
        }
    }
}

