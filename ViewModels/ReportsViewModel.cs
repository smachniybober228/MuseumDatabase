using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Museum.Models;
using Museum.Repository;
using System.Collections.ObjectModel;
using System.Windows;

namespace Museum.ViewModels
{
    public partial class ReportsViewModel : ObservableObject
    {
        private readonly IDbContextFactory<MuseumDbContext> _contextFactory;

        private readonly IRepository<Ticket> _ticketRepo;
        private readonly IRepository<Exhibition> _exhibitionRepo;
        private readonly IRepository<RestorationOrderEntity> _orderRepo;
        private readonly IRepository<Exhibit> _exhibitRepo;
        private readonly IRepository<RestorationAct> _restorationActRepo;
        private readonly IRepository<ReturnAct> _returnActRepo;

        [ObservableProperty]
        private ObservableCollection<Exhibition> _exhibitions;

        [ObservableProperty]
        private Exhibition _selectedExhibitionForAttendance;

        [ObservableProperty]
        private DateTime _startDate = DateTime.Today.AddMonths(-1);

        [ObservableProperty]
        private DateTime _endDate = DateTime.Today;

        [ObservableProperty]
        private ObservableCollection<AttendanceReportRow> _attendanceReportRows;

        [ObservableProperty]
        private ObservableCollection<RestorationOrderEntity> _restorationOrders;

        [ObservableProperty]
        private ObservableCollection<Exhibit> _allExhibits;

        [ObservableProperty]
        private Exhibit _selectedExhibitForHistory;

        [ObservableProperty]
        private ObservableCollection<RestorationHistoryRow> _restorationHistoryRows;

        public ReportsViewModel(
            IRepository<Ticket> ticketRepo,
            IRepository<Exhibition> exhibitionRepo,
            IRepository<RestorationOrderEntity> orderRepo,
            IRepository<Exhibit> exhibitRepo,
            IRepository<RestorationAct> restorationActRepo,
            IRepository<ReturnAct> returnActRepo,
            IDbContextFactory<MuseumDbContext> contextFactory)
        {
            _ticketRepo = ticketRepo;
            _exhibitionRepo = exhibitionRepo;
            _orderRepo = orderRepo;
            _exhibitRepo = exhibitRepo;
            _restorationActRepo = restorationActRepo;
            _returnActRepo = returnActRepo;
            _contextFactory = contextFactory;
        }

        [RelayCommand]
        internal async Task LoadRestorationOrdersAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var orders = await context.RestorationOrderEntities
                .Include(o => o.ExhibitFkNavigation)
                .Include(o => o.RestorerFkNavigation)
                .Include(o => o.RequiredWorkTypes)
                    .ThenInclude(r => r.WorkTypeFkNavigation)
                .Where(o => !o.RestorationActs.Any()) // только активные заказы (нет акта завершения)
                .ToListAsync();

            // Вычисляем статус (опционально)
            foreach (var order in orders)
            {
                order.StatusText = "В работе";
            }

            RestorationOrders = new ObservableCollection<RestorationOrderEntity>(orders);
        }

        public async Task LoadAllExhibitsAsync()
        {
            var list = await _exhibitRepo.GetAllAsync();
            AllExhibits = new ObservableCollection<Exhibit>(list);
        }

        [RelayCommand]
        private async Task GenerateAttendanceReport()
        {
            if (StartDate > EndDate)
            {
                MessageBox.Show("Дата начала не может быть позже даты окончания.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var context = await _contextFactory.CreateDbContextAsync();
            var usedTickets = await context.Tickets
                .Include(t => t.TicketStatusFkNavigation)
                .Include(t => t.ExhibitionFkNavigation)
                .Where(t => t.TicketStatusFkNavigation.Title == "Использован" && t.VisitDate >= StartDate && t.VisitDate <= EndDate)
                .ToListAsync();

            var grouped = usedTickets
                .GroupBy(t => t.ExhibitionFkNavigation?.Title ?? "Неизвестная выставка")
                .Select(g => new AttendanceReportRow
                {
                    ExhibitionTitle = g.Key,
                    VisitorsCount = g.Count(),
                    Revenue = g.Sum(t => t.SalePrice)
                }).ToList();

            AttendanceReportRows = new ObservableCollection<AttendanceReportRow>(grouped);
        }

        [RelayCommand]
        private async Task LoadRestorationHistory()
        {
            if (SelectedExhibitForHistory == null)
            {
                MessageBox.Show("Выберите экспонат.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var allOrders = await _orderRepo.GetAllAsync();
            var ordersForExhibit = allOrders.Where(o => o.ExhibitFk == SelectedExhibitForHistory.Id).ToList();

            var allActs = await _restorationActRepo.GetAllAsync();
            var allReturns = await _returnActRepo.GetAllAsync();

            var history = new List<RestorationHistoryRow>();
            foreach (var order in ordersForExhibit)
            {
                var act = allActs.FirstOrDefault(a => a.RestorationOrderFk == order.Id);
                var returnAct = allReturns.FirstOrDefault(r => r.RestorationOrderFk == order.Id);
                history.Add(new RestorationHistoryRow
                {
                    OrderNumber = order.OrderNumber,
                    ReceiptDate = order.ReceiptDate,
                    PlannedCompletionDate = order.PlannedCompletionDate,
                    CompletionDate = act?.CompletionDate,
                    ReturnDate = returnAct?.ReturnDate,
                    Status = (act == null && returnAct == null) ? "В работе" :
                             (act != null && returnAct == null) ? "Работы завершены, ожидает возврата" : "Закрыт"
                });
            }

            RestorationHistoryRows = new ObservableCollection<RestorationHistoryRow>(history);
        }

        [RelayCommand]
        public async Task LoadExhibitionsAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var list = await context.Exhibitions.ToListAsync();
            Exhibitions = new ObservableCollection<Exhibition>(list);
        }
    }

    public class AttendanceReportRow
    {
        public string ExhibitionTitle { get; set; }
        public int VisitorsCount { get; set; }
        public double Revenue { get; set; }
    }

    public class RestorationHistoryRow
    {
        public string OrderNumber { get; set; }
        public DateTime ReceiptDate { get; set; }
        public DateTime PlannedCompletionDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Status { get; set; }
    }
}