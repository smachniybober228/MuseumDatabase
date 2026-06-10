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

            // Регистрация DbContext как Scoped (для репозиториев)
            services.AddDbContext<MuseumDbContext>(options =>
                options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=бобор;Trusted_Connection=True;TrustServerCertificate=True;"),
                ServiceLifetime.Scoped);  // важно: Scoped, не Singleton

            // Регистрация фабрики (для TicketSalesViewModel)
            services.AddDbContextFactory<MuseumDbContext>(options =>
                options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=бобор;Trusted_Connection=True;TrustServerCertificate=True;"));

            // Репозитории
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IRepository<Exhibition>, Repository<Exhibition>>();
            services.AddScoped<IRepository<Hall>, Repository<Hall>>();
            services.AddScoped<IRepository<Person>, Repository<Person>>();
            services.AddScoped<IRepository<RoleEntity>, Repository<RoleEntity>>();
            services.AddScoped<IRepository<PersonRole>, Repository<PersonRole>>();
            services.AddScoped<IRepository<ExhibitionStatus>, Repository<ExhibitionStatus>>();
            services.AddScoped<IRepository<PersonType>, Repository<PersonType>>();
            services.AddScoped<IRepository<ExpositionPlaceType>, Repository<ExpositionPlaceType>>();
            services.AddScoped<IRepository<ExhibitOnExhibition>, Repository<ExhibitOnExhibition>>();
            services.AddScoped<IRepository<RestorationOrderEntity>, Repository<RestorationOrderEntity>>();
            services.AddScoped<IRepository<RestorationWorkType>, Repository<RestorationWorkType>>();
            services.AddScoped<IRepository<RequiredWorkType>, Repository<RequiredWorkType>>();
            services.AddScoped<IRepository<WorkLogEntry>, Repository<WorkLogEntry>>();
            services.AddScoped<IRepository<RestorationAct>, Repository<RestorationAct>>();
            services.AddScoped<IRepository<ReturnAct>, Repository<ReturnAct>>();
            services.AddScoped<IRepository<Ticket>, Repository<Ticket>>();
            services.AddScoped<IRepository<TicketStatus>, Repository<TicketStatus>>();
            services.AddScoped<IRepository<Exhibit>, Repository<Exhibit>>();

            // ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<ExhibitsViewModel>();
            services.AddTransient<ExhibitionsViewModel>();
            services.AddTransient<RestorationViewModel>();
            services.AddTransient<TicketSalesViewModel>();
            services.AddTransient<ReportsViewModel>();
            services.AddTransient<PeopleViewModel>();

            // Views
            services.AddTransient<Exhibits>();
            services.AddTransient<Exhibitions>();
            services.AddTransient<Restoration>();
            services.AddTransient<TicketSales>();
            services.AddTransient<Reports>();
            services.AddTransient<People>();

            ServiceProvider = services.BuildServiceProvider();

            base.OnStartup(e);
        }
    }

}
