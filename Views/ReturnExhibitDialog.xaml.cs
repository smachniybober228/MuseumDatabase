using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Museum.Models;

namespace Museum.Views
{
    public partial class ReturnExhibitDialog : Window
    {
        public DateTime ReturnDate { get; set; }
        public List<ExhibitStatus> Statuses { get; private set; }
        public int SelectedStatusId { get; set; }

        public ReturnExhibitDialog(RestorationOrderEntity order, List<ExhibitStatus> statuses)
        {
            InitializeComponent();
            ReturnDate = DateTime.Today;
            Statuses = statuses;
            SelectedStatusId = statuses.FirstOrDefault()?.Id ?? 0;
            DataContext = this;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
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