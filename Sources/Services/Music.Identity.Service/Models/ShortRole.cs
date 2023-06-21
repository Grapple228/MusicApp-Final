using Music.Shared.Common;
using Music.Shared.Identity.Common;

namespace Music.Identity.Service.Models;

public class ShortRole : IModel
{
    public ShortRole(Guid id, Roles role)
    {
        Id = id;
        Role = role;
    }
    
    public Roles Role { get; set; }
    public Guid Id { get; init; }
}