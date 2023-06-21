namespace Music.Services.MassTransit.Contracts.Artists;

public record ArtistCreated(
    Guid Id,
    string Name
);