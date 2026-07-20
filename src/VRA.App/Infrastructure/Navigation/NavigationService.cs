using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Controls;

namespace VRA.App.Infrastructure.Navigation;

public class NavigationService
{
    private readonly IServiceProvider _serviceProvider;

    public event Action<UserControl>? ViewChanged;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Navigate<TView>()
        where TView : UserControl
    {
        var view = _serviceProvider.GetRequiredService<TView>();

        ViewChanged?.Invoke(view);
    }
}