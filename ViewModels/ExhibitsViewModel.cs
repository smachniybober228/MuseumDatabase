using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Museum.Models;
using Museum.Views;
using System.Collections.ObjectModel;
using System.Windows;

namespace Museum.ViewModels
{
    public partial class ExhibitsViewModel : ObservableObject
    {
        private readonly IDbContextFactory<MuseumDbContext> _contextFactory;

        [ObservableProperty]
        private ObservableCollection<Exhibit> _exhibits;

        [ObservableProperty]
        private Exhibit _selectedExhibit;

        [ObservableProperty]
        private bool _isLoading;

        public ExhibitsViewModel(IDbContextFactory<MuseumDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
            LoadExhibitsAsync();
        }

        [RelayCommand]
        private async Task LoadExhibitsAsync()
        {
            IsLoading = true;
            using var context = await _contextFactory.CreateDbContextAsync();
            var list = await context.Exhibits
                .Include(e => e.ExhibitStatusFkNavigation)
                .Include(e => e.ReceiptActFkNavigation)
                .Include(e => e.ExhibitCategories)
                    .ThenInclude(ec => ec.CategoryFkNavigation)
                .Include(e => e.ExhibitMaterials)
                    .ThenInclude(em => em.MaterialFkNavigation)
                .Include(e => e.ExhibitTechniques)
                    .ThenInclude(et => et.TechniqueFkNavigation)
                .ToListAsync();
            Exhibits = new ObservableCollection<Exhibit>(list);
            IsLoading = false;
        }

        [RelayCommand]
        private async Task AddExhibitAsync()
        {
            var dialog = new ExhibitEditWindow(null, _contextFactory);
            if (dialog.ShowDialog() == true)
            {
                await LoadExhibitsAsync();
                MessageBox.Show("Экспонат добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        [RelayCommand]
        private async Task EditExhibitAsync()
        {
            if (SelectedExhibit == null)
            {
                MessageBox.Show("Выберите экспонат для редактирования.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var context = await _contextFactory.CreateDbContextAsync();
            var exhibitFromDb = await context.Exhibits
                .Include(e => e.ExhibitStatusFkNavigation)
                .Include(e => e.ReceiptActFkNavigation)
                .Include(e => e.ExhibitCategories)
                    .ThenInclude(ec => ec.CategoryFkNavigation)
                .Include(e => e.ExhibitMaterials)
                    .ThenInclude(em => em.MaterialFkNavigation)
                .Include(e => e.ExhibitTechniques)
                    .ThenInclude(et => et.TechniqueFkNavigation)
                .FirstOrDefaultAsync(e => e.Id == SelectedExhibit.Id);

            if (exhibitFromDb == null) return;

            var dialog = new ExhibitEditWindow(exhibitFromDb, _contextFactory);
            if (dialog.ShowDialog() == true)
            {
                await LoadExhibitsAsync();
                MessageBox.Show("Экспонат обновлён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        [RelayCommand]
        private async Task DeleteExhibitAsync()
        {
            if (SelectedExhibit == null)
            {
                MessageBox.Show("Выберите экспонат для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageBox.Show($"Удалить экспонат '{SelectedExhibit.Title}'? Все связанные данные будут удалены.", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                var exhibit = await context.Exhibits.FindAsync(SelectedExhibit.Id);
                if (exhibit != null)
                {
                    context.Exhibits.Remove(exhibit);
                    await context.SaveChangesAsync();
                    await LoadExhibitsAsync();
                    MessageBox.Show("Экспонат удалён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}