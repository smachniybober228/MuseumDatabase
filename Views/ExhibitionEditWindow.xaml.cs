using Microsoft.EntityFrameworkCore;
using Museum.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Museum.Views
{
    public partial class ExhibitionEditWindow : Window, INotifyPropertyChanged
    {
        public Exhibition EditedExhibition { get; private set; }
        private readonly IDbContextFactory<MuseumDbContext> _contextFactory;

        private List<Hall> _halls;
        private List<Person> _curators;
        private List<ExhibitionStatus> _statuses;

        public List<Hall> Halls { get => _halls; set { _halls = value; OnPropertyChanged(); } }
        public List<Person> Curators { get => _curators; set { _curators = value; OnPropertyChanged(); } }
        public List<ExhibitionStatus> Statuses { get => _statuses; set { _statuses = value; OnPropertyChanged(); } }

        public ExhibitionEditWindow(Exhibition exhibition, IDbContextFactory<MuseumDbContext> contextFactory)
        {
            InitializeComponent();
            _contextFactory = contextFactory;
            EditedExhibition = exhibition ?? new Exhibition();
            Loaded += async (s, e) => await LoadDataAsync();
            DataContext = this;
        }

        private async Task LoadDataAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            Halls = await context.Halls.Include(h => h.FloorFkNavigation).ToListAsync();
            Curators = await context.People.ToListAsync(); // можно фильтровать по роли "Куратор", но для простоты все
            Statuses = await context.ExhibitionStatuses.ToListAsync();
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}