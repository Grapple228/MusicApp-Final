using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Music.Applications.Windows.Controls;

public partial class SelectableImageButton : UserControl
{
    public static readonly DependencyProperty ImageProperty =
        DependencyProperty.Register(nameof(Image), typeof(PathGeometry), typeof(SelectableImageButton),
            new PropertyMetadata(null));

    public static readonly DependencyProperty IsSelectedProperty =
        DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(SelectableImageButton),
            new PropertyMetadata(false));

    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(SelectableImageButton),
            new PropertyMetadata(new CornerRadius(5)));

    public static readonly DependencyProperty ButtonColorProperty =
        DependencyProperty.Register(nameof(ButtonColor), typeof(Brush), typeof(SelectableImageButton),
            new PropertyMetadata(Brushes.White));

    public static readonly DependencyProperty SelectedColorProperty =
        DependencyProperty.Register(nameof(SelectedColor), typeof(Brush), typeof(SelectableImageButton),
            new PropertyMetadata(Brushes.ForestGreen));

    public static readonly DependencyProperty LabelColorProperty =
        DependencyProperty.Register(nameof(LabelColor), typeof(Brush), typeof(SelectableImageButton),
            new PropertyMetadata(Brushes.Black));

    public static readonly DependencyProperty SelectedLabelColorProperty =
        DependencyProperty.Register(nameof(SelectedLabelColor), typeof(Brush), typeof(SelectableImageButton),
            new PropertyMetadata(Brushes.Black));


    public static readonly DependencyProperty LabelTextProperty =
        DependencyProperty.Register(nameof(LabelText), typeof(string), typeof(SelectableImageButton),
            new PropertyMetadata("Label"));


    public static readonly RoutedEvent ClickedEvent = EventManager.RegisterRoutedEvent(nameof(Clicked),
        RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SelectableImageButton));

    public SelectableImageButton()
    {
        InitializeComponent();
    }

    public PathGeometry Image
    {
        get => (PathGeometry)GetValue(ImageProperty);
        set => SetValue(ImageProperty, value);
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

    private void SelectableImageButton_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(ClickedEvent, sender));
    }
}