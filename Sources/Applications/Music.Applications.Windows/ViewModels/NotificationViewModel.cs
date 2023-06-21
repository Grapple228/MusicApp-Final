using System.Windows;
using Music.Applications.Windows.Core;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Services;

namespace Music.Applications.Windows.ViewModels;

public class NotificationViewModel : ViewModelBase
{
    private string _message;
    private string _title;
    private NotificationType _notificationType;
    public override string ModelName { get; protected set; } = nameof(NotificationViewModel);

    public NotificationViewModel()
    {
        NotificationEvents.NotificationDisplayed += NotificationEventsOnNotificationDisplayed;
        NotificationEvents.NotificationCloseRequested += NotificationEventsOnNotificationCloseRequested;
    }

    private void NotificationEventsOnNotificationCloseRequested()
    {
        Close();
    }

    public string Message
    {
        get => _message;
        set
        {
            if (value == _message) return;
            _message = value;
            OnPropertyChanged();
        }
    }

    public string Title
    {
        get => _title;
        set
        {
            if (value == _title) return;
            _title = value;
            OnPropertyChanged();
        }
    }

    public NotificationType NotificationType
    {
        get => _notificationType;
        set
        {
            if (value == _notificationType) return;
            _notificationType = value;
            OnPropertyChanged();
        }
    }

    private Timer? _timer;
    
    private bool _isShown;
    
    public bool IsShown
    {
        get => _isShown;
        protected set
        {
            _isShown = value;
            OnPropertyChanged();
        }
    }

    public double Opacity
    {
        get => _opacity;
        set
        {
            if (value.Equals(_opacity)) return;
            _opacity = value;
            OnPropertyChanged();
        }
    }

    private DateTime _shownTime;
    private double _opacity;

    private const double DefaultOpacity = 0.7;
    private const double UnhoverOpacity = 0.4;
    
    private void Show()
    {
        Opacity = DefaultOpacity;
        _shownTime = DateTime.Now;
        _timer ??= new Timer(CheckDisplayTime, 0, TimeSpan.Zero, TimeSpan.FromMilliseconds(20));
        IsShown = true;
    }


    private void CheckDisplayTime(object? state)
    {
        var seconds = (DateTime.Now - _shownTime).TotalSeconds;

        if (seconds > 2.5)
        {
            Opacity = UnhoverOpacity;
        }
        
        if(seconds < 5) return;
        
        Close();
    }

    private void Close()
    {
        _timer?.Dispose();
        _timer = null;
        IsShown = false;
    }
    
    private void NotificationEventsOnNotificationDisplayed(string message, string title, NotificationType notificationType)
    {
        Close();
        Message = message;
        Title = title;
        NotificationType = notificationType;
        Show();
    }

    public override void Dispose()
    {
        NotificationEvents.NotificationDisplayed -= NotificationEventsOnNotificationDisplayed;
        NotificationEvents.NotificationCloseRequested -= NotificationEventsOnNotificationCloseRequested;
        GC.SuppressFinalize(this);
    }

    ~NotificationViewModel()
    {
        Dispose();
    }
}