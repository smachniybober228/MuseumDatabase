using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using Museum.ViewModels;

namespace Museum.Views
{
    public partial class TicketSales : UserControl
    {
        private readonly TicketSalesViewModel _viewModel;
        public TicketSales()
        {
            InitializeComponent();
            _viewModel = App.ServiceProvider.GetRequiredService<TicketSalesViewModel>();
            DataContext = _viewModel;
        }

        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await _viewModel.LoadTicketsForExhibitionCommand.ExecuteAsync(null);
        }
    }
}