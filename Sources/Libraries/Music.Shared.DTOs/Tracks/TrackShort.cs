using Music.Shared.Common;

namespace Music.Shared.DTOs.Tracks;

public record TrackShort(Guid Id, string Title, DateOnly PublicationDate, Guid OwnerId) : IModel;