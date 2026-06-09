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
        private readonly IRepository<Exhibition> _exhibitionRepo;
        private readonly IRepository<Ticket> _ticketRepo;
        private readonly IRepository<RestorationOrderEntity> _orderRepo;
        private readonly IRepository<RestorationAct> _actRepo;
        private readonly IRepository<ReturnAct> _returnRepo;
        private readonly IRepository<Exhibit> _exhibitRepo;
        private readonly IRepository<WorkLogEntry> _workLogRepo;
        private readonly IRepository<RestorationWorkType> _workTypeRepo;

        [ObservableProperty]
        private ObservableCollection<Exhibition> _exhibitions;

        [ObservableProperty]
        private Exhibition _selectedExhibitionForDaily;

        [ObservableProperty]
        private DateTime _reportStartDate = DateTime.Today.AddMonths(-1);

        [ObservableProperty]
        private DateTime _reportEndDate = DateTime.Today;

        [ObservableProperty]
        private ObservableCollection<ExhibitionAttendanceDto> _attendanceReport;

        [ObservableProperty]
        private ObservableCollection<DailyAttendanceDto> _dailyAttendanceReport;

        [ObservableProperty]
        private ObservableCollection<RestorationExhibitDto> _restorationExhibits;

        [ObservableProperty]
        private ObservableCollection<Exhibit> _allExhibits;

        [ObservableProperty]
        private Exhibit _selectedExhibitForHistory;

        [ObservableProperty]
        private ObservableCollection<RestorationHistoryDto> _restorationHistory;

        public ReportsViewModel(
            IRepository<Exhibition> exhibitionRepo,
            IRepository<Ticket> ticketRepo,
            IRepository<RestorationOrderEntity> orderRepo,
            IRepository<RestorationAct> actRepo,
            IRepository<ReturnAct> returnRepo,
            IRepository<Exhibit> exhibitRepo,
            IRepository<WorkLogEntry> workLogRepo,
            IRepository<RestorationWorkType> workTypeRepo)
        {
            _exhibitionRepo = exhibitionRepo;
            _ticketRepo = ticketRepo;
            _orderRepo = orderRepo;
            _actRepo = actRepo;
            _returnRepo = returnRepo;
            _exhibitRepo = exhibitRepo;
            _workLogRepo = workLogRepo;
            _workTypeRepo = workTypeRepo;

            LoadExhibitionsAsync();
            LoadAllExhibitsAsync();
        }

        private async Task LoadExhibitionsAsync()
        {
            var list = await _exhibitionRepo.GetAllAsync();
            Exhibitions = new ObservableCollection<Exhibition>(list);
        }

        private async Task LoadAllExhibitsAsync()
        {
            var list = await _exhibitRepo.GetAllAsync();
            AllExhibits = new ObservableCollection<Exhibit>(list);
        }

        [RelayCommand]
        private async Task GenerateAttendanceReport()
        {
            var allTickets = await _ticketRepo.GetAllAsync();
            var usedTickets = allTickets.Where(t => t.VisitDate >= ReportStartDate && t.VisitDate <= ReportEndDate
                                                 && t.TicketStatusFkNavigation?.Title == "Использован");
            var query = usedTickets.GroupBy(t => t.ExhibitionFkNavigation.Title)
                                   .Select(g => new ExhibitionAttendanceDto
                                   {
                                       ExhibitionTitle = g.Key,
                                       VisitorsCount = g.Count(),
                                       Revenue = g.Sum(x => x.SalePrice)
                                   });
            AttendanceReport = new ObservableCollection<ExhibitionAttendanceDto>(query);
        }

        [RelayCommand]
        private async Task GenerateDailyAttendance()
        {
            if (SelectedExhibitionForDaily == null)
            {
                MessageBox.Show("Выберите выставку.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var allTickets = await _ticketRepo.GetAllAsync();
            var tickets = allTickets.Where(t => t.ExhibitionFk == SelectedExhibitionForDaily.Id
                                             && t.VisitDate >= ReportStartDate && t.VisitDate <= ReportEndDate
                                             && t.TicketStatusFkNavigation?.Title == "Использован");
            var query = tickets.GroupBy(t => t.VisitDate)
                               .Select(g => new DailyAttendanceDto
                               {
                                   VisitDate = g.Key,
                                   VisitorsCount = g.Count()
                               })
                               .OrderBy(d => d.VisitDate);
            DailyAttendanceReport = new ObservableCollection<DailyAttendanceDto>(query);
        }

        [RelayCommand]
        private async Task GenerateRestorationExhibitsReport()
        {
            var allOrders = await _orderRepo.GetAllAsync();
            var allActs = await _actRepo.GetAllAsync();
            var allReturns = await _returnRepo.GetAllAsync();
            var allExhibits = await _exhibitRepo.GetAllAsync();

            var result = new List<RestorationExhibitDto>();
            foreach (var order in allOrders)
            {
                bool hasAct = allActs.Any(a => a.RestorationOrderFk == order.Id);
                bool hasReturn = allReturns.Any(r => r.RestorationOrderFk == order.Id);
                if (!hasReturn) // только те, которые ещё не возвращены (на реставрации или ожидают возврата)
                {
                    var exhibit = allExhibits.FirstOrDefault(e => e.Id == order.ExhibitFk);
                    var restorer = (await _exhibitRepo.GetAllAsync()).FirstOrDefault(); // заглушка, надо загружать отдельно
                    result.Add(new RestorationExhibitDto
                    {
                        ExhibitTitle = exhibit?.Title ?? "?",
                        OrderNumber = order.OrderNumber,
                        ReceiptDate = order.ReceiptDate,
                        PlannedCompletionDate = order.PlannedCompletionDate,
                        RestorerName = "?", // нужно подтянуть имя реставратора отдельно
                        Status = (!hasAct && !hasReturn) ? "В работе" : "Работы завершены, ожидает возврата"
                    });
                }
            }
            RestorationExhibits = new ObservableCollection<RestorationExhibitDto>(result);
        }

        [RelayCommand]
        private async Task GenerateRestorationHistory()
        {
            if (SelectedExhibitForHistory == null)
            {
                MessageBox.Show("Выберите экспонат.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var orders = await _orderRepo.GetAllAsync();
            var ordersForExhibit = orders.Where(o => o.ExhibitFk == SelectedExhibitForHistory.Id).ToList();
            var acts = await _actRepo.GetAllAsync();
            var returns = await _returnRepo.GetAllAsync();
            var workLogs = await _workLogRepo.GetAllAsync();
            var workTypes = await _workTypeRepo.GetAllAsync();

            var history = new List<RestorationHistoryDto>();
            foreach (var order in ordersForExhibit)
            {
                var act = acts.FirstOrDefault(a => a.RestorationOrderFk == order.Id);
                var returnAct = returns.FirstOrDefault(r => r.RestorationOrderFk == order.Id);
                var logs = workLogs.Where(w => w.RestorationOrderFk == order.Id).ToList();
                var descriptions = logs.Select(l => $"{l.ExecutionDate:dd.MM.yyyy} - {l.WorkTypeFkNavigation?.Title ?? l.WorkTypeFk.ToString()}").ToList();
                history.Add(new RestorationHistoryDto
                {
                    OrderNumber = order.OrderNumber,
                    ReceiptDate = order.ReceiptDate,
                    CompletionDate = act?.CompletionDate,
                    ReturnDate = returnAct?.ReturnDate,
                    FinalReport = act?.FinalReport,
                    TotalCost = act?.TotalCost ?? 0,
                    WorkLogDescriptions = descriptions
                });
            }
            RestorationHistory = new ObservableCollection<RestorationHistoryDto>(history);
        }
    }
}