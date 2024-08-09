using LeanTest.Hosting;

await TestHost
	.CreateDefault<Program>(args)
	.Build()
	.StartAsync();