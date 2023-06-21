using Audio.Other;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Music.Services.Common;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Extensions.Normalizers;
using Music.Services.Database.MongoDb.Models;
using Music.Services.Exceptions;
using Music.Services.Exceptions.Exceptions;
using Music.Services.Files;
using Music.Services.Files.Providers;
using Music.Services.Identity.Common;
using Music.Services.MassTransit.RabbitMq.Consumers.Publishers;
using Music.Shared.Common;
using Music.Shared.DTOs.Tracks;
using Music.Shared.Files;
using Music.Shared.Files.Helpers;
using Music.Shared.Services;
using Music.Tracks.Service.Helpers;
using Music.Tracks.Service.Models;
using Music.Tracks.Service.Requests;

namespace Music.Tracks.Service.Services;

public interface ITracksService
{
    Task<PageResult<TrackDto>> GetPage(long pageNumber, long countPerPage, string? searchQuery = null);
    Task<IReadOnlyCollection<TrackDto>> GetAll(long count = long.MaxValue, string? searchQuery = null);
    Task<IReadOnlyCollection<TrackDto>> GetAll(IEnumerable<Guid> ids);
    Task<IReadOnlyCollection<TrackDto>> GetUserTracks(Guid userId);
    Task<TrackDto> Get(Guid trackId);
    Task ChangeTrackTitle(Guid trackId, string title, Guid userId, HttpContext context);
    Task ChangeTrackPrivacy(Guid trackId, bool isPublic, Guid userId, HttpContext context);
    Task ChangeTrackPublicationDate(Guid trackId, DateOnly publicationDate, Guid userId, HttpContext context);
    Task Delete(Guid trackId, Guid userId, HttpContext context);
    
    Task<TrackDto> Create(Guid ownerId, CreateTrackRequest request);
    Task<int> ChangeTrackFile(Guid trackId, Guid artistId, IFormFile file, HttpContext context);
    Task<PhysicalFileResult> GetTrackFile(Guid trackId);
    Task AddGenreToTrack(Guid trackId, Guid genreId, Guid userId, HttpContext context);
    Task RemoveGenreFromTrack(Guid trackId, Guid genreId, Guid userId, HttpContext context);
    Task AddArtistToTrack(Guid trackId, Guid artistId, Guid userId, HttpContext context);
    Task RemoveArtistFromTrack(Guid trackId, Guid artistId, Guid userId, HttpContext context);
}

public class TracksService : ITracksService
{
    private readonly IRepository<Track> _tracksRepository;
    private readonly FileProviderSettings _fileProviderSettings;
    private readonly IRepository<FileDirectory> _trackFilesRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ITrackNormalizeHelper _trackNormalizeHelper;
    private readonly ServiceSettings _serviceSettings;
    private readonly IRepository<ArtistMongoBase> _artistsRepository;
    private readonly IRepository<GenreMongoBase> _genresRepository;

    public TracksService(IRepository<Track> tracksRepository, FileProviderSettings fileProviderSettings,
        IRepository<FileDirectory> trackFilesRepository, IPublishEndpoint publishEndpoint, 
        ITrackNormalizeHelper trackNormalizeHelper, ServiceSettings serviceSettings, IRepository<ArtistMongoBase> artistsRepository,
        IRepository<GenreMongoBase> genresRepository)
    {
        _tracksRepository = tracksRepository;
        _fileProviderSettings = fileProviderSettings;
        _trackFilesRepository = trackFilesRepository;
        _publishEndpoint = publishEndpoint;
        _trackNormalizeHelper = trackNormalizeHelper;
        _serviceSettings = serviceSettings;
        _artistsRepository = artistsRepository;
        _genresRepository = genresRepository;
    }

    public async Task<PageResult<TrackDto>> GetPage(long pageNumber, long countPerPage, string? searchQuery = null)
    {
        var pageResult = searchQuery == null ? await _tracksRepository.GetAllFromPageAsync(pageNumber, countPerPage)
                : await _tracksRepository.GetAllFromPageAsync(pageNumber, countPerPage,
                    x => x.Title.ToLower().Contains(searchQuery.Trim().ToLower()));
        var trackDtos = await Normalize(pageResult.ItemsOnPage);
        return new PageResult<TrackDto>(pageResult.TotalPagesCount, pageResult.CurrentPageNumber,
            pageResult.TotalItemsCount, trackDtos);
    }

    public async Task<IReadOnlyCollection<TrackDto>> GetAll(long count = 9223372036854775807, string? searchQuery = null)
    {
        if (searchQuery == null)
        {
            var tracks = await _tracksRepository.GetAllAsync();
            return await Normalize(tracks);
        }

        var page = await _tracksRepository.GetAllFromPageAsync(1, count,
            x => x.Title.ToLower().Contains(searchQuery.Trim().ToLower()));
        return await Normalize(page.ItemsOnPage);
    }

    public async Task<IReadOnlyCollection<TrackDto>> GetAll(IEnumerable<Guid> ids)
    {
        var tracks = await _tracksRepository.GetAllAsync(ids);
        return await Normalize(tracks);
    }

    public async Task<IReadOnlyCollection<TrackDto>> GetUserTracks(Guid userId)
    {
        var tracks = await _tracksRepository.GetAllAsync(x => x.OwnerId == userId || x.Artists.Contains(userId));
        return await Normalize(tracks);
    }

    public async Task<TrackDto> Get(Guid trackId)
    {
        var track = await _tracksRepository.GetAsync(trackId)
                    ?? throw new NotFoundException(ExceptionMessages.TrackNotFound);
        return await Normalize(track);
    }

    public async Task ChangeTrackTitle(Guid trackId, string title, Guid userId, HttpContext context)
    {
        var track = await _tracksRepository.GetAsync(trackId) 
                    ?? throw new NotFoundException(ExceptionMessages.TrackNotFound);

        if (!context.IsAdmin() && track.OwnerId != userId)
            throw new ForbiddenException("Artist have ho rights to this track");

        track.Title = title;
        
        await _tracksRepository.UpdateAsync(track);
        await _publishEndpoint.PublishTrackTitleChanged(track);
    }

    public async Task ChangeTrackPrivacy(Guid trackId, bool isPublic, Guid userId, HttpContext context)
    {
        var track = await _tracksRepository.GetAsync(trackId) 
                    ?? throw new NotFoundException(ExceptionMessages.TrackNotFound);

        if (!context.IsAdmin() && track.OwnerId != userId)
            throw new ForbiddenException("Artist have ho rights to this track");

        track.IsPublic = isPublic;
        await _tracksRepository.UpdateAsync(track);
        await _publishEndpoint.PublishTrackPrivacyChanged(track);
    }

    public async Task ChangeTrackPublicationDate(Guid trackId, DateOnly publicationDate, Guid userId, HttpContext context)
    {
        var track = await _tracksRepository.GetAsync(trackId) 
                    ?? throw new NotFoundException(ExceptionMessages.TrackNotFound);

        if (!context.IsAdmin() && track.OwnerId != userId)
            throw new ForbiddenException("Artist have ho rights to this track");

        track.PublicationDate = publicationDate;
        await _tracksRepository.UpdateAsync(track);
        await _publishEndpoint.PublishTrackPublicationDateChanged(track);
    }

    public async Task Delete(Guid trackId, Guid userId, HttpContext context)
    {
        var track = await _tracksRepository.GetAsync(trackId);
        if(track == null) return;
        
        if (!context.IsAdmin() && track.OwnerId != userId)
            throw new ForbiddenException("Artist have ho rights to this track");

        var trackFileInfo = await _trackFilesRepository.GetAsync(trackId);
        if (trackFileInfo != null)
        {
            var fileProvider = new FileProvider(trackFileInfo.RootPath);
            
            foreach (var file in trackFileInfo.Files)
            {
                fileProvider.DeleteFile($"{file.Name}{file.Extension}");
            }

            await _trackFilesRepository.RemoveAsync(trackId);
        }

        await _tracksRepository.RemoveAsync(trackId);

        await _publishEndpoint.PublishTrackDeleted(track);
    }

    public async Task<PhysicalFileResult> GetTrackFile(Guid trackId)
    {
        _ = await _tracksRepository.GetAsync(trackId)
            ?? throw new NotFoundException(ExceptionMessages.TrackNotFound);

        var trackFileInfo = await _trackFilesRepository.GetAsync(trackId)
                            ?? throw new NotFoundException(ExceptionMessages.ImageNotFound);
        
        var provider = new FileProvider(trackFileInfo.RootPath);
        var fileInfo = trackFileInfo.Files.First();
        return provider.ReadFile($"{fileInfo.Name}{fileInfo.Extension}", fileInfo.DataType);
    }

    public async Task AddGenreToTrack(Guid trackId, Guid genreId, Guid userId, HttpContext context)
    {
        var track = await _tracksRepository.GetAsync(trackId)
                    ?? throw new NotFoundException(ExceptionMessages.TrackNotFound);
        
        _ = await _genresRepository.GetAsync(genreId)
            ?? throw new NotFoundException(ExceptionMessages.GenreNotFound);

        if (!context.IsAdmin() && track.OwnerId != userId)
            throw new ForbiddenException("Artist have no rights to change track");
        
        if(track.Genres.Contains(genreId)) return;
        
        track.Genres.Add(genreId);

        await _tracksRepository.UpdateAsync(track);

        await _publishEndpoint.PublishTrackGenreAdded(track, genreId);
    }

    public async Task RemoveGenreFromTrack(Guid trackId, Guid genreId, Guid userId, HttpContext context)
    {
        var track = await _tracksRepository.GetAsync(trackId)
                    ?? throw new NotFoundException(ExceptionMessages.TrackNotFound);
        
        _ = await _genresRepository.GetAsync(genreId)
            ?? throw new NotFoundException(ExceptionMessages.GenreNotFound);

        if (!context.IsAdmin() && track.OwnerId != userId)
            throw new ForbiddenException("Artist have no rights to change track");
        
        if(!track.Genres.Contains(genreId)) return;

        track.Genres.Remove(genreId);

        await _tracksRepository.UpdateAsync(track);

        await _publishEndpoint.PublishTrackGenreRemoved(track, genreId);
    }

    public async Task AddArtistToTrack(Guid trackId, Guid artistId, Guid userId, HttpContext context)
    {
        var track = await _tracksRepository.GetAsync(trackId)
                    ?? throw new NotFoundException(ExceptionMessages.TrackNotFound);

        _ = await _artistsRepository.GetAsync(artistId)
            ?? throw new NotFoundException(ExceptionMessages.ArtistNotFound);
        
        if (!context.IsAdmin() && track.OwnerId != userId)
            throw new ForbiddenException("Artist have no rights to change track");
        
        if(track.OwnerId == artistId) return;
        
        if(track.Artists.Contains(artistId)) return;

        track.Artists.Add(artistId);

        await _tracksRepository.UpdateAsync(track);

        await _publishEndpoint.PublishTrackArtistAdded(track, artistId);
    }

    public async Task RemoveArtistFromTrack(Guid trackId, Guid artistId, Guid userId, HttpContext context)
    {
        var track = await _tracksRepository.GetAsync(trackId)
                    ?? throw new NotFoundException(ExceptionMessages.TrackNotFound);

        _ = await _artistsRepository.GetAsync(artistId)
            ?? throw new NotFoundException(ExceptionMessages.ArtistNotFound);
        
        if (!context.IsAdmin() && track.OwnerId != userId)
            throw new ForbiddenException("Artist have no rights to change track");

        if (track.OwnerId == artistId)
            throw new ForbiddenException("Can't remove track owner");
        
        if(!track.Artists.Contains(artistId)) return;

        track.Artists.Remove(artistId);

        await _tracksRepository.UpdateAsync(track);

        await _publishEndpoint.PublishTrackArtistRemoved(track,artistId);
    }

    public async Task<TrackDto> Create(Guid ownerId, CreateTrackRequest request)
    {
        _ = await _artistsRepository.GetAsync(ownerId)
            ?? throw new NotFoundException(ExceptionMessages.ArtistNotFound);

        CheckFile(request.File);

        int duration;
        await using var stream = request.File.OpenReadStream();

        if (request.Genres != null)
        {
            var genres = await _genresRepository.GetAllAsync(request.Genres.Distinct());
            if (genres.Count != request.Genres.Count)
                throw new NotFoundException(ExceptionMessages.GenreNotFound);
        }
        
        try
        {
            await using var reader = new Mp3FileReader(stream);
            duration = (int)reader.TotalTime.TotalMilliseconds;
        }
        catch (InvalidOperationException)
        {
            throw new BadRequestException("Unsupported media");
        }

        var track = new Track()
        {
            Duration = duration,
            OwnerId = ownerId,
            Title = request.Title,
            Genres = request.Genres ?? new List<Guid>(),
            PublicationDate = request.PublicationDate,
        };

        await _tracksRepository.CreateAsync(track);

        var rootPath = GetRootPath(ownerId);
        
        var provider = new FileProvider(rootPath);
        await provider.WriteFile($"{track.Id}.mp3", stream);

        var fileDirectory = new FileDirectory()
        {
            Id = track.Id,
            RootPath = rootPath,
            Files = new List<FileInfoModel>()
            {
                new()
                {
                    Name = $"{track.Id}",
                    Extension = ".mp3",
                    DataType = FileTypeEnum.Audio.GetDataType()
                }
            }
        };

        await _trackFilesRepository.CreateAsync(fileDirectory);

        await _publishEndpoint.PublishTrackCreated(track);
        
        return await Normalize(track);
    }
    public async Task<int> ChangeTrackFile(Guid trackId, Guid artistId, IFormFile file, HttpContext context)
    {
        var track = await _tracksRepository.GetAsync(trackId)
                    ?? throw new NotFoundException(ExceptionMessages.TrackNotFound);
        
        if (!context.IsAdmin() && track.OwnerId != artistId)
            throw new ForbiddenException("Artist have no rights to change track");
        
        CheckFile(file);
        
        int duration;
        await using var stream = file.OpenReadStream();
        
        try
        {
            await using var reader = new Mp3FileReader(stream);
            duration = (int)reader.TotalTime.TotalMilliseconds;
        }
        catch (InvalidOperationException)
        {
            throw new BadRequestException("Unsupported media");
        }

        var rootPath = GetRootPath(track.OwnerId);
        
        var provider = new FileProvider(rootPath);
        await provider.WriteFile($"{trackId}.mp3", stream);

        var fileInfo = await _trackFilesRepository.GetAsync(trackId);
        if (fileInfo == null)
        {
            fileInfo = new FileDirectory()
            {
                Id = trackId,
                RootPath = rootPath,
                Files = new List<FileInfoModel>
                {
                    new()
                    {
                        Name = $"{trackId}",
                        Extension = ".mp3",
                        DataType = FileTypeEnum.Audio.GetDataType()
                    }
                }
            };
            await _trackFilesRepository.CreateAsync(fileInfo);
        }
        else
        {
            fileInfo.Files.First().ChangingDate = DateTimeOffset.UtcNow;
            await _trackFilesRepository.UpdateAsync(fileInfo);
        }
        
        track.Duration = duration;
        await _tracksRepository.UpdateAsync(track);
        await _publishEndpoint.PublishTrackDurationChanged(track);
        return track.Duration;
    }

    private string GetRootPath(Guid artistId) =>
        $"{_fileProviderSettings.RootPath}/{ServiceNames.Tracks}/{ServiceNames.Artists}/{artistId}/{ServiceNames.Tracks}";

    private async Task<TrackDto> Normalize(ITrackMongo model)
    {
        var allModels = await _trackNormalizeHelper.GetModels(model);
        return model.Normalize(allModels, _serviceSettings);
    }
    
    private async Task<IReadOnlyCollection<TrackDto>> Normalize(IReadOnlyCollection<ITrackMongo> models)
    {
        var allModels = await _trackNormalizeHelper.GetModels(models); 
        return models.Select(x => x.Normalize(allModels, _serviceSettings)).ToList();
    }
    private void CheckFile(IFormFile file)
    {
        if(file.Length == 0)
            throw new BadRequestException("File is empty!");
        if(file.Length > _fileProviderSettings.MaxFileSize)
            throw new BadRequestException($"Maximum file size is {_fileProviderSettings.MaxFileSize} bytes!");
        if(!_fileProviderSettings.AllowedExtensions.Contains(Path.GetExtension(file.FileName)))
            throw new BadRequestException("Unsupported media!");
    }
}