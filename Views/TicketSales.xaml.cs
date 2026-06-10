using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using Museum.ViewModels;

namespace Museum.Views
{
    public partial class TicketSales : UserControl
    {
        public TicketSales()
        {
            InitializeComponent();
            Loaded += async (s, e) =>
            {
                var vm = App.ServiceProvider.GetRequiredService<TicketSalesViewModel>();
                DataContext = vm;
                await vm.LoadExhibitionsAsync();
            };
        }

        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is TicketSalesViewModel vm)
                await vm.LoadTicketsForExhibition();
        }
    }
}