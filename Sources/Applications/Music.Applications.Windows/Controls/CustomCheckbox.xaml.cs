using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Music.Applications.Windows.Controls;

public partial class CustomCheckbox : UserControl
{
    public static readonly DependencyProperty IsCheckedProperty =
        DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(CustomCheckbox),
            new PropertyMetadata(false));

    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(CustomCheckbox),
            new PropertyMetadata(new CornerRadius(5)));

    public static readonly DependencyProperty BoxWidthProperty =
        DependencyProperty.Register(nameof(BoxWidth), typeof(double), typeof(CustomCheckbox),
            new FrameworkPropertyMetadata(double.NaN,
                FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty BoxHeightProperty =
        DependencyProperty.Register(nameof(BoxHeight), typeof(double), typeof(CustomCheckbox),
            new FrameworkPropertyMetadata(double.NaN,
                FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty LabelColorProperty =
        DependencyProperty.Register(nameof(LabelColor), typeof(Brush), typeof(CustomCheckbox),
            new PropertyMetadata(Brushes.Black));

    public static readonly DependencyProperty LabelTextProperty =
        DependencyProperty.Register(nameof(LabelText), typeof(string), typeof(CustomCheckbox),
            new PropertyMetadata("Label"));

    public static readonly DependencyProperty SelectedBoxColorProperty =
        DependencyProperty.Register(nameof(SelectedBoxColor), typeof(Brush), typeof(CustomCheckbox),
            new PropertyMetadata(Brushes.Black));

    public static readonly DependencyProperty BoxColorProperty =
        DependencyProperty.Register(nameof(BoxColor), typeof(Brush), typeof(CustomCheckbox),
            new PropertyMetadata(Brushes.White));

    public static readonly RoutedEvent CheckedEvent = EventManager.RegisterRoutedEvent(nameof(Checked),
        RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SelectableButton));

    public CustomCheckbox()
    {
        InitializeComponent();
    }

    public bool IsChecked
    {
        get => (bool)GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public double BoxWidth
    {
        get => (double)GetValue(BoxWidthProperty);
        set => SetValue(BoxWidthProperty, value);
    }

    public double BoxHeight
    {
        get => (double)GetValue(BoxHeightProperty);
        set => SetValue(BoxHeightProperty, value);
    }

    public Brush LabelColor
    {
        get => (Brush)GetValue(LabelColorProperty);
        set => SetValue(LabelColorProperty, value);
    }

    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public Brush SelectedBoxColor
    {
        get => (Brush)GetValue(SelectedBoxColorProperty);
        set => SetValue(SelectedBoxColorProperty, value);
    }

    public Brush BoxColor
    {
        get => (Brush)GetValue(BoxColorProperty);
        set => SetValue(BoxColorProperty, value);
    }

    public event RoutedEventHandler Checked
    {
        add => AddHandler(CheckedEvent, value);
        remove => RemoveHandler(CheckedEvent, value);
    }

    private void BoxLabel_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        IsChecked = !IsChecked;
        RaiseEvent(new RoutedEventArgs(CheckedEvent));
    }

    private void Box_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        IsChecked = !IsChecked;
        RaiseEvent(new RoutedEventArgs(CheckedEvent));
    }
}