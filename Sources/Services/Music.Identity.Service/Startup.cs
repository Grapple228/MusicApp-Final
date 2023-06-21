using System.Text.Json.Serialization;
using Music.Identity.Service.Models;
using Music.Identity.Service.Services;
using Music.Services.Common;
using Music.Services.Database.MongoDb.Helpers;
using Music.Services.Exceptions.Helpers;
using Music.Services.Identity.Jwt;
using Music.Services.MassTransit.RabbitMq;

namespace Music.Identity.Service;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMongo()
            .AddMongoRepository<Role>("Roles")
            .AddMongoRepository<Device>("Devices")
            .AddMongoRepository<DeviceType>("DeviceTypes")
            .AddMongoRepository<Password>("Passwords")
            .AddMongoRepository<Token>("Tokens")
            .AddMongoRepository<User>("Users")
            .AddMongoRepository<Email>("Emails")
            .AddMassTransitWithRabbitMq();

        services.AddJwtAuthentication();
        
        services.AddScoped<IRolesService, RolesService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IDevicesService, DevicesService>();
        services.AddScoped<ITokensService, TokensService>();
        services.AddScoped<IUsersService, UsersService>();
        
        services.AddSingleton(_configuration.GetSettings<ServiceSettings>());
        var identitySettings = _configuration.GetSettings<IdentitySettings>();
        services.AddSingleton(identitySettings);
        
        JwtTokenHandler.SetKey(identitySettings.Key);

        services.AddScoped<IdentityService>();
        
        services.AddControllers(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;
            })
            .AddJsonOptions(
                options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
        services.AddLogging();
        
        services.AddSwaggerJwtAuthentication("Music.Identity.Service", "v1");
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
        app.UseRouting();
        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}