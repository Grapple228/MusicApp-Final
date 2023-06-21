using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Music.Applications.Windows.Controls;

public partial class ImageButton : UserControl
{
    public static readonly DependencyProperty ImageProperty =
        DependencyProperty.Register(nameof(Image), typeof(PathGeometry), typeof(ImageButton),
            new PropertyMetadata(null));

    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(ImageButton),
            new PropertyMetadata(new CornerRadius(5)));

    public static readonly DependencyProperty ButtonColorProperty =
        DependencyProperty.Register(nameof(ButtonColor), typeof(Brush), typeof(ImageButton),
            new PropertyMetadata(Brushes.White));

    public static readonly DependencyProperty ImageColorProperty =
        DependencyProperty.Register(nameof(ImageColor), typeof(Brush), typeof(ImageButton),
            new PropertyMetadata(Brushes.Black));

    public static readonly DependencyProperty ImageWidthProperty =
        DependencyProperty.Register(nameof(ImageWidth), typeof(double),
            typeof(ImageButton),
            new FrameworkPropertyMetadata(double.NaN,
                FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty ImageHeightProperty =
        DependencyProperty.Register(nameof(ImageHeight), typeof(double),
            typeof(ImageButton),
            new FrameworkPropertyMetadata(double.NaN,
                FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty HoverColorProperty =
        DependencyProperty.Register(nameof(HoverColor), typeof(Brush), typeof(ImageButton),
            new PropertyMetadata(Brushes.Black));

    public static readonly RoutedEvent ClickedEvent = EventManager.RegisterRoutedEvent(nameof(Clicked),
        RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ImageButton));

    public ImageButton()
    {
        InitializeComponent();
    }

    public PathGeometry Image
    {
        get => (PathGeometry)GetValue(ImageProperty);
        set => SetValue(ImageProperty, value);
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

    public Brush ImageColor
    {
        get => (Brush)GetValue(ImageColorProperty);
        set => SetValue(ImageColorProperty, value);
    }

    public double ImageWidth
    {
        get => (double)GetValue(ImageWidthProperty);
        set => SetValue(ImageWidthProperty, value);
    }

    public double ImageHeight
    {
        get => (double)GetValue(ImageHeightProperty);
        set => SetValue(ImageHeightProperty, value);
    }

    public Brush HoverColor
    {
        get => (Brush)GetValue(HoverColorProperty);
        set => SetValue(HoverColorProperty, value);
    }

    public event RoutedEventHandler Clicked
    {
        add => AddHandler(ClickedEvent, value);
        remove => RemoveHandler(ClickedEvent, value);
    }

    private void ImageButton_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(ClickedEvent, sender));
    }
}