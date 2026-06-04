using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Museum.Models;
using Museum.Repository;
using Museum.Views;
using System.Collections.ObjectModel;
using System.Windows;

namespace Museum.ViewModels
{
    public partial class ExhibitsViewModel : ObservableObject
    {
        private readonly IRepository<Exhibit> _exhibitRepository;
        private readonly IRepository<ExhibitStatus> _statusRepository;
        private readonly IRepository<ReceiptAct> _receiptActRepository;

        [ObservableProperty]
        private ObservableCollection<Exhibit> _exhibits;

        [ObservableProperty]
        private Exhibit _selectedExhibit;

        [ObservableProperty]
        private bool _isLoading;

        public ExhibitsViewModel(IRepository<Exhibit> exhibitRepository,
                                 IRepository<ExhibitStatus> statusRepository,
                                 IRepository<ReceiptAct> receiptActRepository)
        {
            _exhibitRepository = exhibitRepository;
            _statusRepository = statusRepository;
            _receiptActRepository = receiptActRepository;
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
            // Создаём копию для редактирования, чтобы не портить оригинал при отмене
            var copy = new Exhibit
            {
                Id = SelectedExhibit.Id,
                InventoryNumber = SelectedExhibit.InventoryNumber,
                Title = SelectedExhibit.Title,
                LengthCm = SelectedExhibit.LengthCm,
                WidthCm = SelectedExhibit.WidthCm,
                HeightCm = SelectedExhibit.HeightCm,
                CreationDate = SelectedExhibit.CreationDate,
                Author = SelectedExhibit.Author,
                OriginPlace = SelectedExhibit.OriginPlace,
                ExhibitStatusFk = SelectedExhibit.ExhibitStatusFk,
                ReceiptActFk = SelectedExhibit.ReceiptActFk
            };

            var dialog = new ExhibitEditWindow(copy, _statusRepository, _receiptActRepository);
            if (dialog.ShowDialog() == true)
            {
                // Копируем изменённые данные обратно в оригинал
                SelectedExhibit.InventoryNumber = copy.InventoryNumber;
                SelectedExhibit.Title = copy.Title;
                SelectedExhibit.LengthCm = copy.LengthCm;
                SelectedExhibit.WidthCm = copy.WidthCm;
                SelectedExhibit.HeightCm = copy.HeightCm;
                SelectedExhibit.CreationDate = copy.CreationDate;
                SelectedExhibit.Author = copy.Author;
                SelectedExhibit.OriginPlace = copy.OriginPlace;
                SelectedExhibit.ExhibitStatusFk = copy.ExhibitStatusFk;
                SelectedExhibit.ReceiptActFk = copy.ReceiptActFk;

                _exhibitRepository.Update(SelectedExhibit);
                await _exhibitRepository.SaveAsync();
                await LoadExhibitsAsync();
            }
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