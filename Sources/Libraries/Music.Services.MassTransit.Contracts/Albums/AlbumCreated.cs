namespace Music.Services.MassTransit.Contracts.Albums;

public record AlbumCreated(
    Guid Id,
    string Title,
    Guid OwnerId,
    DateOnly PublicationDate
);