using Music.Applications.Windows.Events;
using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Services;
using Music.Shared.DTOs.Users;

namespace Music.Applications.Windows.Models.Media.Users;

public class User : ImageModel, IUser
{
    public User(UserDto dto) : base(dto, dto.Id, ChangeImage.Users)
    {
        Id = dto.Id;
        Username = dto.Username;
        IsUserOwner = ApplicationService.IsUserOwner(Id);
        IsSecondaryOwner = IsUserOwner;
        IsRemovable = false;
    }
    
    public User(UserShortDto dto) : base(dto, dto.Id, ChangeImage.Users)
    {
        Id = dto.Id;
        Username = dto.Username;
        IsUserOwner = ApplicationService.IsUserOwner(Id);
        IsSecondaryOwner = IsUserOwner;
        IsRemovable = false;
    }

    public Guid Id { get; init; }
    public bool IsUserOwner { get; }
    public bool IsSecondaryOwner { get; }
    public bool IsRemovable { get; }
    public string Username { get; set; }
}