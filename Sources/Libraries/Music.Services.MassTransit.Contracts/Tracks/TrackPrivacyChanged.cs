namespace Music.Services.MassTransit.Contracts.Tracks;

public record TrackPrivacyChanged(
    Guid Id,
    bool IsPublic
);