using Music.Shared.Common;
using Music.Shared.DTOs.Common;

namespace Music.Shared.Identity.Common.Models;

public record IdentityUserDto(
    Guid Id, 
    string Username, 
    string Email, 
    bool IsEmailConfirmed,
    IEnumerable<Roles> Roles,
    DateTimeOffset RegistrationDate,
    string ImagePath) : 
    ImageRecordBase(ImagePath), IModel;
