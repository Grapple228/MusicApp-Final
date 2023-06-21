namespace Music.Services.MassTransit.Contracts.Tracks;

public record TrackPublicationDateChanged(Guid Id, DateOnly PublicationDate);