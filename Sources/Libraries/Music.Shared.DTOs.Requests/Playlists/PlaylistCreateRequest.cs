using System.ComponentModel.DataAnnotations;
using Music.Shared.DTOs.Common;
using Music.Shared.DTOs.Common.Attributes;

namespace Music.Shared.DTOs.Requests.Playlists;

public record PlaylistCreateRequest(
    [StringValidation(ErrorMessage = DtoErrorMessages.InvalidTitle)] string Title,
    bool IsPublic
) : ICreateRequest;