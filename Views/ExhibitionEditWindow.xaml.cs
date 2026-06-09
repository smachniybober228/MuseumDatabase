using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Museum.Models;

namespace Museum.Views
{
    public partial class ExhibitionEditWindow : Window
    {
        public Exhibition EditedExhibition { get; private set; }
        public List<Hall> Halls { get; private set; }
        public List<Person> Curators { get; private set; }
        public List<ExhibitionStatus> Statuses { get; private set; }

        public ExhibitionEditWindow(Exhibition exhibition, IEnumerable<Hall> halls, IEnumerable<Person> curators, IEnumerable<ExhibitionStatus> statuses)
        {
            InitializeComponent();
            EditedExhibition = exhibition;
            Halls = halls.ToList();
            Curators = curators.ToList();
            Statuses = statuses.ToList();
            DataContext = this;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EditedExhibition.Title))
            {
                MessageBox.Show("Введите название выставки.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
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