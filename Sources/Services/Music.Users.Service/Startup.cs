using Music.Services.Common;
using Music.Services.Database.MongoDb.Helpers;
using Music.Services.Database.MongoDb.Models;
using Music.Services.Exceptions.Helpers;
using Music.Services.Identity.Jwt;
using Music.Services.MassTransit.RabbitMq;
using Music.Services.Models.Media;
using Music.Users.Service.Helpers;
using Music.Users.Service.Services;

namespace Music.Users.Service;

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
            .AddMongoRepository<AlbumMongoBase>("Albums")
            .AddMongoRepository<ArtistMongoBase>("Artists")
            .AddMongoRepository<GenreMongoBase>("Genres")
            .AddMongoRepository<PlaylistMongoBase>("Playlists")
            .AddMongoRepository<TrackMongoBase>("Tracks")
            .AddMongoRepository<UserMongoBase>("Users")
            .AddMongoRepository<AlbumInfo>("AlbumInfos")
            .AddMongoRepository<PlaylistInfo>("PlaylistInfos")
            .AddMongoRepository<TrackInfo>("TrackInfos")
            .AddMongoRepository<ArtistInfo>("ArtistInfos")
            .AddMassTransitWithRabbitMq();

        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IUserNormalizeHelper, UserNormalizeHelper>();
        
        JwtTokenHandler.SetKey(_configuration.GetSettings<IdentitySettings>().Key);
        
        services.AddJwtAuthentication();
        
        services.AddSingleton(_configuration.GetSettings<ServiceSettings>());

        services.AddControllers(options => { options.SuppressAsyncSuffixInActionNames = false; })
            .AddJsonOptions(
                options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
        services.AddLogging();
        
        services.AddSwaggerJwtAuthentication("Music.Users.Service", "v1");
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