namespace Music.Applications.Windows.Events;

public enum NotificationType
{
    Error,
    Success,
    Notification
}

public delegate void NotificationDisplayedHandler(string message, string title, NotificationType notificationType);

public delegate void NotificationCloseRequestedHandler();

public static class NotificationEvents
{
    public static event NotificationDisplayedHandler? NotificationDisplayed;
    public static event NotificationCloseRequestedHandler? NotificationCloseRequested;

    public static void RequestClose()
    {
        NotificationCloseRequested?.Invoke();
    }
    
    public static void DisplayNotification(string message, string title, NotificationType notificationType = NotificationType.Success)
    {
        NotificationDisplayed?.Invoke(message, title, notificationType);
    }
}