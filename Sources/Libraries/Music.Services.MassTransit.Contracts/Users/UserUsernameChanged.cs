namespace Music.Services.MassTransit.Contracts.Users;

public record UserUsernameChanged(Guid Id, string Username);