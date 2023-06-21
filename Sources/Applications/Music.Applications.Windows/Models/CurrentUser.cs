using System.Collections.ObjectModel;
using Music.Applications.Windows.Core;
using Music.Applications.Windows.Events;
using Music.Shared.Identity.Common;
using Music.Shared.Identity.Jwt;

namespace Music.Applications.Windows.Models;

public class CurrentUser : ObservableObject, IDisposable
{
    private bool _changed;
    private bool _isAdmin;
    private bool _isArtist;
    private string _largeImagePath = null!;
    private string _mediumImagePath = null!;
    private string _smallImagePath = null!;
    private string? _username;

    public Guid Id { get; private set; }

    public bool IsArtist
    {
        get => _isArtist;
        set
        {
            _isArtist = value;
            OnPropertyChanged();
        }
    }

    public bool IsAdmin
    {
        get => _isAdmin;
        set
        {
            _isAdmin = value;
            OnPropertyChanged();
        }
    }

    public string? Username
    {
        get => _username;
        private set
        {
            _username = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Roles> Roles { get; } = new();

    public string SmallImagePath
    {
        get => _smallImagePath;
        set
        {
            _smallImagePath = value;
            OnPropertyChanged();
        }
    }

    public string MediumImagePath
    {
        get => _mediumImagePath;
        set
        {
            _mediumImagePath = value;
            OnPropertyChanged();
        }
    }

    public string LargeImagePath
    {
        get => _largeImagePath;
        set
        {
            _largeImagePath = value;
            OnPropertyChanged();
        }
    }

    public void Dispose()
    {
        ImageEvents.UserImageChanged -= ImageEventsOnUserImageChanged;
        UserEvents.UsernameChanged -= UserEventsOnUsernameChanged;
        GC.SuppressFinalize(this);
    }

    private void UserEventsOnUsernameChanged(Guid userId, string username)
    {
       if(userId != Id) return;
       Username = username.Trim();
    }

    public void Change(LoginDto loginDto)
    {
        Clear();
        Id = loginDto.Id;
        Username = loginDto.Username;
        SmallImagePath = loginDto.SmallImagePath;
        MediumImagePath = loginDto.MediumImagePath;
        LargeImagePath = loginDto.LargeImagePath;

        foreach (var role in loginDto.Roles) Roles.Add(role);

        IsAdmin = Roles.Contains(Shared.Identity.Common.Roles.Admin);
        IsArtist = Roles.Contains(Shared.Identity.Common.Roles.Artist);

        ImageEvents.UserImageChanged += ImageEventsOnUserImageChanged;
        UserEvents.UsernameChanged += UserEventsOnUsernameChanged;
    }

    private void ImageEventsOnUserImageChanged(Guid id)
    {
        if (Id != id) return;
        var small = SmallImagePath;
        var medium = MediumImagePath;
        var large = LargeImagePath;

        if (_changed)
        {
            SmallImagePath = small.Remove(SmallImagePath.Length - 1, 1);
            MediumImagePath = medium.Remove(MediumImagePath.Length - 1, 1);
            LargeImagePath = large.Remove(LargeImagePath.Length - 1, 1);
        }
        else
        {
            SmallImagePath = small + "/";
            MediumImagePath = medium + "/";
            LargeImagePath = large + "/";
        }

        _changed = !_changed;
        GC.Collect();
    }

    public override bool Equals(object? obj)
    {
        return obj is CurrentUser user && user.Id == Id;
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Roles);
        return hashCode.ToHashCode();
    }

    protected bool Equals(CurrentUser other)
    {
        return other.Id == Id;
    }

    public void Clear()
    {
        Id = Guid.Empty;
        _changed = false;
        Username = null;
        Roles.Clear();
        IsAdmin = false;
        IsArtist = false;
    }

    ~CurrentUser()
    {
        Dispose();
    }
}