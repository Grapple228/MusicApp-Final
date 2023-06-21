namespace Music.Services.MassTransit.Contracts.Users;

public record UserDeletedPlaylist(Guid Id, Guid PlaylistId);