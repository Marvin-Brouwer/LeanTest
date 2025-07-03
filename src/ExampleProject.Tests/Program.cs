using LeanTest.Hosting;

var testHost = TestHost
	.CreateDefault(args)
	.AddUnitTests();

// TODO:
//	testHost
//		.AddSonarCloudCoverageCalculator()

var testRunner = testHost
	.Build();

//	testRunner
//		.CollectCoverage();

await testRunner
	.StartAsync();