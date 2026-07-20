using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VRA.App.ViewModels;
using VRA.Core.Data;
using VRA.Core.Interfaces;
using VRA.Core.Services;

namespace VRA.App;

public static class Program
{
    public static IHostBuilder CreateHostBuilder(string[]? args = null)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Database
                services.AddDbContext<VraDbContext>(options =>
                    options.UseSqlite(
                        context.Configuration.GetConnectionString("DefaultConnection")));

                // Services
                services.AddScoped<IPatientService, PatientService>();

                // ViewModels
                services.AddSingleton<MainWindowViewModel>();

                // Windows
                services.AddSingleton<MainWindow>();
            });
    }
}