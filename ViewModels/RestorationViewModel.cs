using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Museum.Models;
using Museum.Views;
using System.Collections.ObjectModel;
using System.Windows;

namespace Museum.ViewModels
{
    public partial class RestorationViewModel : ObservableObject
    {
        private readonly IDbContextFactory<MuseumDbContext> _contextFactory;

        [ObservableProperty]
        private ObservableCollection<RestorationOrderEntity> _orders;

        [ObservableProperty]
        private RestorationOrderEntity _selectedOrder;

        [ObservableProperty]
        private bool _isLoading;

        public RestorationViewModel(IDbContextFactory<MuseumDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
            LoadOrdersAsync();
        }

        [RelayCommand]
        private async Task LoadOrdersAsync()
        {
            IsLoading = true;
            using var context = await _contextFactory.CreateDbContextAsync();
            var orders = await context.RestorationOrderEntities
                .Include(o => o.ExhibitFkNavigation)
                .Include(o => o.RestorerFkNavigation)
                .Include(o => o.InitiatorFkNavigation)
                .Include(o => o.RequiredWorkTypes)
                    .ThenInclude(r => r.WorkTypeFkNavigation)
                .Include(o => o.RestorationActs)
                .Include(o => o.ReturnActs)
                .ToListAsync();

            // Вычисляем статус для каждого заказа
            foreach (var order in orders)
            {
                bool hasAct = order.RestorationActs.Any();
                bool hasReturn = order.ReturnActs.Any();
                order.StatusText = (!hasAct && !hasReturn) ? "В работе" :
                                   (hasAct && !hasReturn) ? "Работы завершены, ожидает возврата" : "Закрыт";
            }

            Orders = new ObservableCollection<RestorationOrderEntity>(orders);
            IsLoading = false;
        }

        [RelayCommand]
        private async Task AddOrderAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var exhibits = await context.Exhibits.ToListAsync();
            var allPersons = await context.People.ToListAsync();
            // Для простоты используем всех людей как реставраторов и инициаторов.
            // В реальном приложении лучше фильтровать по ролям.
            var restorers = allPersons;
            var initiators = allPersons;
            var workTypes = await context.RestorationWorkTypes.ToListAsync();

            var dialog = new RestorationOrderDialog(null, exhibits, restorers, initiators, workTypes);
            if (dialog.ShowDialog() == true)
            {
                var newOrder = dialog.Order;
                newOrder.OrderNumber = GenerateOrderNumber();
                context.RestorationOrderEntities.Add(newOrder);
                await context.SaveChangesAsync();

                // Сохраняем требуемые виды работ
                foreach (var workTypeId in dialog.SelectedWorkTypeIds)
                {
                    context.RequiredWorkTypes.Add(new RequiredWorkType
                    {
                        RestorationOrderFk = newOrder.Id,
                        WorkTypeFk = workTypeId
                    });
                }

                // Меняем статус экспоната на "На реставрации"
                var exhibit = await context.Exhibits.FindAsync(newOrder.ExhibitFk);
                var underRestorationStatus = await context.ExhibitStatuses
                    .FirstOrDefaultAsync(s => s.Title == "На реставрации");
                if (underRestorationStatus != null)
                    exhibit.ExhibitStatusFk = underRestorationStatus.Id;

                await context.SaveChangesAsync();
                await LoadOrdersAsync();
                MessageBox.Show("Реставрационный заказ создан.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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

            using var context = await _contextFactory.CreateDbContextAsync();

            // Загружаем заказ из БД с зависимостями (для отслеживания)
            var orderToEdit = await context.RestorationOrderEntities
                .Include(o => o.RequiredWorkTypes)
                .FirstOrDefaultAsync(o => o.Id == SelectedOrder.Id);

            if (orderToEdit == null) return;

            // Проверяем, можно ли редактировать (заказ не завершён)
            var hasAct = await context.RestorationActs.AnyAsync(a => a.RestorationOrderFk == orderToEdit.Id);
            if (hasAct)
            {
                MessageBox.Show("Нельзя редактировать завершённый заказ.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Загружаем данные для диалога
            var exhibits = await context.Exhibits.ToListAsync();
            var allPersons = await context.People.ToListAsync();
            var workTypes = await context.RestorationWorkTypes.ToListAsync();
            var existingWorkTypeIds = orderToEdit.RequiredWorkTypes.Select(r => r.WorkTypeFk).ToList();

            var dialog = new RestorationOrderDialog(orderToEdit, exhibits, allPersons, allPersons, workTypes, existingWorkTypeIds);
            if (dialog.ShowDialog() == true)
            {
                // Обновляем поля заказа
                orderToEdit.ExhibitFk = dialog.Order.ExhibitFk;
                orderToEdit.ReceiptDate = dialog.Order.ReceiptDate;
                orderToEdit.ReasonDirection = dialog.Order.ReasonDirection;
                orderToEdit.PlannedCompletionDate = dialog.Order.PlannedCompletionDate;
                orderToEdit.RestorerFk = dialog.Order.RestorerFk;

                // Обновляем требуемые виды работ: удаляем старые, добавляем новые
                context.RequiredWorkTypes.RemoveRange(orderToEdit.RequiredWorkTypes);
                orderToEdit.RequiredWorkTypes.Clear();
                foreach (var workTypeId in dialog.SelectedWorkTypeIds)
                {
                    orderToEdit.RequiredWorkTypes.Add(new RequiredWorkType
                    {
                        RestorationOrderFk = orderToEdit.Id,
                        WorkTypeFk = workTypeId
                    });
                }

                await context.SaveChangesAsync();

                // Перезагружаем список заказов, чтобы обновить UI
                await LoadOrdersAsync();
                MessageBox.Show("Заказ обновлён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        [RelayCommand]
        private async Task DeleteOrderAsync()
        {
            if (SelectedOrder == null)
            {
                MessageBox.Show("Выберите заказ для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var context = await _contextFactory.CreateDbContextAsync();
            // Загружаем заказ со всеми зависимостями
            var order = await context.RestorationOrderEntities
                .Include(o => o.RequiredWorkTypes)
                .Include(o => o.WorkLogEntries)
                .Include(o => o.RestorationActs)
                .Include(o => o.ReturnActs)
                .FirstOrDefaultAsync(o => o.Id == SelectedOrder.Id);

            if (order == null) return;

            // Нельзя удалять, если есть акты завершения или возврата
            if (order.RestorationActs.Any() || order.ReturnActs.Any())
            {
                MessageBox.Show("Нельзя удалить заказ, по которому уже есть акты.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (MessageBox.Show($"Удалить заказ {order.OrderNumber}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                // Удаляем все зависимые записи
                context.RequiredWorkTypes.RemoveRange(order.RequiredWorkTypes);
                context.WorkLogEntries.RemoveRange(order.WorkLogEntries);
                await context.SaveChangesAsync(); // Сохраняем удаление зависимостей

                // Теперь удаляем сам заказ
                context.RestorationOrderEntities.Remove(order);
                await context.SaveChangesAsync();

                await LoadOrdersAsync();
                MessageBox.Show("Заказ удалён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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

            using var context = await _contextFactory.CreateDbContextAsync();
            var logs = await context.WorkLogEntries
                .Include(w => w.WorkTypeFkNavigation)
                .Where(w => w.RestorationOrderFk == SelectedOrder.Id)
                .ToListAsync();
            var workTypes = await context.RestorationWorkTypes.ToListAsync();

            var dialog = new WorkLogWindow(SelectedOrder, logs, workTypes, _contextFactory);
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

            using var context = await _contextFactory.CreateDbContextAsync();
            var hasAct = await context.RestorationActs.AnyAsync(a => a.RestorationOrderFk == SelectedOrder.Id);
            if (hasAct)
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
                context.RestorationActs.Add(act);
                await context.SaveChangesAsync();
                await LoadOrdersAsync();
                MessageBox.Show("Заказ завершён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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

            using var context = await _contextFactory.CreateDbContextAsync();
            var hasAct = await context.RestorationActs.AnyAsync(a => a.RestorationOrderFk == SelectedOrder.Id);
            if (!hasAct)
            {
                MessageBox.Show("Сначала завершите работы по заказу.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var hasReturn = await context.ReturnActs.AnyAsync(r => r.RestorationOrderFk == SelectedOrder.Id);
            if (hasReturn)
            {
                MessageBox.Show("Экспонат уже возвращён.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var statuses = await context.ExhibitStatuses
                .Where(s => s.Title == "В запасниках" || s.Title == "В основном фонде" || s.Title == "В постоянной экспозиции")
                .ToListAsync();

            var dialog = new ReturnExhibitDialog(SelectedOrder, statuses);
            if (dialog.ShowDialog() == true)
            {
                var returnAct = new ReturnAct
                {
                    RestorationOrderFk = SelectedOrder.Id,
                    ReturnDate = dialog.ReturnDate
                };
                context.ReturnActs.Add(returnAct);

                var exhibit = await context.Exhibits.FindAsync(SelectedOrder.ExhibitFk);
                if (exhibit != null)
                    exhibit.ExhibitStatusFk = dialog.SelectedStatusId;

                await context.SaveChangesAsync();
                await LoadOrdersAsync();
                MessageBox.Show("Экспонат возвращён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private string GenerateOrderNumber() =>
            $"R-{DateTime.Now:yyyyMMddHHmmss}";
    }
}