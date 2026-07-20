using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VRA.App.Controls;
using VRA.App.Infrastructure.Navigation;
using VRA.App.ViewModels;
using VRA.App.Views;
using VRA.App.Views.Pages;
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
                // Views
                services.AddTransient<DashboardView>();
                services.AddTransient<PatientsView>();
                services.AddSingleton<NavigationService>();
                services.AddSingleton<MainWindowViewModel>();
                services.AddTransient<PatientsViewModel>();
                services.AddSingleton<SidebarViewModel>();
                services.AddTransient<SidebarControl>();

                // Windows
                services.AddSingleton<MainWindow>();
            });
    }

}