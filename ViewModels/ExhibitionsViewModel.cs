using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Museum.Models;
using Museum.Views;
using System.Collections.ObjectModel;
using System.Windows;

namespace Museum.ViewModels
{
    public partial class ExhibitionsViewModel : ObservableObject
    {
        private readonly IDbContextFactory<MuseumDbContext> _contextFactory;

        [ObservableProperty]
        private ObservableCollection<Exhibition> _exhibitions;

        [ObservableProperty]
        private Exhibition _selectedExhibition;

        [ObservableProperty]
        private bool _isLoading;

        public ExhibitionsViewModel(IDbContextFactory<MuseumDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
            LoadExhibitionsAsync();
        }

        [RelayCommand]
        private async Task LoadExhibitionsAsync()
        {
            IsLoading = true;
            using var context = await _contextFactory.CreateDbContextAsync();
            var list = await context.Exhibitions
                .Include(e => e.HallFkNavigation)
                .Include(e => e.ResponsibleCuratorFkNavigation)
                .Include(e => e.ExhibitionStatusFkNavigation)
                .Include(e => e.ExhibitOnExhibitions)
                    .ThenInclude(eoe => eoe.ExhibitFkNavigation)
                .ToListAsync();
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
            var dialog = new ExhibitionEditWindow(newExhibition, _contextFactory);
            if (dialog.ShowDialog() == true)
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                context.Exhibitions.Add(dialog.EditedExhibition);
                await context.SaveChangesAsync();
                await LoadExhibitionsAsync();
                MessageBox.Show("Выставка добавлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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

            using var context = await _contextFactory.CreateDbContextAsync();
            var exhibitionFromDb = await context.Exhibitions
                .Include(e => e.HallFkNavigation)
                .Include(e => e.ResponsibleCuratorFkNavigation)
                .Include(e => e.ExhibitionStatusFkNavigation)
                .Include(e => e.ExhibitOnExhibitions)
                    .ThenInclude(eoe => eoe.ExhibitFkNavigation)
                .FirstOrDefaultAsync(e => e.Id == SelectedExhibition.Id);

            if (exhibitionFromDb == null) return;

            var dialog = new ExhibitionEditWindow(exhibitionFromDb, _contextFactory);
            if (dialog.ShowDialog() == true)
            {
                await context.SaveChangesAsync();
                await LoadExhibitionsAsync();
                MessageBox.Show("Выставка обновлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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

            if (MessageBox.Show($"Удалить выставку '{SelectedExhibition.Title}'? Все связанные билеты и экспонаты на выставке будут удалены.", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                var exhibition = await context.Exhibitions.FindAsync(SelectedExhibition.Id);
                if (exhibition != null)
                {
                    context.Exhibitions.Remove(exhibition);
                    await context.SaveChangesAsync();
                    await LoadExhibitionsAsync();
                    MessageBox.Show("Выставка удалена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
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

            using var context = await _contextFactory.CreateDbContextAsync();
            var exhibition = await context.Exhibitions
                .Include(e => e.ExhibitOnExhibitions)
                .FirstOrDefaultAsync(e => e.Id == SelectedExhibition.Id);

            var allExhibits = await context.Exhibits.ToListAsync();
            var placeTypes = await context.ExpositionPlaceTypes.ToListAsync();

            var existingLinks = exhibition?.ExhibitOnExhibitions?.ToList() ?? new List<ExhibitOnExhibition>();

            var dialog = new ExhibitSelectionWindow(exhibition, existingLinks, allExhibits, placeTypes);
            if (dialog.ShowDialog() == true)
            {
                // Обновляем связи: удаляем старые, добавляем новые
                if (exhibition != null)
                {
                    context.ExhibitOnExhibitions.RemoveRange(existingLinks);
                    foreach (var link in dialog.UpdatedLinks)
                    {
                        context.ExhibitOnExhibitions.Add(new ExhibitOnExhibition
                        {
                            ExhibitionFk = exhibition.Id,
                            ExhibitFk = link.ExhibitFk,
                            ExpositionPlaceTypeFk = link.ExpositionPlaceTypeFk,
                            PlaceIdentifier = link.PlaceIdentifier,
                            LabelData = link.LabelData
                        });
                    }
                    await context.SaveChangesAsync();
                    MessageBox.Show("Состав выставки обновлён.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadExhibitionsAsync();
                }
            }
        }
    }
}