using System.Windows;
using System.Windows.Controls;

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
}