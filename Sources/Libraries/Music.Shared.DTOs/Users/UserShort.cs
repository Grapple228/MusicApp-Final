using Music.Shared.Common;

namespace Music.Shared.DTOs.Users;

public record UserShort(Guid Id, string Username) : IModel;