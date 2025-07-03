using LeanTest.Hosting;

await TestHost
	.CreateDefault(args)
	.AddUnitTests()
	.Build()
	.StartAsync();