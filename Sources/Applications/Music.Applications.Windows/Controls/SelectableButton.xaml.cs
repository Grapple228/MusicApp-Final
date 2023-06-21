using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Music.Applications.Windows.Controls;

public partial class SelectableButton : UserControl
{
    public static readonly DependencyProperty IsSelectedProperty =
        DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(SelectableButton),
            new PropertyMetadata(false));

    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(SelectableButton),
            new PropertyMetadata(new CornerRadius(5)));

    public static readonly DependencyProperty ButtonColorProperty =
        DependencyProperty.Register(nameof(ButtonColor), typeof(Brush), typeof(SelectableButton),
            new PropertyMetadata(Brushes.White));

    public static readonly DependencyProperty SelectedColorProperty =
        DependencyProperty.Register(nameof(SelectedColor), typeof(Brush), typeof(SelectableButton),
            new PropertyMetadata(Brushes.ForestGreen));

    public static readonly DependencyProperty LabelColorProperty =
        DependencyProperty.Register(nameof(LabelColor), typeof(Brush), typeof(SelectableButton),
            new PropertyMetadata(Brushes.Black));

    public static readonly DependencyProperty SelectedLabelColorProperty =
        DependencyProperty.Register(nameof(SelectedLabelColor), typeof(Brush), typeof(SelectableButton),
            new PropertyMetadata(Brushes.Black));


    public static readonly DependencyProperty LabelTextProperty =
        DependencyProperty.Register(nameof(LabelText), typeof(string), typeof(SelectableButton),
            new PropertyMetadata("Label"));


    public static readonly RoutedEvent ClickedEvent = EventManager.RegisterRoutedEvent(nameof(Clicked),
        RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SelectableButton));

    public SelectableButton()
    {
        InitializeComponent();
    }

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public Brush ButtonColor
    {
        get => (Brush)GetValue(ButtonColorProperty);
        set => SetValue(ButtonColorProperty, value);
    }

    public Brush SelectedColor
    {
        get => (Brush)GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }

    public Brush LabelColor
    {
        get => (Brush)GetValue(LabelColorProperty);
        set => SetValue(LabelColorProperty, value);
    }

    public Brush SelectedLabelColor
    {
        get => (Brush)GetValue(SelectedLabelColorProperty);
        set => SetValue(SelectedLabelColorProperty, value);
    }

    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public event RoutedEventHandler Clicked
    {
        add => AddHandler(ClickedEvent, value);
        remove => RemoveHandler(ClickedEvent, value);
    }

    private void SelectableButton_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(ClickedEvent, sender));
    }
}