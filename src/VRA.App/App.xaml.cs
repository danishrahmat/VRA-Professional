
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using VRA.App.Configuration;
using VRA.Core.Data;
using VRA.App.VideoLibrary;

namespace VRA.App;

public partial class App : Application
{
    private readonly IHost _host;

    public App()
    {
        _host = Program.CreateHostBuilder().Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        try
        {
            await _host.StartAsync();

            // Resolve configuration early so validation runs before database initialization.
            _host.Services.GetRequiredService<ReinforcementConfiguration>();
            // Create the three shared reinforcement video folders.
            _host.Services.GetRequiredService<ReinforcementVideoLibraryService>();

            // Create database if it doesn't exist.
            await DatabaseInitializer.InitializeAsync(_host.Services);

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }
        catch (ReinforcementConfigurationException ex)
        {
            MessageBox.Show(
                $"VRA Professional cannot start because its reinforcement settings are invalid.\n\n{ex.Message}\n\nPlease check appsettings.json.",
                "Configuration Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Shutdown(-1);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);

            MessageBox.Show(
                "VRA Professional could not start. Please check the application logs or contact technical support.",
                "Startup Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Shutdown(-1);
        }
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        try
        {
            await _host.StopAsync();
        }
        finally
        {
            _host.Dispose();
            base.OnExit(e);
        }
    }
}
