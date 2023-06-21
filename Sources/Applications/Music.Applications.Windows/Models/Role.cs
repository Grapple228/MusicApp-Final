using Music.Shared.Identity.Common;

namespace Music.Applications.Windows.Models;

public class Role
{
    public Role(Roles role)
    {
        Roles = role;
        RoleString = role.ToString();
    }

    public Roles Roles { get; set; }
    public string RoleString { get; set; }
}