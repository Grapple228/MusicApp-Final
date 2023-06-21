using Microsoft.OpenApi.Models;
using Music.Services.Common;
using Music.Services.Exceptions.Helpers;
using Music.Services.Identity.Jwt;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace Music.Gateway;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging();
        services.AddSingleton(_ => _configuration.GetSettings<ServiceSettings>());
        
        JwtTokenHandler.SetKey(_configuration.GetSettings<IdentitySettings>().Key);
        
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Music.Gateway", Version = "v1" });
        });
        
        services.AddJwtAuthentication();

        services.AddEndpointsApiExplorer();
        
        services.AddOcelot(_configuration)
            .AddCacheManager(x => { x.WithDictionaryHandle(); });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseHsts();
        }

        app.UseExceptionMiddleware();
        
        app.UseStatusCodePages();
        app.UseAuthentication();
        app.UseHttpsRedirection();
        app.UseHttpMethodOverride();
        app.UseRouting();
        app.UseWebSockets();
        app.UseAuthorization();
        
        app.UseOcelot().Wait();
    }
}