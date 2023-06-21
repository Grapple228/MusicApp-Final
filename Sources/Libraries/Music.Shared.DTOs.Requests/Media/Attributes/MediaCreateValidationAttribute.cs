using System.ComponentModel.DataAnnotations;

namespace Music.Shared.DTOs.Requests.Media.Attributes;

public class MediaCreateValidationAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is not MediaCreateRequest request) return false;
        return !request.IsBlocked || !request.IsLiked;
    }
}