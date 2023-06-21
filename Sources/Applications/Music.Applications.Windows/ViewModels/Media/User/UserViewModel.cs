using Music.Applications.Windows.Core;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Interfaces;

namespace Music.Applications.Windows.ViewModels.Media.User;

public class UserViewModel : LoadableViewModel
{
    private Models.Media.Users.User _user;
    public override string ModelName { get; protected set; } = nameof(UserViewModel);

    public Models.Media.Users.User User
    {
        get => _user;
        private set
        {
            if (Equals(value, _user)) return;
            _user = value;
            OnPropertyChanged();
        }
    }

    public Guid Id { get; set; }
    
    public UserViewModel(Guid userId)
    {
        Id = userId;
        Task.Run(async () => await LoadInfo(userId));
    }
    
    protected override async Task LoadMedia(Guid mediaId)
    {
        var user = await App.GetService().GetUser(mediaId);
        User = new Models.Media.Users.User(user);
        UserEvents.UsernameChanged += UserEventsOnUsernameChanged;
    }

    private void UserEventsOnUsernameChanged(Guid userId, string username)
    {
        if(User.Id != userId) return;
        User.Username = username.Trim();
    }

    public override async Task Reload()
    {
        await LoadInfo(Id);
    }

    public override void Dispose()
    {
        UserEvents.UsernameChanged -= UserEventsOnUsernameChanged;
    }

    ~UserViewModel()
    {
        Dispose();   
    }
    
    public override bool Equals(object? obj)
    {
        return obj is UserViewModel model && model.Id == Id;
    }

    protected bool Equals(UserViewModel other)
    {
        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), _user, ModelName, Id);
    }
}