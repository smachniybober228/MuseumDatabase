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
            DataContext = App.ServiceProvider.GetRequiredService<PeopleViewModel>();
        }
    }
}