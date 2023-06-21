using System.ComponentModel.DataAnnotations;
using Music.Shared.DTOs.Common;
using Music.Shared.DTOs.Common.Attributes;

namespace Music.Tracks.Service.Requests;

public class CreateTrackRequest
{
    [StringValidation(ErrorMessage = DtoErrorMessages.InvalidTitle)]
    [Required] public string Title { get; set; } = null!;
    public ICollection<Guid>? Genres { get; set; }
    [Required] public DateOnly PublicationDate { get; set; }
    [Required] public IFormFile File { get; set; } = null!;
}