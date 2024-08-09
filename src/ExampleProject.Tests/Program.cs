using LeanTest.Hosting;

await TestHost
	.CreateDefault<Program>(args)
// TODO:
//	.AddSonarCloudCoverageCalculator()
	.Build()
//	.CollectCoverage();
	.StartAsync();