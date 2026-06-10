using Microsoft.EntityFrameworkCore;
using Museum.Models;
using Museum.Repository;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Museum.Views
{
    public partial class ExhibitEditWindow : Window
    {
        public Exhibit EditedExhibit { get; private set; }
        private readonly IDbContextFactory<MuseumDbContext> _contextFactory;
        private List<ExhibitStatus> _statuses;
        private List<ReceiptAct> _receiptActs;


        public ExhibitEditWindow(Exhibit exhibit,
                                 IDbContextFactory<MuseumDbContext> contextFactory)
        {
            InitializeComponent();
            _contextFactory = contextFactory;
            EditedExhibit = exhibit ?? new Exhibit();
            Loaded += async (s, e) => await LoadDataAsync();
            DataContext = this;
        }

        private async Task LoadDataAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            _statuses = await context.ExhibitStatuses.ToListAsync();
            _receiptActs = await context.ReceiptActs.ToListAsync();

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
        public List<ExhibitStatus> Statuses => _statuses;
        public List<ReceiptAct> ReceiptActs => _receiptActs;
    }
}