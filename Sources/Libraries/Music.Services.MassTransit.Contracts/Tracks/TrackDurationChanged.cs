namespace Music.Services.MassTransit.Contracts.Tracks;

public record TrackDurationChanged(Guid Id, int Duration);