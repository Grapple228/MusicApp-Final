using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Music.Applications.Windows.Services;

namespace Music.Applications.Windows.Controls;

public sealed partial class Dialog : UserControl
{
    public static readonly DependencyProperty FormColorProperty =
        DependencyProperty.Register(nameof(FormColor), typeof(Brush), typeof(Dialog),
            new PropertyMetadata(Brushes.Black));

    public static readonly DependencyProperty IsOpenProperty =
        DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(Dialog),
            new PropertyMetadata(false));

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(Dialog),
            new PropertyMetadata("Title"));

    public static readonly DependencyProperty DialogWidthProperty =
        DependencyProperty.Register(nameof(DialogWidth), typeof(double), typeof(Dialog),
            new PropertyMetadata(150.0));

    public static readonly RoutedEvent CloseRequestedEvent = EventManager.RegisterRoutedEvent(nameof(CloseRequested),
        RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Dialog));

    public Dialog()
    {
        InitializeComponent();
    }

    public Brush FormColor
    {
        get => (Brush)GetValue(FormColorProperty);
        set => SetValue(FormColorProperty, value);
    }

    public double DialogWidth
    {
        get => (double)GetValue(DialogWidthProperty);
        set => SetValue(DialogWidthProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public bool IsOpen
    {
        get => (bool)GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public event RoutedEventHandler CloseRequested
    {
        add => AddHandler(CloseRequestedEvent, value);
        remove => RemoveHandler(CloseRequestedEvent, value);
    }

    private void CloseRequestedHandler(object sender, MouseButtonEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(CloseRequestedEvent));
    }

    private void UIElement_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        ApplicationService.OnPreviewMouseWheel(sender, e);
    }
}