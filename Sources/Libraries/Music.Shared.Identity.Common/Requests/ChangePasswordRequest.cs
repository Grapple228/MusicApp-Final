using Music.Shared.DTOs.Common.Attributes;

namespace Music.Shared.Identity.Common.Requests;

public record ChangePasswordRequest(
    [StringValidation] string CurrentPassword,
    [StringValidation] string NewPassword
    );