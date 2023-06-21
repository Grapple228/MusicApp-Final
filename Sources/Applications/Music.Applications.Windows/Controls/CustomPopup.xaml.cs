using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.ViewModels;

namespace Music.Applications.Windows.Controls;

public partial class CustomPopup : UserControl, IDisposable
{
    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(CustomPopup),
            new PropertyMetadata(new CornerRadius(10.0d)));

    public static readonly DependencyProperty PlacementProperty =
        DependencyProperty.Register(nameof(Placement), typeof(PlacementMode), typeof(CustomPopup),
            new PropertyMetadata(PlacementMode.Bottom));

    public static readonly DependencyProperty IsOpenProperty =
        DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(CustomPopup),
            new PropertyMetadata(false));

    public static readonly DependencyProperty TargetProperty =
        DependencyProperty.Register(nameof(Target), typeof(UIElement), typeof(CustomPopup),
            new PropertyMetadata(null));

    public static readonly DependencyProperty IsShownOnHoverProperty =
        DependencyProperty.Register(nameof(IsShownOnHover), typeof(bool), typeof(CustomPopup),
            new PropertyMetadata(false));

    public static readonly DependencyProperty HorizontalOffsetProperty =
        DependencyProperty.Register(nameof(HorizontalOffset), typeof(double), typeof(CustomPopup),
            new PropertyMetadata(0.0d));

    public CustomPopup()
    {
        InitializeComponent();
    }

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public PlacementMode Placement
    {
        get => (PlacementMode)GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }

    public bool IsOpen
    {
        get => (bool)GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public UIElement? Target
    {
        get => (UIElement)GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    public bool IsShownOnHover
    {
        get => (bool)GetValue(IsShownOnHoverProperty);
        set => SetValue(IsShownOnHoverProperty, value);
    }

    public double HorizontalOffset
    {
        get => (double)GetValue(HorizontalOffsetProperty);
        set => SetValue(HorizontalOffsetProperty, value);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    private void PopupComponent_OnMouseLeave(object sender, MouseEventArgs e)
    {
        if (!IsShownOnHover) return;
        if (Target is { IsMouseOver: true }) return;
        var vm = App.ServiceProvider.GetRequiredService<MainViewModel>();
        vm.PopupViewModel.Close();
    }
}