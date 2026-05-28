using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Museum.Models;
using System.Collections.ObjectModel;

namespace Museum.ViewModels
{
    public partial class ExhibitViewModel : ObservableObject
    {
        private readonly IRepository<Exhibit> _exhibitRepo;
        private readonly IRepository<ExhibitStatus> _statusRepo;
        private readonly IRepository<ReceiptAct> _actRepo;

        [ObservableProperty]
        private ObservableCollection<Exhibit> exhibits;

        [ObservableProperty]
        private Exhibit selectedExhibit;

        [ObservableProperty]
        private string searchText;

        [RelayCommand]
        private async Task LoadExhibits()
        {
            var list = await _exhibitRepo.GetAllAsync();
            Exhibits = new ObservableCollection<Exhibit>(list);
        }

        [RelayCommand]
        private async Task AddEditExhibit()
        { 
            // todo
            //// Открыть диалоговое окно для редактирования
            //var dialog = new ExhibitDialogViewModel(SelectedExhibit);
            //if (dialog.ShowDialog() == true)
            //{
            //    if (SelectedExhibit == null)
            //        await _exhibitRepo.AddAsync(dialog.Exhibit);
            //    else
            //        _exhibitRepo.Update(dialog.Exhibit);
            //    await _exhibitRepo.SaveAsync();
            //    await LoadExhibits();
            //}
        }

        [RelayCommand]
        private async Task DeleteExhibit()
        {
            if (SelectedExhibit != null)
            {
                _exhibitRepo.Delete(SelectedExhibit);
                await _exhibitRepo.SaveAsync();
                await LoadExhibits();
            }
        }
    }
}
