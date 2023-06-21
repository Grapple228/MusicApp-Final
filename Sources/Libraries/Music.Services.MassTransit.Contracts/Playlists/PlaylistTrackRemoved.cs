namespace Music.Services.MassTransit.Contracts.Playlists;

public record PlaylistTrackRemoved(Guid Id, Guid TrackId);