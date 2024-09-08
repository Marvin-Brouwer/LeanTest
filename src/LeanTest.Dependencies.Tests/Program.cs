using LeanTest.Hosting;

await TestHost
	.CreateDefault(args)
	.Build()
	.StartAsync();