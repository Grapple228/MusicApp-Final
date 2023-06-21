namespace Music.Services.MassTransit.Contracts.Playlists;

public record PlaylistCreated(
    Guid Id,
    string Title,
    Guid OwnerId,
    bool IsPublic
);