using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Museum.Models;
using Museum.Repository;
using Museum.Views;
using System.Collections.ObjectModel;
using System.Windows;

namespace Museum.ViewModels
{
    public partial class ExhibitionsViewModel : ObservableObject
    {
        private readonly IRepository<Exhibition> _exhibitionRepo;
        private readonly IRepository<Hall> _hallRepo;
        private readonly IRepository<Person> _personRepo;
        private readonly IRepository<ExhibitionStatus> _statusRepo;
        private readonly IRepository<Exhibit> _exhibitRepo;
        private readonly IRepository<ExpositionPlaceType> _placeTypeRepo;
        private readonly IRepository<ExhibitOnExhibition> _exhibitOnExhibitionRepo;

        [ObservableProperty]
        private ObservableCollection<Exhibition> _exhibitions;

        [ObservableProperty]
        private Exhibition _selectedExhibition;

        [ObservableProperty]
        private bool _isLoading;

        public ExhibitionsViewModel(
            IRepository<Exhibition> exhibitionRepo,
            IRepository<Hall> hallRepo,
            IRepository<Person> personRepo,
            IRepository<ExhibitionStatus> statusRepo,
            IRepository<Exhibit> exhibitRepo,
            IRepository<ExpositionPlaceType> placeTypeRepo,
            IRepository<ExhibitOnExhibition> exhibitOnExhibitionRepo)
        {
            _exhibitionRepo = exhibitionRepo;
            _hallRepo = hallRepo;
            _personRepo = personRepo;
            _statusRepo = statusRepo;
            _exhibitRepo = exhibitRepo;
            _placeTypeRepo = placeTypeRepo;
            _exhibitOnExhibitionRepo = exhibitOnExhibitionRepo;

            LoadExhibitionsAsync();
        }

        [RelayCommand]
        private async Task LoadExhibitionsAsync()
        {
            IsLoading = true;
            var list = await _exhibitionRepo.GetAllAsync();
            Exhibitions = new ObservableCollection<Exhibition>(list);
            IsLoading = false;
        }

        [RelayCommand]
        private async Task AddExhibitionAsync()
        {
            var newExhibition = new Exhibition
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(1)
            };
            var halls = await _hallRepo.GetAllAsync();
            var curators = await _personRepo.GetAllAsync();
            var statuses = await _statusRepo.GetAllAsync();

            var dialog = new ExhibitionEditWindow(newExhibition, halls, curators, statuses);
            if (dialog.ShowDialog() == true)
            {
                await _exhibitionRepo.AddAsync(dialog.EditedExhibition);
                await _exhibitionRepo.SaveAsync();
                await LoadExhibitionsAsync();
            }
        }

        [RelayCommand]
        private async Task EditExhibitionAsync()
        {
            if (SelectedExhibition == null)
            {
                MessageBox.Show("Выберите выставку для редактирования.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var copy = new Exhibition
            {
                Id = SelectedExhibition.Id,
                Title = SelectedExhibition.Title,
                StartDate = SelectedExhibition.StartDate,
                EndDate = SelectedExhibition.EndDate,
                HallFk = SelectedExhibition.HallFk,
                ResponsibleCuratorFk = SelectedExhibition.ResponsibleCuratorFk,
                ExhibitionStatusFk = SelectedExhibition.ExhibitionStatusFk
            };

            var halls = await _hallRepo.GetAllAsync();
            var curators = await _personRepo.GetAllAsync();
            var statuses = await _statusRepo.GetAllAsync();

            var dialog = new ExhibitionEditWindow(copy, halls, curators, statuses);
            if (dialog.ShowDialog() == true)
            {
                SelectedExhibition.Title = copy.Title;
                SelectedExhibition.StartDate = copy.StartDate;
                SelectedExhibition.EndDate = copy.EndDate;
                SelectedExhibition.HallFk = copy.HallFk;
                SelectedExhibition.ResponsibleCuratorFk = copy.ResponsibleCuratorFk;
                SelectedExhibition.ExhibitionStatusFk = copy.ExhibitionStatusFk;

                _exhibitionRepo.Update(SelectedExhibition);
                await _exhibitionRepo.SaveAsync();
                await LoadExhibitionsAsync();
            }
        }

        [RelayCommand]
        private async Task DeleteExhibitionAsync()
        {
            if (SelectedExhibition == null)
            {
                MessageBox.Show("Выберите выставку для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить выставку '{SelectedExhibition.Title}'? Все связанные билеты и экспонаты на выставке будут удалены.",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _exhibitionRepo.Delete(SelectedExhibition);
                await _exhibitionRepo.SaveAsync();
                await LoadExhibitionsAsync();
            }
        }

        [RelayCommand]
        private async Task ManageExhibitsAsync()
        {
            if (SelectedExhibition == null)
            {
                MessageBox.Show("Выберите выставку для управления экспонатами.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Загружаем все существующие связи для этой выставки (без трекинга, чтобы избежать конфликтов)
            var allLinks = await _exhibitOnExhibitionRepo.GetAllAsync();
            var existingLinks = allLinks.Where(l => l.ExhibitionFk == SelectedExhibition.Id).ToList();

            var allExhibits = await _exhibitRepo.GetAllAsync();
            var placeTypes = await _placeTypeRepo.GetAllAsync();

            var dialog = new ExhibitSelectionWindow(SelectedExhibition, existingLinks, allExhibits.ToList(), placeTypes.ToList());
            if (dialog.ShowDialog() == true)
            {
                // Удаляем все существующие связи для этой выставки (если они есть)
                foreach (var link in existingLinks)
                {
                    _exhibitOnExhibitionRepo.Delete(link);
                }
                await _exhibitOnExhibitionRepo.SaveAsync();

                // Добавляем новые связи из диалога
                foreach (var link in dialog.UpdatedLinks)
                {
                    var newLink = new ExhibitOnExhibition
                    {
                        ExhibitionFk = SelectedExhibition.Id,
                        ExhibitFk = link.ExhibitFk,
                        ExpositionPlaceTypeFk = link.ExpositionPlaceTypeFk,
                        PlaceIdentifier = link.PlaceIdentifier,
                        LabelData = link.LabelData
                    };
                    await _exhibitOnExhibitionRepo.AddAsync(newLink);
                }
                await _exhibitOnExhibitionRepo.SaveAsync();

                MessageBox.Show("Состав выставки обновлён.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadExhibitionsAsync();
            }
        }
    }
}