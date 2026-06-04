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
                options.UseSqlServer("Server=DBSRV\\vip2025;Database=бобор;Trusted_Connection=True;TrustServerCertificate=True;"));

            // Репозитории (обобщённый)
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<ExhibitsViewModel>();

            ServiceProvider = services.BuildServiceProvider();

            base.OnStartup(e);
        }
    }

}
