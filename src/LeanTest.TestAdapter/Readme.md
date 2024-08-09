# LeanTest.TestAdapter

This project is purely for VisualStudio's test adapter to work.

Requirements:
- The projects name needs to end with `".TestAdapter"`
- The project needs to contain an implementation of `ITestDiscoverer`, decorated with a `DefaultExecutorUriAttribute`
- The project needs to contain an implementation of `ITestExecutor`, decorated with a `ExtensionUriAttribute`
- Both aforementioned attributes need to reference the same URL (possibly to invoke a named pipe)

We've put all the logic into a base class so this project only inherits and registers.
Most of the requirements are discovered by trial and error, credits to [Patrick Lioi](https://github.com/plioi),
the creator of [Fixie](https://github.com/fixie/fixie)__another testing framework__ for figuring this out and sharing it with us.

Similar to Fixie, we've included `Microsoft.NET.Test.Sdk` as a dependency so the test projects don't have to.

> ![NOTE]
> TODO: Create a visual studio template, similar to xUnit, nUnit and VsTest.
