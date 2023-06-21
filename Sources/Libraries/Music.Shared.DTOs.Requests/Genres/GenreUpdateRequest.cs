using Music.Shared.DTOs.Common;
using Music.Shared.DTOs.Common.Attributes;

namespace Music.Shared.DTOs.Requests.Genres;

public record GenreUpdateRequest(
    [StringValidation(ErrorMessage = DtoErrorMessages.InvalidValue)] string Value,
    [ColorValidation(ErrorMessage = DtoErrorMessages.InvalidColor)] string Color
) : IUpdateRequest;