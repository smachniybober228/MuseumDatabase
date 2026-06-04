using Microsoft.Extensions.DependencyInjection;
using Museum.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Museum.Views
{
    /// <summary>
    /// Логика взаимодействия для Exhibits.xaml
    /// </summary>
    public partial class Exhibits : UserControl
    {
        public Exhibits()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<ExhibitsViewModel>();
        }
    }
}
