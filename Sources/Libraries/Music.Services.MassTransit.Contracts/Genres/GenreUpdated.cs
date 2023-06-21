namespace Music.Services.MassTransit.Contracts.Genres;

public record GenreUpdated(
    Guid Id,
    string Value,
    string Color
);