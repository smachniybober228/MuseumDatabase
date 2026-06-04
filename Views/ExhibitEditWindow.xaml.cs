using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Museum.Models;
using Museum.Repository;

namespace Museum.Views
{
    public partial class ExhibitEditWindow : Window
    {
        public Exhibit EditedExhibit { get; private set; }

        private readonly IRepository<ExhibitStatus> _statusRepo;
        private readonly IRepository<ReceiptAct> _receiptActRepo;
        private ObservableCollection<ExhibitStatus> _statuses;
        private ObservableCollection<ReceiptAct> _receiptActs;

        public ExhibitEditWindow(Exhibit exhibit,
                                 IRepository<ExhibitStatus> statusRepo,
                                 IRepository<ReceiptAct> receiptActRepo)
        {
            InitializeComponent();
            EditedExhibit = exhibit;
            _statusRepo = statusRepo;
            _receiptActRepo = receiptActRepo;
            Loaded += async (s, e) =>
            {
                // Небольшая задержка, чтобы UI успел отрисоваться
                await Task.Delay(10);
                await LoadDataAsync();
            };
            DataContext = this;
        }

        private async Task LoadDataAsync()
        {
            _statuses = new ObservableCollection<ExhibitStatus>(await _statusRepo.GetAllAsync());
            _receiptActs = new ObservableCollection<ReceiptAct>(await _receiptActRepo.GetAllAsync());

            if (cmbStatus != null)
            {
                cmbStatus.ItemsSource = _statuses;
                cmbStatus.SelectedItem = _statuses.FirstOrDefault(s => s.Id == EditedExhibit.ExhibitStatusFk);
            }

            if (cmbReceiptAct != null)
            {
                cmbReceiptAct.ItemsSource = _receiptActs;
                cmbReceiptAct.SelectedItem = _receiptActs.FirstOrDefault(r => r.Id == EditedExhibit.ReceiptActFk);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (cmbStatus.SelectedItem is ExhibitStatus selectedStatus)
                EditedExhibit.ExhibitStatusFk = selectedStatus.Id;
            if (cmbReceiptAct.SelectedItem is ReceiptAct selectedAct)
                EditedExhibit.ReceiptActFk = selectedAct.Id;

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // Поля для привязки в XAML
        public Exhibit ExhibitData => EditedExhibit;
        public ObservableCollection<ExhibitStatus> Statuses => _statuses;
        public ObservableCollection<ReceiptAct> ReceiptActs => _receiptActs;
    }
}