using System.Windows;
using System.Windows.Media;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Models.Popup;
using Music.Applications.Windows.ViewModels.Dialog;
using Music.Applications.Windows.ViewModels.Media.Playlist;
using MusicClient.Api.Playlists;

namespace Music.Applications.Windows.ViewModels.Popup.Media;

public class UserPopupViewModel : PopupNavigationViewModelBase
{
    public UserPopupViewModel(IUser user, NavigationPopupOptions options)
    {
        if (options.IsOwner)
            Navigations.Add(new PopupNavigationModel(
                "Change Username",
                (PathGeometry)Application.Current.Resources["EditIcon"],
                () =>
                {
                    DialogViewModel.OpenGlobal("Change username",
                        new ChangeUsernameViewModel(user.Id, user.Username));
                }));
        
        if (options.IsAdmin || options.IsOwner)
            Navigations.Add(new PopupNavigationModel(
                "Change Image",
                (PathGeometry)Application.Current.Resources["ImageIcon"],
                () => { Task.Run(() => App.GetService().ChangeImage(user.Id, ChangeImage.Users)); }));

        if(options.IsAdmin)
            Navigations.Add(new PopupNavigationModel(
                "Delete",
                (PathGeometry)Application.Current.Resources["TrashIcon"],
                () =>
                {
                    Task.Run(async () =>
                    {
                        await App.GetService().DeleteUser(user.Id);
                    });
                },
                true));
    }
    
    public override string ModelName { get; protected set; } = nameof(UserPopupViewModel);
}