using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Music.Applications.Windows.Components;

public partial class ReactionComponent : UserControl
{
    public static readonly DependencyProperty DistanceBetweenProperty =
        DependencyProperty.Register(nameof(DistanceBetween), typeof(double), typeof(ReactionComponent),
            new PropertyMetadata(5.0d));

    public static readonly DependencyProperty HoverColorProperty =
        DependencyProperty.Register(nameof(HoverColor), typeof(Brush), typeof(ReactionComponent),
            new PropertyMetadata(Brushes.Transparent));

    public static readonly DependencyProperty LikeColorProperty =
        DependencyProperty.Register(nameof(LikeColor), typeof(Brush), typeof(ReactionComponent),
            new PropertyMetadata(Brushes.Firebrick));

    public static readonly DependencyProperty BlockColorProperty =
        DependencyProperty.Register(nameof(BlockColor), typeof(Brush), typeof(ReactionComponent),
            new PropertyMetadata(Brushes.White));

    public static readonly DependencyProperty IsLikeVisibleProperty =
        DependencyProperty.Register(nameof(IsLikeVisible), typeof(bool), typeof(ReactionComponent),
            new PropertyMetadata(true));

    public static readonly DependencyProperty IsBlockVisibleProperty =
        DependencyProperty.Register(nameof(IsBlockVisible), typeof(bool), typeof(ReactionComponent),
            new PropertyMetadata(true));

    public static readonly DependencyProperty IsLikedProperty =
        DependencyProperty.Register(nameof(IsLiked), typeof(bool), typeof(ReactionComponent),
            new PropertyMetadata(false));

    public static readonly DependencyProperty IsBlockedProperty =
        DependencyProperty.Register(nameof(IsBlocked), typeof(bool), typeof(ReactionComponent),
            new PropertyMetadata(false));

    public static readonly RoutedEvent LikedEvent = EventManager.RegisterRoutedEvent(nameof(Liked),
        RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ReactionComponent));

    public static readonly RoutedEvent BlockedEvent = EventManager.RegisterRoutedEvent(nameof(Blocked),
        RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ReactionComponent));

    public ReactionComponent()
    {
        InitializeComponent();
    }

    public double DistanceBetween
    {
        get => (double)GetValue(DistanceBetweenProperty);
        set => SetValue(DistanceBetweenProperty, value);
    }

    public Brush HoverColor
    {
        get => (Brush)GetValue(HoverColorProperty);
        set => SetValue(HoverColorProperty, value);
    }

    public Brush LikeColor
    {
        get => (Brush)GetValue(LikeColorProperty);
        set => SetValue(LikeColorProperty, value);
    }

    public Brush BlockColor
    {
        get => (Brush)GetValue(BlockColorProperty);
        set => SetValue(BlockColorProperty, value);
    }

    public bool IsLikeVisible
    {
        get => (bool)GetValue(IsLikeVisibleProperty);
        set => SetValue(IsLikeVisibleProperty, value);
    }

    public bool IsBlockVisible
    {
        get => (bool)GetValue(IsBlockVisibleProperty);
        set => SetValue(IsBlockVisibleProperty, value);
    }

    public bool IsLiked
    {
        get => (bool)GetValue(IsLikedProperty);
        set => SetValue(IsLikedProperty, value);
    }

    public bool IsBlocked
    {
        get => (bool)GetValue(IsBlockedProperty);
        set => SetValue(IsBlockedProperty, value);
    }

    public event RoutedEventHandler Liked
    {
        add => AddHandler(LikedEvent, value);
        remove => RemoveHandler(LikedEvent, value);
    }

    public event RoutedEventHandler Blocked
    {
        add => AddHandler(BlockedEvent, value);
        remove => RemoveHandler(BlockedEvent, value);
    }

    private void LikeButton_0nClicked(object sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(LikedEvent));
    }

    private void BlockButton_0nClicked(object sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(BlockedEvent));
    }
}