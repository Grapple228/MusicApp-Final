using System.Text.Json.Serialization;
using Music.Image.Service.Models;
using Music.Image.Service.Models.Directories;
using Music.Image.Service.Services;
using Music.Services.Common;
using Music.Services.Database.MongoDb.Helpers;
using Music.Services.Exceptions.Helpers;
using Music.Services.Files;
using Music.Services.Identity.Jwt;
using Music.Services.MassTransit.RabbitMq;
using Music.Shared.DTOs.Albums;
using Music.Shared.DTOs.Artists;
using Music.Shared.DTOs.Playlists;
using Music.Shared.DTOs.Tracks;
using Music.Shared.DTOs.Users;

namespace Music.Image.Service;

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
            .AddMongoRepository<Track>("Tracks")
            .AddMongoRepository<Album>("Albums")
            .AddMongoRepository<Artist>("Artists")
            .AddMongoRepository<Playlist>("Playlists")
            .AddMongoRepository<User>("Users")
            .AddMongoRepository<TrackFileDirectory>("TrackImages")
            .AddMongoRepository<AlbumFileDirectory>("AlbumImages")
            .AddMongoRepository<ArtistFileDirectory>("ArtistImages")
            .AddMongoRepository<PlaylistFileDirectory>("PlaylistImages")
            .AddMongoRepository<UserFileDirectory>("UserImages")
            .AddMassTransitWithRabbitMq();

        services.AddScoped<IImageService, ImageService>();
        
        JwtTokenHandler.SetKey(_configuration.GetSettings<IdentitySettings>().Key);

        services.AddJwtAuthentication();
        
        services.AddSingleton(_configuration.GetSettings<ServiceSettings>());
        services.AddSingleton(_configuration.GetSettings<FileProviderSettings>());

        services.AddControllers(options => { options.SuppressAsyncSuffixInActionNames = false; })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

        services.AddLogging();
        services.AddSwaggerJwtAuthentication("Music.Images.Service", "v1");
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