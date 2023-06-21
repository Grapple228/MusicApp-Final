using System.Windows.Media;
using Music.Applications.Windows.Core;
using Music.Applications.Windows.Events;

namespace Music.Applications.Windows.Models.Popup;

public class PopupNavigationModel : ObservableObject
{
    private PathGeometry _image;
    private bool _isDanger;

    private string _label;

    public PopupNavigationModel(string label, PathGeometry image, Action action, bool isDanger = false)
    {
        Action = action;
        Label = label;
        Image = image;
        IsDanger = isDanger;
    }

    private Action Action { get; }

    public string Label
    {
        get => _label;
        set
        {
            _label = value;
            OnPropertyChanged();
        }
    }

    public PathGeometry Image
    {
        get => _image;
        set
        {
            _image = value;
            OnPropertyChanged();
        }
    }

    public bool IsDanger
    {
        get => _isDanger;
        set
        {
            _isDanger = value;
            OnPropertyChanged();
        }
    }

    public void Execute()
    {
        PopupEvents.Close();
        Action.Invoke();
    }
}