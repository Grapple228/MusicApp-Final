namespace Music.Services.MassTransit.Contracts.Tracks;

public record TrackMediaChanged(
    Guid Id,
    Guid UserId,
    bool IsLiked,
    bool IsBlocked
) : IMediaChanged;