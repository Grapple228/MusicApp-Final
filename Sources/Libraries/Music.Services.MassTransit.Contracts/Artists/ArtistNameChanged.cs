namespace Music.Services.MassTransit.Contracts.Artists;

public record ArtistNameChanged(
    Guid Id,
    string Name
);