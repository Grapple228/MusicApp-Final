namespace Music.Services.MassTransit.Contracts.Playlists;

public record PlaylistMediaChanged(
    Guid Id,
    Guid UserId,
    bool IsLiked,
    bool IsBlocked
) : IMediaChanged;