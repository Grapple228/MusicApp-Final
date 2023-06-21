using System.Collections.ObjectModel;
using Music.Applications.Windows.Core;
using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Models;
using Music.Applications.Windows.Models.Media.Genres;
using Music.Applications.Windows.Services;
using Music.Shared.DTOs.Genres;
using Music.Shared.Identity.Common;

namespace Music.Applications.Windows.ViewModels.Navigation;

public class AdminPanelViewModel : LoadableViewModel, IDisplaying
{
    private bool _isFirstLoad = true;
    private CurrentUser _user = null!;

    public AdminPanelViewModel(CurrentUser user)
    {
        User = user;
        Id = User.Id;
        Task.Run(() => LoadInfo(Id));
        IsFirstLoad = false;
        GenreEvents.GenreCreated += GenreEventsOnGenreCreated;
        GenreEvents.GenreDeleted += GenreEventsOnGenreDeleted;
        GenreEvents.GenreUpdated += GenreEventsOnGenreUpdated;

        UserEvents.UserDeleted += UserEventsOnUserDeleted;
        UserEvents.UserRoleAdded += UserEventsOnUserRoleAdded;
        UserEvents.UserRoleRemoved += UserEventsOnUserRoleRemoved;
        UserEvents.UsernameChanged += UserEventsOnUsernameChanged;
    }

    private void UserEventsOnUsernameChanged(Guid userId, string username)
    {
        var user = Users.FirstOrDefault(x => x.Id == userId);
        if(user == null) return;
        user.Username = username.Trim();
    }

    public Guid Id { get; init; }

    public CurrentUser User
    {
        get => _user;
        private set
        {
            _user = value;
            OnPropertyChanged();
        }
    }

    public bool IsFirstLoad
    {
        get => _isFirstLoad;
        set
        {
            if (value == _isFirstLoad) return;
            _isFirstLoad = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Genre> Genres { get; } = new();
    public ObservableCollection<IdentityUser> Users { get; } = new();

    public override string ModelName { get; protected set; } = "Admin Panel";

    public CurrentDisplaying CurrentDisplaying
    {
        get => DisplayingMedia.AdminPanelViewModel;
        set
        {
            DisplayingMedia.AdminPanelViewModel = value;
            OnPropertyChanged();
            Task.Run(async () => await Reload());
            IsFirstLoad = false;
        }
    }

    private void UserEventsOnUserRoleRemoved(Guid userId, Roles role)
    {
        var user = Users.FirstOrDefault(x => x.Id == userId);
        var existing = user?.Roles.FirstOrDefault(x => x.Roles == role);
        if (existing == null) return;
        ApplicationService.Invoke(() => user?.Roles.Remove(existing));
    }

    private void UserEventsOnUserRoleAdded(Guid userId, Roles role)
    {
        var user = Users.FirstOrDefault(x => x.Id == userId);
        if (user == null) return;
        var existing = user.Roles.FirstOrDefault(x => x.Roles == role);
        if (existing != null) return;
        ApplicationService.Invoke(() => user.Roles.Add(new Role(role)));
    }

    private void UserEventsOnUserDeleted(Guid userId)
    {
        var user = Users.FirstOrDefault(x => x.Id == userId);
        if (user == null) return;
        ApplicationService.Invoke(() => Users.Remove(user));
    }

    private void GenreEventsOnGenreUpdated(GenreDto genreDto)
    {
        var genre = Genres.FirstOrDefault(x => x.Id == genreDto.Id);
        if (genre == null) return;
        ApplicationService.Invoke(() => genre.Update(genreDto));
    }

    private void GenreEventsOnGenreDeleted(Guid genreId)
    {
        var genre = Genres.FirstOrDefault(x => x.Id == genreId);
        if (genre == null) return;
        ApplicationService.Invoke(() => Genres.Remove(genre));
    }

    private void GenreEventsOnGenreCreated(GenreDto genreDto)
    {
        var genre = Genres.FirstOrDefault(x => x.Id == genreDto.Id);
        if (genre != null) return;
        ApplicationService.Invoke(() => Genres.Add(new Genre(genreDto)));
    }

    protected override async Task LoadMedia(Guid mediaId)
    {
        switch (CurrentDisplaying)
        {
            case CurrentDisplaying.Users:
                await LoadUsers();
                break;
            case CurrentDisplaying.Genres:
                await LoadGenres();
                break;
        }

        async Task LoadGenres()
        {
            try
            {
                IEnumerable<GenreDto> genres = await App.GetService().GetGenres();
                ApplicationService.Invoke(() => Genres.Clear());
                foreach (var genreDto in genres) ApplicationService.Invoke(() => Genres.Add(new Genre(genreDto)));
            }
            catch
            {
                // ignored
            }
        }

        async Task LoadUsers()
        {
            try
            {
                var users = (await App.GetService().GetIdentityUsers()).Where(x =>
                    x.Id != ApplicationService.CurrentUserId());

                ApplicationService.Invoke(() => Users.Clear());
                foreach (var userDto in users) ApplicationService.Invoke(() => Users.Add(new IdentityUser(userDto)));
            }
            catch
            {
                // ignored
            }
        }
    }

    public override async Task Reload()
    {
        await LoadInfo(Guid.Empty);
    }

    ~AdminPanelViewModel()
    {
        Dispose();
    }

    public override void Dispose()
    {
        GenreEvents.GenreCreated -= GenreEventsOnGenreCreated;
        GenreEvents.GenreDeleted -= GenreEventsOnGenreDeleted;
        GenreEvents.GenreUpdated -= GenreEventsOnGenreUpdated;

        UserEvents.UserDeleted -= UserEventsOnUserDeleted;
        UserEvents.UserRoleAdded -= UserEventsOnUserRoleAdded;
        UserEvents.UserRoleRemoved -= UserEventsOnUserRoleRemoved;
        UserEvents.UsernameChanged -= UserEventsOnUsernameChanged;
        GC.SuppressFinalize(this);
    }
}