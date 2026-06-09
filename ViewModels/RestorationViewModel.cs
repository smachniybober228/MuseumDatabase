using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Museum.Models;
using Museum.Repository;
using Museum.Views;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Museum.ViewModels
{
    public partial class RestorationViewModel : ObservableObject
    {
        private readonly IRepository<RestorationOrderEntity> _orderRepo;
        private readonly IRepository<Exhibit> _exhibitRepo;
        private readonly IRepository<Person> _personRepo;
        private readonly IRepository<RestorationWorkType> _workTypeRepo;
        private readonly IRepository<RequiredWorkType> _requiredWorkRepo;
        private readonly IRepository<WorkLogEntry> _workLogRepo;
        private readonly IRepository<RestorationAct> _restorationActRepo;
        private readonly IRepository<ReturnAct> _returnActRepo;
        private readonly IRepository<ExhibitStatus> _exhibitStatusRepo;

        [ObservableProperty]
        private ObservableCollection<RestorationOrderEntity> _orders;

        [ObservableProperty]
        private RestorationOrderEntity _selectedOrder;

        [ObservableProperty]
        private bool _isLoading;

        public RestorationViewModel(
            IRepository<RestorationOrderEntity> orderRepo,
            IRepository<Exhibit> exhibitRepo,
            IRepository<Person> personRepo,
            IRepository<RestorationWorkType> workTypeRepo,
            IRepository<RequiredWorkType> requiredWorkRepo,
            IRepository<WorkLogEntry> workLogRepo,
            IRepository<RestorationAct> restorationActRepo,
            IRepository<ReturnAct> returnActRepo,
            IRepository<ExhibitStatus> exhibitStatusRepo)
        {
            _orderRepo = orderRepo;
            _exhibitRepo = exhibitRepo;
            _personRepo = personRepo;
            _workTypeRepo = workTypeRepo;
            _requiredWorkRepo = requiredWorkRepo;
            _workLogRepo = workLogRepo;
            _restorationActRepo = restorationActRepo;
            _returnActRepo = returnActRepo;
            _exhibitStatusRepo = exhibitStatusRepo;
            LoadOrdersAsync();
        }

        private async Task<bool> HasRestorationActAsync(int orderId)
        {
            var acts = await _restorationActRepo.GetAllAsync();
            return acts.Any(a => a.RestorationOrderFk == orderId);
        }

        private async Task<bool> HasReturnActAsync(int orderId)
        {
            var returns = await _returnActRepo.GetAllAsync();
            return returns.Any(r => r.RestorationOrderFk == orderId);
        }

        [RelayCommand]
        private async Task LoadOrdersAsync()
        {
            IsLoading = true;
            var list = await _orderRepo.GetAllAsync();

            // Загружаем все акты и возвраты один раз
            var allActs = await _restorationActRepo.GetAllAsync();
            var allReturns = await _returnActRepo.GetAllAsync();

            foreach (var order in list)
            {
                bool hasAct = allActs.Any(a => a.RestorationOrderFk == order.Id);
                bool hasReturn = allReturns.Any(r => r.RestorationOrderFk == order.Id);

                if (!hasAct && !hasReturn)
                    order.StatusText = "В работе";
                else if (hasAct && !hasReturn)
                    order.StatusText = "Работы завершены, ожидает возврата";
                else
                    order.StatusText = "Закрыт";
            }

            Orders = new ObservableCollection<RestorationOrderEntity>(list);
            IsLoading = false;
        }

        [RelayCommand]
        private async Task AddOrderAsync()
        {
            var exhibits = await _exhibitRepo.GetAllAsync();
            // Получаем всех людей (или отфильтруйте по роли "Хранитель" через PersonRole)
            var allPersons = await _personRepo.GetAllAsync();
            var restorers = allPersons.ToList(); // для реставраторов лучше тоже фильтровать
            var initiators = allPersons.ToList(); // инициаторы – хранители, упрощённо берём всех

            var workTypes = await _workTypeRepo.GetAllAsync();

            var dialog = new RestorationOrderDialog(null, exhibits.ToList(), restorers, initiators, workTypes.ToList());
            if (dialog.ShowDialog() == true)
            {
                var newOrder = dialog.Order;
                newOrder.OrderNumber = GenerateOrderNumber();
                await _orderRepo.AddAsync(newOrder);
                await _orderRepo.SaveAsync();

                // Сохраняем требуемые виды работ
                foreach (var wt in dialog.SelectedWorkTypeIds)
                {
                    var required = new RequiredWorkType
                    {
                        RestorationOrderFk = newOrder.Id,
                        WorkTypeFk = wt
                    };
                    await _requiredWorkRepo.AddAsync(required);
                }
                await _requiredWorkRepo.SaveAsync();

                // Меняем статус экспоната на "На реставрации"
                var exhibit = await _exhibitRepo.GetByIdAsync(newOrder.ExhibitFk);
                var statuses = await _exhibitStatusRepo.GetAllAsync();
                var underRestorationStatus = statuses.FirstOrDefault(s => s.Title == "На реставрации");
                if (underRestorationStatus == null)
                {
                    MessageBox.Show("Статус 'На реставрации' не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                exhibit.ExhibitStatusFk = underRestorationStatus.Id;
                _exhibitRepo.Update(exhibit);
                await _exhibitRepo.SaveAsync();

                await LoadOrdersAsync();
                MessageBox.Show("Реставрационный заказ создан.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        [RelayCommand]
        private async Task EditOrderAsync()
        {
            if (SelectedOrder == null)
            {
                MessageBox.Show("Выберите заказ для редактирования.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (await HasRestorationActAsync(SelectedOrder.Id))
            {
                MessageBox.Show("Нельзя редактировать завершённый заказ.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var exhibits = await _exhibitRepo.GetAllAsync();
            var restorers = await _personRepo.GetAllAsync();
            var workTypes = await _workTypeRepo.GetAllAsync();
            // Загружаем текущие требуемые виды работ
            var allRequired = await _requiredWorkRepo.GetAllAsync();
            var existingWorkTypeIds = allRequired.Where(r => r.RestorationOrderFk == SelectedOrder.Id).Select(r => r.WorkTypeFk).ToList();

            var initiators = await _personRepo.GetAllAsync(); // или отфильтровать
            var dialog = new RestorationOrderDialog(SelectedOrder, exhibits.ToList(), restorers.ToList(), initiators.ToList(), workTypes.ToList(), existingWorkTypeIds);
            if (dialog.ShowDialog() == true)
            {
                // Обновляем основные поля
                SelectedOrder.ExhibitFk = dialog.Order.ExhibitFk;
                SelectedOrder.ReceiptDate = dialog.Order.ReceiptDate;
                SelectedOrder.ReasonDirection = dialog.Order.ReasonDirection;
                SelectedOrder.PlannedCompletionDate = dialog.Order.PlannedCompletionDate;
                SelectedOrder.RestorerFk = dialog.Order.RestorerFk;
                _orderRepo.Update(SelectedOrder);

                // Обновляем требуемые виды работ (удалить старые, добавить новые)
                var toDelete = allRequired.Where(r => r.RestorationOrderFk == SelectedOrder.Id).ToList();
                foreach (var del in toDelete)
                    _requiredWorkRepo.Delete(del);
                foreach (var wt in dialog.SelectedWorkTypeIds)
                {
                    var required = new RequiredWorkType
                    {
                        RestorationOrderFk = SelectedOrder.Id,
                        WorkTypeFk = wt
                    };
                    await _requiredWorkRepo.AddAsync(required);
                }
                await _requiredWorkRepo.SaveAsync();
                await _orderRepo.SaveAsync();
                await LoadOrdersAsync();
                MessageBox.Show("Заказ обновлён.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        [RelayCommand]
        private async Task DeleteOrderAsync()
        {
            if(SelectedOrder == null)
    {
                MessageBox.Show("Выберите заказ для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (await HasRestorationActAsync(SelectedOrder.Id) || await HasReturnActAsync(SelectedOrder.Id))
            {
                MessageBox.Show("Нельзя удалить заказ, по которому уже есть акты.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (MessageBox.Show($"Удалить заказ {SelectedOrder.OrderNumber}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                // Удаляем связанные записи
                var allRequired = await _requiredWorkRepo.GetAllAsync();
                var toDeleteRequired = allRequired.Where(r => r.RestorationOrderFk == SelectedOrder.Id).ToList();
                foreach (var req in toDeleteRequired)
                    _requiredWorkRepo.Delete(req);

                var allWorkLogs = await _workLogRepo.GetAllAsync();
                var toDeleteLogs = allWorkLogs.Where(w => w.RestorationOrderFk == SelectedOrder.Id).ToList();
                foreach (var log in toDeleteLogs)
                    _workLogRepo.Delete(log);

                // Также, если есть акты (хотя проверка выше должна была их отсечь), но на всякий случай:
                var allActs = await _restorationActRepo.GetAllAsync();
                var toDeleteActs = allActs.Where(a => a.RestorationOrderFk == SelectedOrder.Id).ToList();
                foreach (var act in toDeleteActs)
                    _restorationActRepo.Delete(act);

                var allReturns = await _returnActRepo.GetAllAsync();
                var toDeleteReturns = allReturns.Where(r => r.RestorationOrderFk == SelectedOrder.Id).ToList();
                foreach (var ret in toDeleteReturns)
                    _returnActRepo.Delete(ret);

                // Сохраняем удаление зависимостей
                await _requiredWorkRepo.SaveAsync();
                await _workLogRepo.SaveAsync();
                await _restorationActRepo.SaveAsync();
                await _returnActRepo.SaveAsync();

                // Теперь удаляем сам заказ
                _orderRepo.Delete(SelectedOrder);
                await _orderRepo.SaveAsync();
                await LoadOrdersAsync();
            }
        }

        [RelayCommand]
        private async Task OpenWorkLogAsync()
        {
            if (SelectedOrder == null)
            {
                MessageBox.Show("Выберите заказ для просмотра журнала работ.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var workLogs = await _workLogRepo.GetAllAsync();
            var logsForOrder = workLogs.Where(w => w.RestorationOrderFk == SelectedOrder.Id).ToList();
            var workTypes = await _workTypeRepo.GetAllAsync();

            var dialog = new WorkLogWindow(SelectedOrder, logsForOrder, workTypes.ToList(), _workLogRepo);
            if (dialog.ShowDialog() == true)
            {
                await LoadOrdersAsync();
            }
        }

        [RelayCommand]
        private async Task CompleteOrderAsync()
        {
            if (SelectedOrder == null)
            {
                MessageBox.Show("Выберите заказ для завершения.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (await HasRestorationActAsync(SelectedOrder.Id))
            {
                MessageBox.Show("Заказ уже завершён.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var dialog = new CompleteOrderDialog(SelectedOrder);
            if (dialog.ShowDialog() == true)
            {
                var act = new RestorationAct
                {
                    RestorationOrderFk = SelectedOrder.Id,
                    CompletionDate = dialog.CompletionDate,
                    FinalReport = dialog.FinalReport,
                    TotalCost = dialog.TotalCost
                };
                await _restorationActRepo.AddAsync(act);
                await _restorationActRepo.SaveAsync();
                await LoadOrdersAsync();
                MessageBox.Show("Заказ завершён.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        [RelayCommand]
        private async Task ReturnExhibitAsync()
        {
            if (SelectedOrder == null)
            {
                MessageBox.Show("Выберите заказ для возврата экспоната.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!await HasRestorationActAsync(SelectedOrder.Id))
            {
                MessageBox.Show("Сначала завершите работы по заказу.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (await HasReturnActAsync(SelectedOrder.Id))
            {
                MessageBox.Show("Экспонат уже возвращён.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Получаем возможные статусы для возврата (В запасниках, В основном фонде, В постоянной экспозиции)
            var statuses = await _exhibitStatusRepo.GetAllAsync();
            var availableStatuses = statuses.Where(s => s.Title == "В запасниках" || s.Title == "В основном фонде" || s.Title == "В постоянной экспозиции").ToList();

            var dialog = new ReturnExhibitDialog(SelectedOrder, availableStatuses);
            if (dialog.ShowDialog() == true)
            {
                var returnAct = new ReturnAct
                {
                    RestorationOrderFk = SelectedOrder.Id,
                    ReturnDate = dialog.ReturnDate
                };
                await _returnActRepo.AddAsync(returnAct);
                // Обновляем статус экспоната
                var exhibit = await _exhibitRepo.GetByIdAsync(SelectedOrder.ExhibitFk);
                exhibit.ExhibitStatusFk = dialog.SelectedStatusId;
                _exhibitRepo.Update(exhibit);
                await _exhibitRepo.SaveAsync();
                await _returnActRepo.SaveAsync();
                await LoadOrdersAsync();
                MessageBox.Show("Экспонат возвращён.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private string GenerateOrderNumber()
        {
            return $"R-{DateTime.Now:yyyyMMddHHmmss}";
        }
    }
}