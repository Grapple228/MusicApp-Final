using Music.Shared.Common;

namespace Music.Shared.DTOs.Albums;

public record AlbumShort(Guid Id, string Title, DateOnly PublicationDate, Guid OwnerId) : IModel;