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
        private ObservableCollection<Exhibit> exhibits;

        [ObservableProperty]
        private Exhibit selectedExhibit;

        [ObservableProperty]
        private bool isLoading;

        public ExhibitsViewModel(IRepository<Exhibit> exhibitRepository)
        {
            _exhibitRepository = exhibitRepository;
            // Загружаем данные при создании ViewModel
            Task.Run(async () => await LoadExhibitsAsync());
        }

        [RelayCommand]
        private async Task LoadExhibitsAsync()
        {
            isLoading = true;
            var list = await _exhibitRepository.GetAllAsync();
            Exhibits = new ObservableCollection<Exhibit>(list);
            isLoading = false;
        }

        [RelayCommand]
        private async Task AddExhibitAsync()
        {
            // Открыть диалог создания нового экспоната
            // var dialogVM = new ExhibitDialogViewModel(null, null, null); // упрощённо, нужны репозитории
            // ... логика показа диалога
            // После сохранения: await LoadExhibitsAsync();
        }

        [RelayCommand]
        private async Task EditExhibitAsync()
        {
            if (SelectedExhibit == null)
            {
                MessageBox.Show("Выберите экспонат для редактирования.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Открыть диалог редактирования SelectedExhibit
            // После сохранения: await LoadExhibitsAsync();
        }

        [RelayCommand]
        private async Task DeleteExhibitAsync()
        {
            if (SelectedExhibit == null)
            {
                MessageBox.Show("Выберите экспонат для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить экспонат '{SelectedExhibit.Title}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _exhibitRepository.Delete(SelectedExhibit);
                await _exhibitRepository.SaveAsync();
                await LoadExhibitsAsync();
            }
        }
    }
}
