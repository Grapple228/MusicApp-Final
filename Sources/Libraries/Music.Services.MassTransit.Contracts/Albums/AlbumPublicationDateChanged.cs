namespace Music.Services.MassTransit.Contracts.Albums;

public record AlbumPublicationDateChanged(
    Guid Id, DateOnly PublicationDate
);