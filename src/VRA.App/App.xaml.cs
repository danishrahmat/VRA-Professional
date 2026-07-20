using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using VRA.Core.Data;

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
        await _host.StartAsync();

        // Create database if it doesn't exist
        await DatabaseInitializer.InitializeAsync(_host.Services);

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();

        mainWindow.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();

        _host.Dispose();

        base.OnExit(e);
    }


}