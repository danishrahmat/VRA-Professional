using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VRA.App.ViewModels;

namespace VRA.App;

public static class Program
{
    public static IHostBuilder CreateHostBuilder(string[]? args = null)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // ViewModels
                services.AddSingleton<MainWindowViewModel>();

                // Windows
                services.AddSingleton<MainWindow>();
            });
    }
}