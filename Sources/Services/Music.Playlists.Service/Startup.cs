using Music.Playlists.Service.Helpers;
using Music.Playlists.Service.Models;
using Music.Playlists.Service.Services;
using Music.Services.Common;
using Music.Services.Database.MongoDb.Extensions.Normalizers.Media;
using Music.Services.Database.MongoDb.Extensions.Services;
using Music.Services.Database.MongoDb.Helpers;
using Music.Services.Database.MongoDb.Models;
using Music.Services.DTOs.Extensions;
using Music.Services.Exceptions.Helpers;
using Music.Services.Identity.Jwt;
using Music.Services.MassTransit.RabbitMq;
using Music.Services.Models;
using Music.Services.Models.Media;
using Music.Shared.DTOs.Media.Models;

namespace Music.Playlists.Service;

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
            .AddMongoRepository<Playlist>("Playlists")
            .AddMongoRepository<TrackMongoBase>("Tracks")
            .AddMongoRepository<UserMongoBase>("Users")
            .AddMongoRepository<TrackInfo>("TrackInfos")
            .AddMongoRepository<AlbumInfo>("AlbumInfos")
            .AddMongoRepository<ArtistInfo>("ArtistInfos")
            .AddMongoRepository<CustomPlaylistInfo>("PlaylistInfos")
            .AddMassTransitWithRabbitMq();
        
        services.AddScoped<IPlaylistsService, PlaylistsService>();
        services.AddScoped<IPlaylistNormalizeHelper, PlaylistNormalizeHelper>();
        services.AddScoped<IMediaServiceBase<MediaPlaylistDto>, MediaServiceBase<MediaPlaylistDto, CustomPlaylistInfo, Playlist, UserMongoBase>>();
        services.AddScoped<IMediaNormalizer<MediaPlaylistDto, CustomPlaylistInfo, Playlist>, PlaylistMediaNormalizerBase<IPlaylistNormalizeHelper, 
            ArtistMongoBase, AlbumMongoBase, TrackMongoBase, GenreMongoBase, UserMongoBase, Playlist, CustomPlaylistInfo, TrackInfo>>();
        
        services.AddScoped<IMediaServiceBase<MediaTrackDto>, MediaServiceBase<MediaTrackDto, TrackInfo, TrackMongoBase, UserMongoBase>>();
        services.AddScoped<ITrackNormalizeHelper, TrackNormalizeModelHelper>();
        services.AddScoped<IMediaNormalizer<MediaTrackDto, TrackInfo, TrackMongoBase>, TrackMediaNormalizerBase<ITrackNormalizeHelper, 
            ArtistMongoBase, AlbumMongoBase, TrackMongoBase, GenreMongoBase, AlbumInfo, ArtistInfo, TrackInfo>>();
        
        JwtTokenHandler.SetKey(_configuration.GetSettings<IdentitySettings>().Key);
        
        services.AddJwtAuthentication();
        
        services.AddSingleton(_configuration.GetSettings<ServiceSettings>());

        services.AddControllers(options => { options.SuppressAsyncSuffixInActionNames = false; })
            .AddJsonOptions(
                options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
        services.AddLogging();
        
        services.AddSwaggerJwtAuthentication("Music.Playlists.Service", "v1");
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