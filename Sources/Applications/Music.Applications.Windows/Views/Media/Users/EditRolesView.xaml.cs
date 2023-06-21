using System.Windows;
using System.Windows.Input;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Models;
using Music.Shared.Identity.Common;

namespace Music.Applications.Windows.Views.Media.Users;

public partial class EditRolesView
{
    public EditRolesView()
    {
        InitializeComponent();
    }

    private void NotSelectedRoleText_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: Role role }) return;
        UserEvents.RequestRoleAdd(role.Roles);
    }

    private void SelectedRoleText_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: Role role }) return;
        if (role.Roles == Roles.User) return;
        UserEvents.RequestRoleRemove(role.Roles);
    }
}