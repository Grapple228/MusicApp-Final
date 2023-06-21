namespace Music.Services.MassTransit.Contracts.Users;

public record UserCreated(
    Guid Id,
    string Username
);