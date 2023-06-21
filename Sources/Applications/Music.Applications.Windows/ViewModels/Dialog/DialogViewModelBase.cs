using Music.Applications.Windows.Core;
using Music.Applications.Windows.Events;

namespace Music.Applications.Windows.ViewModels.Dialog;

public abstract class DialogViewModelBase : ViewModelBase
{
    private ViewModelBase? _content;
    private bool _disposed;
    private bool _isOpen;
    private string? _title;

    protected DialogViewModelBase()
    {
        DialogEvents.DialogClose += DialogEventsOnDialogClose;
        GlobalEvents.UpdateRequested += GlobalEventsOnUpdateRequested;
    }

    public bool IsOpen
    {
        get => _isOpen;
        set
        {
            _isOpen = value;
            OnPropertyChanged();
        }
    }

    public ViewModelBase? Content
    {
        get => _content;
        private set
        {
            _content = value;
            OnPropertyChanged();
        }
    }

    public string? Title
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged();
        }
    }

    private void GlobalEventsOnUpdateRequested()
    {
        if (Content is LoadableViewModel loadableViewModel) Task.Run(() => loadableViewModel.Reload());
    }

    public void Close()
    {
        IsOpen = false;
        Title = null;
        Content?.Dispose();
        Content = null;
    }

    public virtual void Open(string? title, ViewModelBase? content)
    {
        Content?.Dispose();
        Content = content;
        Title = title ?? "Dialog";
        IsOpen = true;
    }

    public override void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            IsOpen = false;

            _content?.Dispose();
            DialogEvents.DialogClose -= DialogEventsOnDialogClose;
            GlobalEvents.UpdateRequested -= GlobalEventsOnUpdateRequested;
        }

        _disposed = true;
    }

    private void DialogEventsOnDialogClose()
    {
        Close();
    }

    ~DialogViewModelBase()
    {
        Dispose(false);
    }
}