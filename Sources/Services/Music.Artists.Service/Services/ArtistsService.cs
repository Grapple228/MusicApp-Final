using MassTransit;
using Music.Artists.Service.Helpers;
using Music.Artists.Service.Models;
using Music.Services.Common;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Extensions.Normalizers;
using Music.Services.Database.MongoDb.Models;
using Music.Services.Exceptions;
using Music.Services.Exceptions.Exceptions;
using Music.Services.MassTransit.Contracts;
using Music.Shared.Common;
using Music.Shared.DTOs.Artists;

namespace Music.Artists.Service.Services;

public interface IArtistsService
{
    Task<PageResult<ArtistDto>> GetPage(long pageNumber, long countPerPage, string? searchQuery = null);
    Task<IReadOnlyCollection<ArtistDto>> GetAll(long count = long.MaxValue, string? searchQuery = null);
    Task<ArtistDto> Get(Guid artistId);
}

public class ArtistsService : IArtistsService
{
    private readonly ServiceSettings _serviceSettings;
    private readonly IArtistNormalizeHelper _artistNormalizeHelper;
    private readonly IRepository<Artist> _artistsRepository;

    public ArtistsService(ServiceSettings serviceSettings,
        IArtistNormalizeHelper artistNormalizeHelper, IRepository<Artist> artistsRepository)
    {
        _serviceSettings = serviceSettings;
        _artistNormalizeHelper = artistNormalizeHelper;
        _artistsRepository = artistsRepository;
    }

    public async Task<PageResult<ArtistDto>> GetPage(long pageNumber, long countPerPage, string? searchQuery = null)
    {
        var pageResult = searchQuery == null ? await _artistsRepository.GetAllFromPageAsync(pageNumber, countPerPage)
            : await _artistsRepository.GetAllFromPageAsync(pageNumber, countPerPage,
                x => x.Name.ToLower().Contains(searchQuery.Trim().ToLower()));
        var artistDtos = await Normalize(pageResult.ItemsOnPage);
        return new PageResult<ArtistDto>(pageResult.TotalPagesCount, pageResult.CurrentPageNumber,
            pageResult.TotalItemsCount, artistDtos);
    }

    public async Task<IReadOnlyCollection<ArtistDto>> GetAll(long count = 9223372036854775807, string? searchQuery = null)
    {
        if (searchQuery == null)
        {
            var artists = await _artistsRepository.GetAllAsync();
            return await Normalize(artists);
        }

        var page = await _artistsRepository.GetAllFromPageAsync(1, count,
            x => x.Name.ToLower().Contains(searchQuery.Trim().ToLower()));
        return await Normalize(page.ItemsOnPage);
    }

    public async Task<ArtistDto> Get(Guid artistId)
    {
        var artist = await _artistsRepository.GetAsync(artistId)
                    ?? throw new NotFoundException(ExceptionMessages.ArtistNotFound);
        return await Normalize(artist);
    }

    private async Task<ArtistDto> Normalize(IArtistMongo model)
    {
        var allModels = await _artistNormalizeHelper.GetModels(model);
        return model.Normalize(allModels, _serviceSettings);
    }
    
    private async Task<IReadOnlyCollection<ArtistDto>> Normalize(IReadOnlyCollection<IArtistMongo> models)
    {
        var allModels = await _artistNormalizeHelper.GetModels(models); 
        return models.Select(x => x.Normalize(allModels, _serviceSettings)).ToList();
    }
}