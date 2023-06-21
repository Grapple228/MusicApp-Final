using Music.Shared.Common;

namespace Music.Shared.DTOs.Genres;

public record GenreDto(
    Guid Id, 
    string Value, 
    string Color) : IModel;