using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VRA.App.Controls;
using VRA.App.Infrastructure;
using VRA.App.Infrastructure.Dialogs;
using VRA.App.Infrastructure.Navigation;
using VRA.App.Services;
using VRA.App.ViewModels;
using VRA.App.Views;
using VRA.App.Views.Dialogs;
using VRA.App.Views.Pages;
using VRA.Core.Data;
using VRA.Core.Interfaces;
using VRA.Core.Services;
using VRA.App.Configuration;

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
                services.AddSingleton<NavigationService>();
                services.AddSingleton<CurrentPatientService>();
                services.AddScoped<IHearingTestSessionService, HearingTestSessionService>();
                services.AddSingleton<CurrentHearingTestSessionService>();
                services.AddSingleton<IReinforcementService, SimulatedReinforcementService>();
                services.AddSingleton<IDialogService, DialogService>();
                // ViewModels
                services.AddTransient<DashboardViewModel>();
                services.AddTransient<PatientDetailsViewModel>();
                services.AddSingleton<MainWindowViewModel>();
                services.AddTransient<PatientsViewModel>();
                services.AddSingleton<SidebarViewModel>();
                services.AddTransient<NewPatientViewModel>();
                services.AddTransient<HearingTestViewModel>();

                services.AddTransient<ReinforcementViewModel>();
                services.AddTransient<ReinforcementView>();
                // Views
                services.AddTransient<DashboardView>();
                services.AddTransient<PatientsView>();
                services.AddTransient<PatientDetailsView>();
                services.AddTransient<SidebarControl>();
                services.AddTransient<HearingTestView>();

                // Windows
                services.AddSingleton<MainWindow>();
                services.AddTransient<NewPatientWindow>();

                //configuration
                services.AddSingleton<ReinforcementConfiguration>(DefaultReinforcementConfiguration.Create());
            });
    }

}