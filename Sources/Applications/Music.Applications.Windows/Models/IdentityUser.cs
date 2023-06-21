using System.Collections.ObjectModel;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Services;
using Music.Shared.Identity.Common.Models;

namespace Music.Applications.Windows.Models;

public class IdentityUser : ImageModel, IUser
{
    private string _username;

    public IdentityUser(IdentityUserDto dto) : base(dto, dto.Id, ChangeImage.Users)
    {
        Id = dto.Id;
        Email = dto.Email;
        IsEmailConfirmed = dto.IsEmailConfirmed;
        Username = dto.Username;
        RegistrationDate = dto.RegistrationDate.LocalDateTime;
        IsUserOwner = ApplicationService.IsUserOwner(Id);
        foreach (var role in dto.Roles) Roles.Add(new Role(role));
    }

    public Guid Id { get; init; }
    public string Email { get; set; }
    public bool IsEmailConfirmed { get; set; }

    public string Username
    {
        get => _username;
        set
        {
            if (value == _username) return;
            _username = value;
            OnPropertyChanged();
        }
    }

    public DateTimeOffset RegistrationDate { get; set; }
    public ObservableCollection<Role> Roles { get; } = new();

    public override bool Equals(object? obj)
    {
        return obj is IdentityUser user && user.Id == Id;
    }

    protected bool Equals(IdentityUser other)
    {
        return Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Roles);
    }

    public bool IsUserOwner { get; set; }
    public bool IsSecondaryOwner { get; }
    public bool IsRemovable { get; }
}