using Music.Albums.Service.Helpers;
using Music.Albums.Service.Models;
using Music.Albums.Service.Services;
using Music.Services.Common;
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

namespace Music.Albums.Service;

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
            .AddMongoRepository<Album>("Albums")
            .AddMongoRepository<CustomAlbumInfo>("AlbumInfos")
            .AddMongoRepository<ArtistInfo>("ArtistInfos")
            .AddMongoRepository<TrackInfo>("TrackInfos")
            .AddMongoRepository<TrackMongoBase>("Tracks")
            .AddMongoRepository<UserMongoBase>("Users")
            .AddMongoRepository<ArtistMongoBase>("Artists")
            .AddMongoRepository<GenreMongoBase>("Genres")
            .AddMassTransitWithRabbitMq();

        services.AddScoped<IAlbumsService,AlbumsService>();
        services.AddScoped<IMediaServiceBase<MediaAlbumDto>, MediaServiceBase<MediaAlbumDto, CustomAlbumInfo, Album, UserMongoBase>>();
        services.AddScoped<IAlbumNormalizeHelper, AlbumNormalizeModelHelper>();
        services.AddScoped<IMediaNormalizer<MediaAlbumDto, CustomAlbumInfo, Album>, AlbumMediaNormalizerBase<IAlbumNormalizeHelper, 
                ArtistMongoBase, Album, TrackMongoBase, GenreMongoBase, CustomAlbumInfo, ArtistInfo, TrackInfo>>();
        
        services.AddScoped<IMediaServiceBase<MediaTrackDto>, MediaServiceBase<MediaTrackDto, TrackInfo, TrackMongoBase, UserMongoBase>>();
        services.AddScoped<ITrackNormalizeHelper, TrackNormalizeModelHelper>();
        services.AddScoped<IMediaNormalizer<MediaTrackDto, TrackInfo, TrackMongoBase>, TrackMediaNormalizerBase<ITrackNormalizeHelper, 
            ArtistMongoBase, Album, TrackMongoBase, GenreMongoBase, CustomAlbumInfo, ArtistInfo, TrackInfo>>();
        
        JwtTokenHandler.SetKey(_configuration.GetSettings<IdentitySettings>().Key);
        services.AddJwtAuthentication();

        services.AddSingleton(_configuration.GetSettings<ServiceSettings>());
        
        services.AddControllers(options => { options.SuppressAsyncSuffixInActionNames = false; })
            .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
        
        services.AddLogging();

        services.AddSwaggerJwtAuthentication("Music.Albums.Service", "v1");
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