namespace Music.Services.MassTransit.Contracts.Albums;

public record AlbumTrackRemoved(Guid Id, Guid TrackId);