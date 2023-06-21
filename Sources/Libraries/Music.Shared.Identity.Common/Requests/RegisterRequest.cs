using System.ComponentModel.DataAnnotations;
using Music.Shared.DTOs.Common;
using Music.Shared.DTOs.Common.Attributes;
using Music.Shared.Identity.Common.Attributes;

namespace Music.Shared.Identity.Common.Requests;

public record RegisterRequest(
    [StringWithoutSpaces(ErrorMessage = DtoErrorMessages.InvalidName)] string Username, 
    [EmailAddress(ErrorMessage = DtoErrorMessages.InvalidEmail)] string Email, 
    [StringValidation(ErrorMessage = DtoErrorMessages.InvalidString)] string Password) : ICreateRequest;
