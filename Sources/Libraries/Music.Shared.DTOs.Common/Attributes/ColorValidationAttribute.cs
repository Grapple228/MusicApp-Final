using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Music.Shared.DTOs.Common.Attributes;

public class ColorValidationAttribute : ValidationAttribute
{
    private static readonly Regex ColorRegex = new("^#(?:[0-9a-fA-F]{3}){1,2}$");
    
    public override bool IsValid(object? value)
    {
        var str = value?.ToString();
        return !string.IsNullOrWhiteSpace(str) && ColorRegex.Match(str).Success;
    }
}