using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Core;
using Music.Applications.Windows.Models;
using Music.Applications.Windows.Services;
using Music.Applications.Windows.Services.Navigation;
using Music.Applications.Windows.ViewModels;
using Music.Applications.Windows.ViewModels.Authentication;
using Music.Applications.Windows.ViewModels.Default;
using Music.Applications.Windows.ViewModels.Dialog;
using Music.Applications.Windows.ViewModels.Media.Album;
using Music.Applications.Windows.ViewModels.Media.Artist;
using Music.Applications.Windows.ViewModels.Media.Playlist;
using Music.Applications.Windows.ViewModels.Media.Track;
using Music.Applications.Windows.ViewModels.Media.User;
using Music.Applications.Windows.ViewModels.Navigation;
using Music.Applications.Windows.ViewModels.Popup;
using Music.Applications.Windows.Views;
using MusicClient.Client;

namespace Music.Applications.Windows;

public partial class App
{
    public App()
    {
        var services = new ServiceCollection();

        services.AddSingleton(provider => new MainWindow
        {
            DataContext = provider.GetRequiredService<MainViewModel>()
        });

        services.AddSingleton<MainViewModel>();
        services.AddSingleton<SettingsViewModel>();
        services.AddTransient<DefaultViewModel>();
        services.AddTransient<IdentityUserViewModel>();
        services.AddTransient<DialogViewModel>();
        services.AddTransient<PopupViewModel>();
        services.AddTransient<AuthenticationViewModel>();
        services.AddTransient<RegistrationViewModel>();
        services.AddTransient<BrowseViewModel>();
        services.AddTransient<HomeViewModel>();
        services.AddTransient<LikedViewModel>();
        services.AddTransient<AlbumsViewModel>();
        services.AddTransient<ArtistsViewModel>();
        services.AddTransient<TracksViewModel>();
        services.AddTransient<CreatePlaylistViewModel>();
        services.AddTransient<CreateTrackViewModel>();
        services.AddTransient<AdminPanelViewModel>();
        services.AddTransient<ArtistStudioViewModel>();
        services.AddTransient<AddToPlaylistViewModel>();

        services.AddTransient<MediaAlbumViewModel>();
        services.AddTransient<MediaTrackViewModel>();
        services.AddTransient<MediaArtistViewModel>();

        services.AddSingleton<RoomViewModel>();
        
        services.AddSingleton<CurrentUser>();

        services.AddSingleton<GlobalNavigationService>();
        services.AddSingleton<NotificationViewModel>();
        services.AddScoped<ApplicationNavigationService>();
        services.AddSingleton<ApplicationService>();
        services.AddSingleton<ControlPanelViewModel>();

        services.AddSingleton<Func<Type, ViewModelBase>>(provider =>
            viewModelType => (ViewModelBase)provider.GetRequiredService(viewModelType));

        services.AddSingleton(SettingsManager.GetSettings());
        services.AddSingleton<IApiClient, ApiClient>(provider =>
            new ApiClient(provider.GetRequiredService<SettingsViewModel>().GatewayPath, "", ""));

        ServiceProvider = services.BuildServiceProvider();
    }

    public static ServiceProvider ServiceProvider { get; private set; } = null!;

    public static ApplicationService GetService()
    {
        return ServiceProvider.GetRequiredService<ApplicationService>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        var settings = ServiceProvider.GetRequiredService<SettingsViewModel>().SettingsModel;
        Task.Run(async () => await GetService().Authenticate(settings.RefreshToken));
        var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}