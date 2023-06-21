using Music.Artists.Service.Helpers;
using Music.Artists.Service.Models;
using Music.Artists.Service.Services;
using Music.Services.Common;
using Music.Services.Database.MongoDb.Extensions.Helpers.Album;
using Music.Services.Database.MongoDb.Extensions.Normalizers.Media;
using Music.Services.Database.MongoDb.Extensions.Services;
using Music.Services.Database.MongoDb.Helpers;
using Music.Services.Database.MongoDb.Models;
using Music.Services.DTOs.Extensions;
using Music.Services.Exceptions.Helpers;
using Music.Services.Identity.Jwt;
using Music.Services.MassTransit.RabbitMq;
using Music.Services.Models.Media;
using Music.Shared.DTOs.Media.Models;

namespace Music.Artists.Service;

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
            .AddMongoRepository<Artist>("Artists")
            .AddMongoRepository<UserMongoBase>("Users")
            .AddMongoRepository<TrackMongoBase>("Tracks")
            .AddMongoRepository<GenreMongoBase>("Genres")
            .AddMongoRepository<CustomArtistInfo>("ArtistInfos")
            .AddMongoRepository<TrackInfo>("TrackInfos")
            .AddMongoRepository<AlbumInfo>("AlbumInfos")
            .AddMassTransitWithRabbitMq();
        
        services.AddScoped<IArtistsService, ArtistsService>();
        services.AddScoped<IArtistNormalizeHelper, ArtistNormalizeHelper>();
        services.AddScoped<IMediaServiceBase<MediaArtistDto>, MediaServiceBase<MediaArtistDto, CustomArtistInfo, Artist, UserMongoBase>>();
        services.AddScoped<IMediaNormalizer<MediaArtistDto, CustomArtistInfo, Artist>, ArtistMediaNormalizerBase<IArtistNormalizeHelper, 
            Artist, AlbumMongoBase, TrackMongoBase, GenreMongoBase, AlbumInfo, CustomArtistInfo, TrackInfo>>();
        
        services.AddScoped<IMediaServiceBase<MediaTrackDto>, MediaServiceBase<MediaTrackDto, TrackInfo, TrackMongoBase, UserMongoBase>>();
        services.AddScoped<ITrackNormalizeHelper, TrackNormalizeModelHelper>();
        services.AddScoped<IMediaNormalizer<MediaTrackDto, TrackInfo, TrackMongoBase>, TrackMediaNormalizerBase<ITrackNormalizeHelper, 
            Artist, AlbumMongoBase, TrackMongoBase, GenreMongoBase, AlbumInfo, CustomArtistInfo, TrackInfo>>();
        
        JwtTokenHandler.SetKey(_configuration.GetSettings<IdentitySettings>().Key);
        
        services.AddJwtAuthentication();

        services.AddSingleton(_configuration.GetSettings<ServiceSettings>());
        
        services.AddControllers(options => { options.SuppressAsyncSuffixInActionNames = false; })
            .AddJsonOptions(
                options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
        services.AddLogging();
        
        services.AddSwaggerJwtAuthentication("Music.Artists.Service", "v1");
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