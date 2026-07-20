using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VRA.App.Controls;

public partial class NavigationButton : UserControl
{
    public NavigationButton()
    {
        InitializeComponent();
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(NavigationButton),
            new PropertyMetadata(string.Empty, OnTitleChanged));

    private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((NavigationButton)d).TitleText.Text = (string)e.NewValue;
    }

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(
            nameof(Icon),
            typeof(string),
            typeof(NavigationButton),
            new PropertyMetadata(string.Empty, OnIconChanged));

    private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((NavigationButton)d).IconText.Text = (string)e.NewValue;
    }

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public static readonly DependencyProperty IsSelectedProperty =
        DependencyProperty.Register(
            nameof(IsSelected),
            typeof(bool),
            typeof(NavigationButton),
            new PropertyMetadata(false, OnIsSelectedChanged));

    private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var button = (NavigationButton)d;

        if ((bool)e.NewValue)
        {
            button.Accent.Background = new SolidColorBrush(Color.FromRgb(0, 122, 204));
            button.Root.Background = new SolidColorBrush(Color.FromRgb(45, 48, 54));
            button.TitleText.FontWeight = FontWeights.Bold;
        }
        else
        {
            button.Accent.Background = Brushes.Transparent;
            button.Root.Background = Brushes.Transparent;
            button.TitleText.FontWeight = FontWeights.SemiBold;
        }
    }
}