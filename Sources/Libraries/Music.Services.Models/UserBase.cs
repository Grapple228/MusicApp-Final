using Music.Shared.Common;

namespace Music.Services.Models;

public interface IUserBase : IModel
{
    string Username { get; set; }
}

public class UserBase : ModelBase, IUserBase
{
    public string Username { get; set; } = null!;
}