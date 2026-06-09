using Museum.Models;
using Museum.Repository;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Museum.Views
{
    public partial class WorkLogWindow : Window
    {
        private readonly RestorationOrderEntity _order;
        private readonly List<RestorationWorkType> _workTypes;
        private readonly IRepository<WorkLogEntry> _workLogRepo;
        private readonly ObservableCollection<WorkLogEntry> _workLogEntries;
        private readonly List<WorkLogEntry> _originalEntries;
        private readonly List<WorkLogEntry> _addedEntries = new();
        private readonly List<WorkLogEntry> _deletedEntries = new();

        public WorkLogWindow(RestorationOrderEntity order,
                             List<WorkLogEntry> entries,
                             List<RestorationWorkType> workTypes,
                             IRepository<WorkLogEntry> workLogRepo)
        {
            InitializeComponent();
            _order = order;
            _workTypes = workTypes;
            _workLogRepo = workLogRepo;
            _originalEntries = entries.ToList();
            _workLogEntries = new ObservableCollection<WorkLogEntry>(entries);
            dgWorkLogEntries.ItemsSource = _workLogEntries;
        }

        private void AddEntry_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddWorkLogEntryDialog(_workTypes);
            if (dialog.ShowDialog() == true)
            {
                var newEntry = new WorkLogEntry
                {
                    RestorationOrderFk = _order.Id,
                    ExecutionDate = dialog.EntryDate,
                    WorkTypeFk = dialog.SelectedWorkType.Id
                };
                // Устанавливаем навигационное свойство для отображения
                newEntry.WorkTypeFkNavigation = dialog.SelectedWorkType;
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
            // Сохраняем добавленные записи
            foreach (var entry in _addedEntries)
                await _workLogRepo.AddAsync(entry);
            // Удаляем удалённые записи
            foreach (var entry in _deletedEntries)
                _workLogRepo.Delete(entry);
            await _workLogRepo.SaveAsync();

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