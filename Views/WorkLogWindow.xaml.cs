using Microsoft.EntityFrameworkCore;
using Museum.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Museum.Views
{
    public partial class WorkLogWindow : Window
    {
        private readonly IDbContextFactory<MuseumDbContext> _contextFactory;
        private readonly RestorationOrderEntity _order;
        private readonly List<RestorationWorkType> _workTypes;
        private readonly ObservableCollection<WorkLogEntry> _workLogEntries;
        private readonly List<WorkLogEntry> _originalEntries;
        private readonly List<WorkLogEntry> _addedEntries = new();
        private readonly List<WorkLogEntry> _deletedEntries = new();

        public WorkLogWindow(RestorationOrderEntity order,
                             List<WorkLogEntry> entries,
                             List<RestorationWorkType> workTypes,
                             IDbContextFactory<MuseumDbContext> contextFactory)
        {
            InitializeComponent();
            _contextFactory = contextFactory;
            _order = order;
            _workTypes = workTypes;
            _originalEntries = entries.ToList();
            _workLogEntries = new ObservableCollection<WorkLogEntry>(entries);
            dgWorkLogEntries.ItemsSource = _workLogEntries;
        }

        private void AddEntry_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddWorkLogEntryDialog(_workTypes);
            if (dialog.ShowDialog() == true)
            {
                var workType = _workTypes.FirstOrDefault(wt => wt.Id == dialog.SelectedWorkType.Id);
                var newEntry = new WorkLogEntry
                {
                    RestorationOrderFk = _order.Id,
                    ExecutionDate = dialog.EntryDate,
                    WorkTypeFk = dialog.SelectedWorkType.Id,
                    WorkTypeFkNavigation = workType // для отображения в DataGrid
                };
                _workLogEntries.Add(newEntry);
                _addedEntries.Add(newEntry);
            }
        }

        private void DeleteEntry_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgWorkLogEntries.SelectedItem as WorkLogEntry;
            if (selected == null)
            {
                MessageBox.Show("Выберите запись для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageBox.Show("Удалить выбранную запись?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _workLogEntries.Remove(selected);
                if (_originalEntries.Contains(selected))
                    _deletedEntries.Add(selected);
                else
                    _addedEntries.Remove(selected);
            }
        }

        private async void SaveAndClose_Click(object sender, RoutedEventArgs e)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            // Обнуляем навигационные свойства, чтобы EF не пытался вставить связанные сущности
            foreach (var entry in _addedEntries)
            {
                entry.WorkTypeFkNavigation = null;
            }
            // Добавляем новые записи
            foreach (var entry in _addedEntries)
            {
                context.WorkLogEntries.Add(entry);
            }
            // Удаляем удалённые записи
            foreach (var entry in _deletedEntries)
            {
                context.WorkLogEntries.Remove(entry);
            }
            await context.SaveChangesAsync();

            DialogResult = true;
            Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}