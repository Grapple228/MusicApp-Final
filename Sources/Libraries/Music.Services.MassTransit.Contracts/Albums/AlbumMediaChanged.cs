namespace Music.Services.MassTransit.Contracts.Albums;

public record AlbumMediaChanged(
    Guid Id,
    Guid UserId,
    bool IsLiked,
    bool IsBlocked
) : IMediaChanged;