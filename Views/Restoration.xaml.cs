using Microsoft.Extensions.DependencyInjection;
using Museum.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Museum.Views
{
    /// <summary>
    /// Логика взаимодействия для Restoration.xaml
    /// </summary>
    public partial class Restoration : UserControl
    {
        public Restoration()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<RestorationViewModel>();
        }
    }
}
