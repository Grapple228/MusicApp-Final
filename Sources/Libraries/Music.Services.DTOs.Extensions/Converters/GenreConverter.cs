using Music.Services.Models;
using Music.Shared.DTOs.Genres;

namespace Music.Services.DTOs.Extensions.Converters;

public static class GenreConverter
{
    public static GenreDto AsDto(this IGenreBase genre) =>
        new(genre.Id, genre.Value, genre.Color);
    
    public static IEnumerable<GenreDto> AsDto(this IEnumerable<IGenreBase> genres) =>
        genres.Select(AsDto);
}