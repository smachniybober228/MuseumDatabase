using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.IdentityModel.Tokens;
using Museum.Models;

namespace Museum.Views
{
    public partial class RestorationOrderDialog : Window
    {
        public RestorationOrderEntity Order { get; private set; }
        public List<Exhibit> Exhibits { get; private set; }
        public List<Person> Restorers { get; private set; }
        public List<Person> Initiators { get; private set; }
        public ObservableCollection<WorkTypeSelection> WorkTypes { get; private set; }
        public List<int> SelectedWorkTypeIds => WorkTypes.Where(w => w.IsSelected).Select(w => w.Id).ToList();

        public RestorationOrderDialog(RestorationOrderEntity order,
                                      List<Exhibit> exhibits,
                                      List<Person> restorers,
                                      List<Person> initiators,
                                      List<RestorationWorkType> workTypes,
                                      List<int> selectedIds = null)
        {
            InitializeComponent();
            Order = order ?? new RestorationOrderEntity { ReceiptDate = DateTime.Today, PlannedCompletionDate = DateTime.Today.AddMonths(1) };
            Exhibits = exhibits;
            Restorers = restorers;
            Initiators = initiators;
            WorkTypes = new ObservableCollection<WorkTypeSelection>(workTypes.Select(w => new WorkTypeSelection { Id = w.Id, Title = w.Title, IsSelected = selectedIds?.Contains(w.Id) ?? false }));
            DataContext = this;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (Order.ExhibitFk == 0) { MessageBox.Show("Выберите экспонат."); return; }
            if (Order.RestorerFk == 0) { MessageBox.Show("Выберите реставратора."); return; }
            if (Order.InitiatorFk == 0) { MessageBox.Show("Выберите инициатора (хранителя)."); return; }
            if (string.IsNullOrEmpty(Order.ReasonDirection)) { MessageBox.Show("Укажите причину."); return; }
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    public class WorkTypeSelection
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsSelected { get; set; }
    }
}