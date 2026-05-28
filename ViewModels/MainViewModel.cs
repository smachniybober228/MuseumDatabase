using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Museum.Models;
using Museum.Repository;
using System.Collections.ObjectModel;

namespace Museum.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IRepository<Exhibit> _exhibitRepo;
        private readonly IRepository<Exhibition> _exhibitionRepo;
        private readonly IRepository<RestorationOrderEntity> _orderRepo;
        private readonly IRepository<Ticket> _ticketRepo;

        [ObservableProperty]
        private ObservableCollection<Exhibit> exhibits;

        [ObservableProperty]
        private ObservableCollection<Exhibition> exhibitions;

        // Команды для навигации
        [RelayCommand]
        private void OpenExhibitManagement() { }

        [RelayCommand]
        private void OpenExhibitionManagement() { }

        [RelayCommand]
        private void OpenRestorationManagement() { }

        [RelayCommand]
        private void OpenTicketSales() { }

        [RelayCommand]
        private void ShowReports() { }
    }
}
