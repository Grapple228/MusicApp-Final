using Music.Shared.DTOs.Common;
using Music.Shared.DTOs.Requests.Media.Attributes;

namespace Music.Shared.DTOs.Requests.Media;

[MediaCreateValidation(ErrorMessage = "Can't set both IsLiked and isBlocked to true at the same time")]
public record MediaCreateRequest(
        bool IsLiked,
        bool IsBlocked
    ) : ICreateRequest;
    