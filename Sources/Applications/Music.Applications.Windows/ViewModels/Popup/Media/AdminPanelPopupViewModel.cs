using System.Windows;
using System.Windows.Media;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Models.Popup;
using Music.Applications.Windows.Services;
using Music.Applications.Windows.ViewModels.Dialog;
using Music.Applications.Windows.ViewModels.Media.Genre;

namespace Music.Applications.Windows.ViewModels.Popup.Media;

public class AdminPanelPopupViewModel : PopupNavigationViewModelBase
{
    public AdminPanelPopupViewModel()
    {
        Navigations.Add(new PopupNavigationModel(
            "Create genre",
            (PathGeometry)Application.Current.Resources["AddIcon"],
            () => { DialogViewModel.OpenGlobal("Create genre", new EditGenreViewModel(true)); }));

        Navigations.Add(new PopupNavigationModel(
            "Change Image",
            (PathGeometry)Application.Current.Resources["ImageIcon"],
            () =>
            {
                Task.Run(() => App.GetService().ChangeImage(ApplicationService.CurrentUserId(), ChangeImage.Users));
            }));
    }

    public override string ModelName { get; protected set; } = nameof(AdminPanelPopupViewModel);

    public override bool Equals(object? obj)
    {
        return obj is AdminPanelPopupViewModel model && model.ModelName == ModelName;
    }

    protected bool Equals(AdminPanelPopupViewModel other)
    {
        return base.Equals(other) && ModelName == other.ModelName;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), ModelName);
    }
}