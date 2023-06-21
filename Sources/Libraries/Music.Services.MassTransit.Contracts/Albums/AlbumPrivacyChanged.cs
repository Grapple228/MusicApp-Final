namespace Music.Services.MassTransit.Contracts.Albums;

public record AlbumPrivacyChanged(
    Guid Id,
    bool IsPublic
);