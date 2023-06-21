using System.ComponentModel.DataAnnotations;
using Music.Shared.DTOs.Common;
using Music.Shared.DTOs.Common.Attributes;

namespace Music.Shared.DTOs.Requests.Albums;

public record AlbumCreateRequest(
    [StringValidation(ErrorMessage = DtoErrorMessages.InvalidTitle)] string Title,
    [Required] DateOnly PublicationDate
) : ICreateRequest;