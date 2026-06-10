using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using Museum.ViewModels;

namespace Museum.Views
{
    public partial class People : UserControl
    {
        public People()
        {
            InitializeComponent();
            var vm = App.ServiceProvider.GetRequiredService<PeopleViewModel>();
            DataContext = vm;
            Loaded += async (s, e) => await vm.LoadPeopleAsync();
        }
    }
}