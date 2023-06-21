namespace Music.Services.MassTransit.Contracts.Artists;

public record ArtistMediaChanged(
    Guid Id,
    Guid UserId,
    bool IsLiked,
    bool IsBlocked
) : IMediaChanged;