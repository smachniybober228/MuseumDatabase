using System.Windows.Controls;
using Museum.ViewModels;

namespace Museum.Views
{
    /// <summary>
    /// Логика взаимодействия для Exhibits.xaml
    /// </summary>
    public partial class Exhibits : UserControl
    {
        private readonly ExhibitsViewModel viewModel;
        public Exhibits()
        {
            InitializeComponent();

            viewModel = new ExhibitsViewModel();
            DataContext = viewModel;
        }
    }
}
