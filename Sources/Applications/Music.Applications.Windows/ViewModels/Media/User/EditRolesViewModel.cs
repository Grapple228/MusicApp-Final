using System.Collections.ObjectModel;
using Music.Applications.Windows.Core;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Models;
using Music.Applications.Windows.Services;
using Music.Shared.Identity.Common;

namespace Music.Applications.Windows.ViewModels.Media.User;

public class EditRolesViewModel : LoadableViewModel
{
    private IdentityUser _user = null!;

    public EditRolesViewModel(Guid userId)
    {
        Id = userId;
        Task.Run(() => LoadInfo(Id));
        UserEvents.UserRoleAddRequested += UserEventsOnUserRoleAddRequested;
        UserEvents.UserRoleRemoveRequested += UserEventsOnUserRoleRemoveRequested;
        UserEvents.UserRoleRemoved += UserEventsOnUserRoleRemoved;
        UserEvents.UserRoleAdded += UserEventsOnUserRoleAdded;
    }

    public ObservableCollection<Role> NotSelectedRoles { get; } = new();

    public Guid Id { get; }

    public IdentityUser User
    {
        get => _user;
        set
        {
            _user = value;
            OnPropertyChanged();
        }
    }

    public override string ModelName { get; protected set; } = nameof(EditRolesViewModel);

    private void UserEventsOnUserRoleAdded(Guid userid, Roles role)
    {
        var roleToRemove = NotSelectedRoles.FirstOrDefault(x => x.Roles == role);
        if (roleToRemove != null) ApplicationService.Invoke(() => NotSelectedRoles.Remove(roleToRemove));

        var genreToAdd = User.Roles.FirstOrDefault(x => x.Roles == role);
        if (genreToAdd == null) ApplicationService.Invoke(() => User.Roles.Add(new Role(role)));
    }

    private void UserEventsOnUserRoleRemoved(Guid userid, Roles role)
    {
        if (Id != userid) return;
        var roleToRemove = User.Roles.FirstOrDefault(x => x.Roles == role);
        if (roleToRemove != null) ApplicationService.Invoke(() => User.Roles.Remove(roleToRemove));

        var roleToAdd = NotSelectedRoles.FirstOrDefault(x => x.Roles == role);
        if (roleToAdd == null) ApplicationService.Invoke(() => NotSelectedRoles.Add(new Role(role)));
    }

    private void UserEventsOnUserRoleRemoveRequested(Roles role)
    {
        UserEvents.RemoveRole(Id, role);
    }

    private void UserEventsOnUserRoleAddRequested(Roles role)
    {
        UserEvents.AddRole(Id, role);
    }

    protected override async Task LoadMedia(Guid mediaId)
    {
        try
        {
            var userDto = await App.GetService().GetIdentityUser(mediaId);
            ApplicationService.Invoke(() => User = new IdentityUser(userDto));
            var allRoles = new List<Roles>
            {
                Roles.Admin,
                Roles.Artist
            };
            ApplicationService.Invoke(() => NotSelectedRoles.Clear());
            var notSelected = allRoles.Where(x => User.Roles.All(r => r.Roles != x));
            foreach (var role in notSelected) ApplicationService.Invoke(() => NotSelectedRoles.Add(new Role(role)));
        }
        catch
        {
        }
    }

    public override async Task Reload()
    {
        await LoadInfo(Id);
    }

    public override void Dispose()
    {
        UserEvents.UserRoleAddRequested -= UserEventsOnUserRoleAddRequested;
        UserEvents.UserRoleRemoveRequested -= UserEventsOnUserRoleRemoveRequested;
        UserEvents.UserRoleRemoved -= UserEventsOnUserRoleRemoved;
        UserEvents.UserRoleAdded -= UserEventsOnUserRoleAdded;
        GC.SuppressFinalize(this);
    }

    ~EditRolesViewModel()
    {
        Dispose();
    }
}