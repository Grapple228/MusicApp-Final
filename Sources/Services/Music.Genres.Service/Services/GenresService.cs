using MassTransit;
using Music.Genres.Service.Models;
using Music.Services.Database.Common.Repositories;
using Music.Services.DTOs.Extensions.Converters;
using Music.Services.Exceptions;
using Music.Services.Exceptions.Exceptions;
using Music.Services.MassTransit.Contracts;
using Music.Services.MassTransit.RabbitMq.Consumers.Publishers;
using Music.Services.Models;
using Music.Shared.Common;
using Music.Shared.DTOs.Genres;
using Music.Shared.DTOs.Requests.Genres;

namespace Music.Genres.Service.Services;

public interface IGenresService
{
    Task<PageResult<GenreDto>> GetPage(long pageNumber, long countPerPage);
    Task<IReadOnlyCollection<GenreDto>> GetAll();
    Task<GenreDto> Get(Guid genreId);
    Task<GenreDto> Create(GenreCreateRequest request);
    Task<GenreDto> Update(Guid genreId, GenreUpdateRequest request);
    Task Delete(Guid genreId);
}

public class GenresService : IGenresService
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IRepository<Genre> _genresRepository;

    public GenresService(IPublishEndpoint publishEndpoint, IRepository<Genre> genresRepository)
    {
        _publishEndpoint = publishEndpoint;
        _genresRepository = genresRepository;
    }

    public async Task<PageResult<GenreDto>> GetPage(long pageNumber, long countPerPage)
    {
        var pageResult = await _genresRepository.GetAllFromPageAsync(pageNumber, countPerPage);
        var genreDtos = Normalize(pageResult.ItemsOnPage);
        return new PageResult<GenreDto>(pageResult.TotalPagesCount, pageResult.CurrentPageNumber,
            pageResult.TotalItemsCount, genreDtos);
    }

    public async Task<IReadOnlyCollection<GenreDto>> GetAll()
    {
        var genres = await _genresRepository.GetAllAsync();
        return Normalize(genres);
    }

    public async Task<GenreDto> Get(Guid genreId)
    {
        var genre = await _genresRepository.GetAsync(genreId)
                    ?? throw new NotFoundException(ExceptionMessages.GenreNotFound);
        return Normalize(genre);
    }

    public async Task<GenreDto> Create(GenreCreateRequest request)
    {
        if (await _genresRepository.GetAsync(x => x.Value.ToLower() == request.Value.ToLower()) != null)
            throw new ConflictException("Genre exists");
        
        var genre = new Genre()
        {
            Value = request.Value,
            Color = request.Color.ToUpper()
        };

        await _genresRepository.CreateAsync(genre);

        await _publishEndpoint.PublishGenreCreated(genre);

        return Normalize(genre);
    }

    public async Task<GenreDto> Update(Guid genreId, GenreUpdateRequest request)
    {
        var existingGenre = await _genresRepository.GetAsync(genreId) 
                            ?? throw new NotFoundException(ExceptionMessages.GenreNotFound);

        if (await _genresRepository.GetAsync(x => x.Value.ToLower() == request.Value.ToLower() && x.Id != genreId) != null)
            throw new ConflictException("Genre exists");
        
        existingGenre.Value = request.Value;
        existingGenre.Color = request.Color.ToUpper();

        await _genresRepository.UpdateAsync(existingGenre);
        
        await _publishEndpoint.PublishGenreUpdated(existingGenre);

        return Normalize(existingGenre);
    }

    public async Task Delete(Guid genreId)
    {
        var genre = await _genresRepository.GetAsync(genreId);
        if(genre == null) return;
        
        await _genresRepository.RemoveAsync(genreId);

        await _publishEndpoint.PublishGenreDeleted(genre);
    }
    
    private static GenreDto Normalize(IGenreBase model) => model.AsDto();

    private static List<GenreDto> Normalize(IEnumerable<IGenreBase> models) => models.AsDto().ToList();
}