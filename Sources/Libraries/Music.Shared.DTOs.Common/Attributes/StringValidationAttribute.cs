using System.ComponentModel.DataAnnotations;

namespace Music.Shared.DTOs.Common.Attributes;

public class StringValidationAttribute : ValidationAttribute
{
   
    public override bool IsValid(object? value)
    {
        var str = value?.ToString();
        
        if (string.IsNullOrWhiteSpace(str))
            return false;
        
        return str.Length is >= Constants.MinStringLength and <= Constants.MaxStringLength;
    }
}