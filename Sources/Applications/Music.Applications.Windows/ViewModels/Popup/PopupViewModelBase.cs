using System.Windows;
using Music.Applications.Windows.Core;
using Music.Applications.Windows.Events;

namespace Music.Applications.Windows.ViewModels.Popup;

public abstract class PopupViewModelBase : ObservableObject, IDisposable
{
    private ViewModelBase? _content;

    private bool _disposed;
    private double _horizontalOffset;
    private bool _isOpen;
    private bool _isShownOnHover;

    private double _offset;
    private UIElement? _target;
    private double _width;

    protected PopupViewModelBase()
    {
        PopupEvents.PopupClose += PopupEventsOnPopupClose;
        GlobalEvents.UpdateRequested += GlobalEventsOnUpdateRequested;
        Width = 160;
    }

    public double Width
    {
        get => _width;
        set
        {
            if (value.Equals(_width)) return;
            _width = value;
            OnPropertyChanged();
            HorizontalOffset = -_width;
        }
    }

    public double HorizontalOffset
    {
        get => _horizontalOffset;
        set
        {
            _offset = value;
            _horizontalOffset = _offset + (Target is FrameworkElement element ? element.Width : 0);
            OnPropertyChanged();
        }
    }

    public UIElement? Target
    {
        get => _target;
        set
        {
            _target = value;
            OnPropertyChanged();
            HorizontalOffset = _offset;
        }
    }

    public bool IsShownOnHover
    {
        get => _isShownOnHover;
        set
        {
            if (value == _isShownOnHover) return;
            _isShownOnHover = value;
            OnPropertyChanged();
        }
    }

    public bool IsOpen
    {
        get => _isOpen;
        protected set
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
            _content?.Dispose();
            _content = value;
            OnPropertyChanged();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void GlobalEventsOnUpdateRequested()
    {
        Close();
    }

    private void PopupEventsOnPopupClose()
    {
        Close();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            IsOpen = false;
            _content?.Dispose();
            _content = null;
            PopupEvents.PopupClose -= PopupEventsOnPopupClose;
            GlobalEvents.UpdateRequested -= GlobalEventsOnUpdateRequested;
        }

        _disposed = true;
    }

    ~PopupViewModelBase()
    {
        Dispose(false);
    }

    public virtual void ChangeState(UIElement? target, ViewModelBase? content)
    {
        if (IsOpen)
        {
            var isTargetEqual = Target == target;
            if (isTargetEqual)
            {
                if (Content != null && content != null && Content.Equals(content))
                    Close();
                else
                    Open(target, content, isTargetEqual);
            }
            else
            {
                if (Content?.GetType() == content?.GetType()
                    && content is PopupNavigationViewModelBase c
                    && Content is PopupNavigationViewModelBase curC)
                {
                    curC.Navigations.Clear();
                    foreach (var popupNavigationModel in c.Navigations) curC.Navigations.Add(popupNavigationModel);

                    Open(target, Content);
                    return;
                }

                Open(target, content, isTargetEqual);
            }
        }
        else
        {
            Open(target, content);
        }
    }

    public virtual void Open(UIElement? target, ViewModelBase? content, bool isTargetEqual = false)
    {
        if (!isTargetEqual)
        {
            IsOpen = false;
            Target = null;
        }

        Content = content;

        Target = target;
        IsOpen = true;
    }

    public virtual void Close()
    {
        IsOpen = false;
        Target = null;
        Content = null;
    }
}