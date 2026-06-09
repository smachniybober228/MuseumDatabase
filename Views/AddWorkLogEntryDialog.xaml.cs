using System;
using System.Collections.Generic;
using System.Windows;
using Museum.Models;

namespace Museum.Views
{
    public partial class AddWorkLogEntryDialog : Window
    {
        public DateTime EntryDate { get; set; } = DateTime.Today;
        public RestorationWorkType SelectedWorkType { get; set; }
        public List<RestorationWorkType> WorkTypes { get; }

        public AddWorkLogEntryDialog(List<RestorationWorkType> workTypes)
        {
            InitializeComponent();
            WorkTypes = workTypes;
            DataContext = this;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWorkType == null)
            {
                MessageBox.Show("Выберите вид работы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}