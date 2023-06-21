using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Core;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Models;

namespace Music.Applications.Windows.ViewModels.Media.User;

public class IdentityUserViewModel : LoadableViewModel
{
    public Guid Id { get; set; }
    
    private IdentityUser _user;
    public override string ModelName { get; protected set; } = nameof(IdentityUserViewModel);

    public IdentityUser User
    {
        get => _user;
        set
        {
            _user = value;
            OnPropertyChanged();
        }
    }

    public IdentityUserViewModel()
    {
        Id = App.ServiceProvider.GetRequiredService<CurrentUser>().Id;
        Task.Run(async () => await LoadInfo(Id));
        UserEvents.UsernameChanged += UserEventsOnUsernameChanged;
    }
    
    public IdentityUserViewModel(Guid userId)
    {
        Id = userId;
        Task.Run(async () => await LoadInfo(Id));
        UserEvents.UsernameChanged += UserEventsOnUsernameChanged;
    }

    private void UserEventsOnUsernameChanged(Guid userId, string username)
    {
        if(User.Id != userId) return;
        User.Username = username.Trim();
    }

    protected override async Task LoadMedia(Guid mediaId)
    {
        var user = await App.GetService().GetProfile();
        User = new IdentityUser(user);
    }

    public override async Task Reload()
    {
        await LoadInfo(Id);
    }

    public override void Dispose()
    {
        UserEvents.UsernameChanged -= UserEventsOnUsernameChanged;
        GC.SuppressFinalize(this);
    }

    ~IdentityUserViewModel()
    {
        Dispose();
    }
}