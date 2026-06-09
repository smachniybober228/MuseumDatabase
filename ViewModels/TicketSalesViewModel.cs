using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Museum.Models;
using Museum.Repository;
using Museum.Views;
using System.Collections.ObjectModel;
using System.Windows;

namespace Museum.ViewModels
{
    public partial class TicketSalesViewModel : ObservableObject
    {
        private readonly IRepository<Ticket> _ticketRepo;
        private readonly IRepository<Exhibition> _exhibitionRepo;
        private readonly IRepository<TicketStatus> _ticketStatusRepo;

        [ObservableProperty]
        private ObservableCollection<Exhibition> _exhibitions;

        [ObservableProperty]
        private Exhibition _selectedExhibition;

        [ObservableProperty]
        private ObservableCollection<Ticket> _ticketsForSelectedExhibition;

        [ObservableProperty]
        private Ticket _selectedTicket;

        [ObservableProperty]
        private DateTime _visitDate = DateTime.Today;

        [ObservableProperty]
        private double _salePrice = 500;

        public TicketSalesViewModel(
            IRepository<Ticket> ticketRepo,
            IRepository<Exhibition> exhibitionRepo,
            IRepository<TicketStatus> ticketStatusRepo)
        {
            _ticketRepo = ticketRepo;
            _exhibitionRepo = exhibitionRepo;
            _ticketStatusRepo = ticketStatusRepo;
            LoadExhibitionsAsync();
        }

        private async Task LoadExhibitionsAsync()
        {
            var list = await _exhibitionRepo.GetAllAsync();
            Exhibitions = new ObservableCollection<Exhibition>(list);
        }

        [RelayCommand]
        private async Task LoadTicketsForExhibition()
        {
            if (SelectedExhibition == null)
            {
                TicketsForSelectedExhibition?.Clear();
                return;
            }
            var allTickets = await _ticketRepo.GetAllAsync();
            var filtered = allTickets.Where(t => t.ExhibitionFk == SelectedExhibition.Id);
            TicketsForSelectedExhibition = new ObservableCollection<Ticket>(filtered);
        }

        [RelayCommand]
        private async Task SellTicket()
        {
            if (SelectedExhibition == null)
            {
                MessageBox.Show("Выберите выставку.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var soldStatus = (await _ticketStatusRepo.GetAllAsync()).FirstOrDefault(s => s.Title == "Продан");
            if (soldStatus == null)
            {
                MessageBox.Show("Статус 'Продан' не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var ticket = new Ticket
            {
                TicketNumber = GenerateTicketNumber(),
                ExhibitionFk = SelectedExhibition.Id,
                SaleDateTime = DateTime.Now,
                SalePrice = SalePrice,
                VisitDate = VisitDate,
                TicketStatusFk = soldStatus.Id
            };

            await _ticketRepo.AddAsync(ticket);
            await _ticketRepo.SaveAsync();
            await LoadTicketsForExhibition();
            MessageBox.Show($"Билет {ticket.TicketNumber} продан.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private async Task UseTicket()
        {
            if (SelectedTicket == null)
            {
                MessageBox.Show("Выберите билет для отметки.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (SelectedTicket.TicketStatusFkNavigation?.Title == "Использован")
            {
                MessageBox.Show("Билет уже использован.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var usedStatus = (await _ticketStatusRepo.GetAllAsync()).FirstOrDefault(s => s.Title == "Использован");
            if (usedStatus == null)
            {
                MessageBox.Show("Статус 'Использован' не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SelectedTicket.TicketStatusFk = usedStatus.Id;
            _ticketRepo.Update(SelectedTicket);
            await _ticketRepo.SaveAsync();
            await LoadTicketsForExhibition();
            MessageBox.Show("Билет отмечен как использованный.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private string GenerateTicketNumber() =>
            $"T-{DateTime.Now:yyyyMMddHHmmss}-{new Random().Next(1000)}";
    }
}