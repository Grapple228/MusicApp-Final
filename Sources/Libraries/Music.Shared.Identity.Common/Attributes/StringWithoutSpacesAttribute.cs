using System.ComponentModel.DataAnnotations;
using Music.Shared.DTOs.Common;

namespace Music.Shared.Identity.Common.Attributes;

public class StringWithoutSpacesAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        var str = value?.ToString();
        
        if (string.IsNullOrWhiteSpace(str))
            return false;

        if (str.Contains(' '))
            return false;
        
        return str.Length is >= Constants.MinStringLength and <= Constants.MaxStringLength;
    }
}