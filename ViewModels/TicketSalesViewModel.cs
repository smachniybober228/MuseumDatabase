using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Museum.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace Museum.ViewModels
{
    public partial class TicketSalesViewModel : ObservableObject
    {
        private readonly IDbContextFactory<MuseumDbContext> _contextFactory;

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

        public TicketSalesViewModel(IDbContextFactory<MuseumDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task LoadExhibitionsAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var list = await context.Exhibitions.ToListAsync();
            Exhibitions = new ObservableCollection<Exhibition>(list);
        }

        [RelayCommand]
        internal async Task LoadTicketsForExhibition()
        {
            if (SelectedExhibition == null)
            {
                TicketsForSelectedExhibition?.Clear();
                return;
            }
            using var context = await _contextFactory.CreateDbContextAsync();
            var tickets = await context.Tickets
                .Include(t => t.TicketStatusFkNavigation)
                .Where(t => t.ExhibitionFk == SelectedExhibition.Id)
                .ToListAsync();
            TicketsForSelectedExhibition = new ObservableCollection<Ticket>(tickets);
        }

        [RelayCommand]
        private async Task SellTicket()
        {
            if (SelectedExhibition == null)
            {
                MessageBox.Show("Выберите выставку.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var context = await _contextFactory.CreateDbContextAsync();
            var soldStatus = await context.TicketStatuses.FirstOrDefaultAsync(s => s.Title == "Продан");
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

            context.Tickets.Add(ticket);
            await context.SaveChangesAsync();
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

            using var context = await _contextFactory.CreateDbContextAsync();
            var ticketFromDb = await context.Tickets
                .Include(t => t.TicketStatusFkNavigation)
                .FirstOrDefaultAsync(t => t.Id == SelectedTicket.Id);

            if (ticketFromDb == null)
            {
                MessageBox.Show("Билет не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (ticketFromDb.TicketStatusFkNavigation?.Title == "Использован")
            {
                MessageBox.Show("Билет уже использован.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var usedStatus = await context.TicketStatuses.FirstOrDefaultAsync(s => s.Title == "Использован");
            if (usedStatus == null)
            {
                MessageBox.Show("Статус 'Использован' не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ticketFromDb.TicketStatusFk = usedStatus.Id;
            await context.SaveChangesAsync();

            // Обновляем локальный объект для UI
            SelectedTicket.TicketStatusFk = usedStatus.Id;
            SelectedTicket.TicketStatusFkNavigation = usedStatus;

            await LoadTicketsForExhibition();
            MessageBox.Show("Билет отмечен как использованный.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private string GenerateTicketNumber() =>
            $"T-{DateTime.Now:yyyyMMddHHmmss}-{new Random().Next(1000)}";
    }
}