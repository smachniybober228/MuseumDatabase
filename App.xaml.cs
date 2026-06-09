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
                options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=бобор;Trusted_Connection=True;TrustServerCertificate=True;"));
            // Для работы в колледже: "Server=DBSRV\\vip2025;Database=бобор;Trusted_Connection=True;TrustServerCertificate=True;"

            // Репозитории
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IRepository<Exhibition>, Repository<Exhibition>>();
            services.AddScoped<IRepository<Hall>, Repository<Hall>>();
            services.AddScoped<IRepository<Person>, Repository<Person>>();
            services.AddScoped<IRepository<ExhibitionStatus>, Repository<ExhibitionStatus>>();
            services.AddScoped<IRepository<ExpositionPlaceType>, Repository<ExpositionPlaceType>>();
            services.AddScoped<IRepository<ExhibitOnExhibition>, Repository<ExhibitOnExhibition>>();

            // ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<ExhibitsViewModel>();
            services.AddTransient<ExhibitionsViewModel>();

            services.AddTransient<Exhibits>();
            services.AddTransient<Exhibitions>();

            ServiceProvider = services.BuildServiceProvider();

            base.OnStartup(e);
        }
    }

}
