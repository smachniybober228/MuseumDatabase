using Museum.Models;
using System;
using System.Windows;

namespace Museum.Views
{
    public partial class CompleteOrderDialog : Window
    {
        public DateTime CompletionDate { get; set; }
        public string FinalReport { get; set; }
        public double TotalCost { get; set; }

        public CompleteOrderDialog(RestorationOrderEntity order)
        {
            InitializeComponent();
            CompletionDate = DateTime.Today;
            DataContext = this;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FinalReport))
            {
                MessageBox.Show("Заполните итоговый отчёт.");
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