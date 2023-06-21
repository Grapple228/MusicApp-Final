namespace Music.Applications.Windows.Events;

public delegate void UpdateRequestedHandler();

public delegate void SignedOutHandler(string message = "");

public static class GlobalEvents
{
    public static event UpdateRequestedHandler? UpdateRequested;
    public static event SignedOutHandler? SignedOut;

    public static void SignOut(string message = "")
    {
        SignedOut?.Invoke(message);
    }

    public static void Update()
    {
        UpdateRequested?.Invoke();
    }
}