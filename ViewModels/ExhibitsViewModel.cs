using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Museum.Models;
using Museum.Repository;
using System.Collections.ObjectModel;
using System.Windows;

namespace Museum.ViewModels
{
    public partial class ExhibitsViewModel : ObservableObject
    {
        private readonly IRepository<Exhibit> _exhibitRepository;

        [ObservableProperty]
        private ObservableCollection<Exhibit> _exhibits;

        [ObservableProperty]
        private Exhibit _selectedExhibit;

        [ObservableProperty]
        private bool _isLoading;

        public ExhibitsViewModel(IRepository<Exhibit> exhibitRepository)
        {
            _exhibitRepository = exhibitRepository;
            LoadExhibitsAsync();
        }

        [RelayCommand]
        private async Task LoadExhibitsAsync()
        {
            IsLoading = true;
            var list = await _exhibitRepository.GetAllAsync();
            Exhibits = new ObservableCollection<Exhibit>(list);
            IsLoading = false;
        }

        [RelayCommand]
        private async Task AddExhibitAsync()
        {
            // ... 
        }

        [RelayCommand]
        private async Task EditExhibitAsync()
        {
            if (SelectedExhibit == null)
            {
                MessageBox.Show("Выберите экспонат для редактирования.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // ...
        }

        [RelayCommand]
        private async Task DeleteExhibitAsync()
        {
            if (SelectedExhibit == null)
            {
                MessageBox.Show("Выберите экспонат для удаления.", "Предупреждение");
                return;
            }
            if (MessageBox.Show($"Удалить экспонат '{SelectedExhibit.Title}'? Все связанные данные (фото, категории, заказы и т.д.) будут удалены автоматически.",
                        "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _exhibitRepository.Delete(SelectedExhibit);
                await _exhibitRepository.SaveAsync();
                await LoadExhibitsAsync();
            }
        }
    }
}