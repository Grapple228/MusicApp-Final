namespace Music.Applications.Windows.Events;

public delegate void DialogCloseRequested();

public static class DialogEvents
{
    public static event DialogCloseRequested? DialogClose;

    public static void Close()
    {
        DialogClose?.Invoke();
    }
}