using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Museum.Models;

namespace Museum.Views
{
    public partial class SellTicketDialog : Window
    {
        public List<Exhibition> Exhibitions { get; set; }
        public int SelectedExhibitionId { get; set; }
        public DateTime VisitDate { get; set; } = DateTime.Today;
        public decimal SalePrice { get; set; }

        public SellTicketDialog(List<Exhibition> exhibitions)
        {
            InitializeComponent();
            Exhibitions = exhibitions;
            DataContext = this;
        }

        private void Sell_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedExhibitionId == 0)
            {
                MessageBox.Show("Выберите выставку.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (VisitDate < DateTime.Today)
            {
                MessageBox.Show("Дата посещения не может быть раньше сегодняшнего дня.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (SalePrice <= 0)
            {
                MessageBox.Show("Введите корректную стоимость.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
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