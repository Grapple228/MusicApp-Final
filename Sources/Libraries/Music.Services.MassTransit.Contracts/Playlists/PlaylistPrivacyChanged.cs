namespace Music.Services.MassTransit.Contracts.Playlists;

public record PlaylistPrivacyChanged(
    Guid Id,
    bool IsPublic
);