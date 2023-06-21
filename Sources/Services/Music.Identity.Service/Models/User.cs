using Music.Services.Models;

namespace Music.Identity.Service.Models;

public class User : UserBase
{
    public Guid EmailId { get; set; }
    public DateTimeOffset RegistrationDate { get; init; } = DateTimeOffset.UtcNow;
    public ICollection<Guid> Roles { get; set; } = new List<Guid>();
}