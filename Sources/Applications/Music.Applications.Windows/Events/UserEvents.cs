using Music.Applications.Windows.Services;
using Music.Shared.Identity.Common;

namespace Music.Applications.Windows.Events;

public delegate void UserDeletedHandler(Guid userId);

public delegate void UserRoleRemovedHandler(Guid userId, Roles role);

public delegate void UserRoleAddedHandler(Guid userId, Roles role);

public delegate void UserRoleRemoveRequestedHandler(Roles role);

public delegate void UserRoleAddRequestedHandler(Roles role);

public delegate void UsernameChangedHandler(Guid userId, string username);

public static class UserEvents
{
    public static event UserDeletedHandler? UserDeleted;
    public static event UserRoleRemovedHandler? UserRoleRemoved;
    public static event UserRoleAddedHandler? UserRoleAdded;
    public static event UserRoleRemoveRequestedHandler? UserRoleRemoveRequested;
    public static event UserRoleAddRequestedHandler? UserRoleAddRequested;
    public static event UsernameChangedHandler? UsernameChanged;

    public static void ChangeUsername(Guid userId, string username)
    {
        UsernameChanged?.Invoke(userId, username);
    }
    
    public static void RequestRoleAdd(Roles role)
    {
        UserRoleAddRequested?.Invoke(role);
    }

    public static void RequestRoleRemove(Roles role)
    {
        UserRoleRemoveRequested?.Invoke(role);
    }

    public static void AddRole(Guid userId, Roles role)
    {
        try
        {
            Task.Run(() => App.GetService().AddRole(userId, role));
        }
        catch
        {
            return;
        }

        UserRoleAdded?.Invoke(userId, role);
    }

    public static void RemoveRole(Guid userId, Roles role)
    {
        try
        {
            Task.Run(() => App.GetService().RemoveRole(userId, role));
        }
        catch
        {
            return;
        }

        UserRoleRemoved?.Invoke(userId, role);
    }

    public static void DeleteUser(Guid userId)
    {
        try
        {
            Task.Run(() => App.GetService().DeleteUser(userId));
        }
        catch
        {
            return;
        }

        UserDeleted?.Invoke(userId);
    }

    public static void DeleteUser()
    {
        try
        {
            Task.Run(() => App.GetService().DeleteUser());
        }
        catch
        {
            return;
        }

        UserDeleted?.Invoke(ApplicationService.CurrentUserId());
    }
}