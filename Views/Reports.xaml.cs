using Microsoft.Extensions.DependencyInjection;
using Museum.ViewModels;
using System.Windows.Controls;

namespace Museum.Views
{
    /// <summary>
    /// Логика взаимодействия для Reports.xaml
    /// </summary>
    public partial class Reports : UserControl
    {
        public Reports()
        {
            InitializeComponent();
            var vm = App.ServiceProvider.GetRequiredService<ReportsViewModel>();
            DataContext = vm;
            Loaded += async (s, e) =>
            {
                await vm.LoadExhibitionsAsync();
                await vm.LoadAllExhibitsAsync();
                await vm.LoadRestorationExhibitsAsync();
            };
        }
    }
}
