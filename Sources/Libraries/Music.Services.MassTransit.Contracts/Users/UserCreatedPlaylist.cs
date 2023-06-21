namespace Music.Services.MassTransit.Contracts.Users;

public record UserCreatedPlaylist(Guid Id, Guid PlaylistId);