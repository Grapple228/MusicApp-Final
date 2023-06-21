using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Music.Identity.Service.Services;

namespace Music.Identity.Service;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var usersService = services.GetRequiredService<IUsersService>();
        await usersService.CreateDefaultUsers();
        await host.RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}