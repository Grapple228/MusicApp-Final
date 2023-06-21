using Music.Gateway;

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults((webBuilder) =>
        {
            webBuilder.UseStartup<Startup>()
                .ConfigureAppConfiguration(c =>
                {
                    c.AddJsonFile("ocelot.json", false, true);
                });
        });

CreateHostBuilder(args).Build().Run();