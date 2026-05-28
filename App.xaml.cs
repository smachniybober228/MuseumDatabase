using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Museum.Models;
using Museum.Repository;
using Museum.ViewModels;
using Museum.Views;
using System.Windows;

namespace Museum
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            // Регистрация DbContext
            services.AddDbContext<MuseumDbContext>(options =>
                options.UseSqlServer("YourConnectionString"));

            // Регистрация обобщённого репозитория
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Регистрация ViewModel и View
            services.AddTransient<ExhibitsViewModel>();
            services.AddTransient<Exhibits>();
            // ... остальные

            ServiceProvider = services.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            base.OnStartup(e);
        }
    }

}
