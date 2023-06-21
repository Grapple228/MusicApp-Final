using System.Windows;
using System.Windows.Media;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Models.Popup;
using Music.Applications.Windows.ViewModels.Dialog;
using Music.Applications.Windows.ViewModels.Media.User;

namespace Music.Applications.Windows.ViewModels.Popup.Media;

public class IdentityUserPopupViewModel : PopupNavigationViewModelBase
{
    public IdentityUserPopupViewModel(IUser user)
    {
        Navigations.Add(new PopupNavigationModel(
            "Change Image",
            (PathGeometry)Application.Current.Resources["ImageIcon"],
            () => { Task.Run(() => App.GetService().ChangeImage(user.Id, ChangeImage.Users)); }));

        Navigations.Add(new PopupNavigationModel(
            "Edit Roles",
            (PathGeometry)Application.Current.Resources["EditIcon"],
            () => { DialogViewModel.OpenGlobal("Edit roles", new EditRolesViewModel(user.Id)); }));

        Navigations.Add(new PopupNavigationModel(
            "Delete",
            (PathGeometry)Application.Current.Resources["TrashIcon"],
            () => { UserEvents.DeleteUser(user.Id); }, true));
    }

    public override string ModelName { get; protected set; } = nameof(IdentityUserPopupViewModel);
}