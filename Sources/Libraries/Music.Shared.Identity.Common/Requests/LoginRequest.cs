using Music.Shared.DTOs.Common;
using Music.Shared.DTOs.Common.Attributes;
using Music.Shared.Identity.Common.Attributes;
using Music.Shared.Identity.Common.Models;

namespace Music.Shared.Identity.Common.Requests;

public record LoginRequest(
    [StringWithoutSpaces(ErrorMessage = DtoErrorMessages.InvalidName)] string Username, 
    [StringWithoutSpaces(ErrorMessage = DtoErrorMessages.InvalidString)] string Password, 
    DeviceInfo DeviceInfo) : IUpdateRequest;