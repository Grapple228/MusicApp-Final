namespace Music.Services.MassTransit.Contracts.Genres;

public record GenreCreated(
    Guid Id,
    string Value,
    string Color
);