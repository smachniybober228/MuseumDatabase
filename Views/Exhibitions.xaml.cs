using Microsoft.Extensions.DependencyInjection;
using Museum.ViewModels;
using System.Windows.Controls;

namespace Museum.Views
{
    public partial class Exhibitions : UserControl
    {
        public Exhibitions()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<ExhibitionsViewModel>();
        }
    }
}