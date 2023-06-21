namespace Music.Services.MassTransit.Contracts.Tracks;

public record TrackCreated(
    Guid Id,
    string Title,
    int Duration,
    Guid OwnerId,
    ICollection<Guid> Genres,
    DateOnly PublicationDate
);