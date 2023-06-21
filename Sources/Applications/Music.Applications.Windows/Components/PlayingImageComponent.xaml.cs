using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Music.Applications.Windows.Enums;

namespace Music.Applications.Windows.Components;

public partial class PlayingImageComponent : UserControl
{
    public static readonly DependencyProperty ImagePathProperty =
        DependencyProperty.Register(nameof(ImagePath), typeof(string), typeof(PlayingImageComponent),
            new PropertyMetadata(null));
    
    public string ImagePath
    {
        get => (string)GetValue(ImagePathProperty);
        set => SetValue(ImagePathProperty, value);
    }
    
    public static readonly DependencyProperty IsCurrentProperty =
        DependencyProperty.Register(nameof(IsCurrent), typeof(bool), typeof(PlayingImageComponent),
            new PropertyMetadata(false));
    
    public bool IsCurrent
    {
        get => (bool)GetValue(IsCurrentProperty);
        set => SetValue(IsCurrentProperty, value);
    }
    
    public static readonly DependencyProperty IsAlwaysShownProperty =
        DependencyProperty.Register(nameof(IsAlwaysShown), typeof(bool), typeof(PlayingImageComponent),
            new PropertyMetadata(false));
    
    public bool IsAlwaysShown
    {
        get => (bool)GetValue(IsAlwaysShownProperty);
        set => SetValue(IsAlwaysShownProperty, value);
    }
    
    public static readonly DependencyProperty IsPlayingProperty =
        DependencyProperty.Register(nameof(IsPlaying), typeof(bool), typeof(PlayingImageComponent),
            new PropertyMetadata(false));
    
    public bool IsPlaying
    {
        get => (bool)GetValue(IsPlayingProperty);
        set => SetValue(IsPlayingProperty, value);
    }
    
    public PlayingImageComponent()
    {
        InitializeComponent();
    }
    
    public static readonly RoutedEvent ClickedEvent = EventManager.RegisterRoutedEvent(nameof(Clicked),
        RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PlayingImageComponent));

    public event RoutedEventHandler Clicked
    {
        add => AddHandler(ClickedEvent, value);
        remove => RemoveHandler(ClickedEvent, value);
    }
    
    private void ImageBorder_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(ClickedEvent));
    }
}