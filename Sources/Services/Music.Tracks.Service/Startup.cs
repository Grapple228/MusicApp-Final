using Music.Services.Common;
using Music.Services.Database.MongoDb.Extensions.Normalizers.Media;
using Music.Services.Database.MongoDb.Extensions.Services;
using Music.Services.Database.MongoDb.Helpers;
using Music.Services.Database.MongoDb.Models;
using Music.Services.DTOs.Extensions;
using Music.Services.DTOs.Extensions.Converters;
using Music.Services.Exceptions.Helpers;
using Music.Services.Files;
using Music.Services.Identity.Jwt;
using Music.Services.MassTransit.RabbitMq;
using Music.Services.Models.Media;
using Music.Shared.DTOs.Media.Models;
using Music.Tracks.Service.Controllers;
using Music.Tracks.Service.Helpers;
using Music.Tracks.Service.Hubs;
using Music.Tracks.Service.Models;
using Music.Tracks.Service.Services;

namespace Music.Tracks.Service;

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
            .AddMongoRepository<UserMongoBase>("Users")
            .AddMongoRepository<Track>("Tracks")
            .AddMongoRepository<FileDirectory>("TrackFiles")
            .AddMongoRepository<ArtistInfo>("ArtistInfos")
            .AddMongoRepository<CustomTrackInfo>("TrackInfos")
            .AddMongoRepository<AlbumInfo>("AlbumInfos")
            .AddMongoRepository<RoomBase>("Rooms")
            .AddMongoRepository<RoomUser>("RoomUsers")
            .AddMassTransitWithRabbitMq();
        
        services.AddScoped<ITracksService, TracksService>();
        services.AddScoped<ITrackNormalizeHelper, TrackNormalizeHelper>();
        
        services.AddScoped<IMediaServiceBase<MediaTrackDto>, MediaServiceBase<MediaTrackDto, CustomTrackInfo, Track, UserMongoBase>>();
        services.AddScoped<IMediaNormalizer<MediaTrackDto, CustomTrackInfo, Track>, TrackMediaNormalizerBase<ITrackNormalizeHelper, 
            ArtistMongoBase, AlbumMongoBase, Track, GenreMongoBase, AlbumInfo, ArtistInfo, CustomTrackInfo>>();

        services.AddSignalR();
        
        JwtTokenHandler.SetKey(_configuration.GetSettings<IdentitySettings>().Key);
        
        services.AddJwtAuthentication();

        services.AddSingleton(_configuration.GetSettings<ServiceSettings>());
        services.AddSingleton(_configuration.GetSettings<FileProviderSettings>());

        services.AddControllers(options => { options.SuppressAsyncSuffixInActionNames = false; })
            .AddJsonOptions(
                options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
        services.AddLogging();
        
        services.AddSwaggerJwtAuthentication("Music.Tracks.Service", "v1");
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

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHub<StreamingHub>("api/streamingHub"); 
        });
    }
}