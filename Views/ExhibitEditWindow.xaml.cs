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
    public partial class ExhibitEditWindow : Window, INotifyPropertyChanged
    {
        public Exhibit EditedExhibit { get; private set; }
        private readonly IDbContextFactory<MuseumDbContext> _contextFactory;

        private List<ExhibitStatus> _statuses;
        private List<ReceiptAct> _receiptActs;
        private List<SelectableCategory> _selectableCategories;
        private List<SelectableMaterial> _selectableMaterials;
        private List<SelectableTechnique> _selectableTechniques;

        private ExhibitStatus _selectedStatus;
        private ReceiptAct _selectedReceiptAct;

        public List<ExhibitStatus> Statuses { get => _statuses; set { _statuses = value; OnPropertyChanged(); } }
        public List<ReceiptAct> ReceiptActs { get => _receiptActs; set { _receiptActs = value; OnPropertyChanged(); } }
        public List<SelectableCategory> SelectableCategories { get => _selectableCategories; set { _selectableCategories = value; OnPropertyChanged(); } }
        public List<SelectableMaterial> SelectableMaterials { get => _selectableMaterials; set { _selectableMaterials = value; OnPropertyChanged(); } }
        public List<SelectableTechnique> SelectableTechniques { get => _selectableTechniques; set { _selectableTechniques = value; OnPropertyChanged(); } }

        public ExhibitStatus SelectedStatus { get => _selectedStatus; set { _selectedStatus = value; OnPropertyChanged(); } }
        public ReceiptAct SelectedReceiptAct { get => _selectedReceiptAct; set { _selectedReceiptAct = value; OnPropertyChanged(); } }

        public Exhibit ExhibitData => EditedExhibit;

        public ExhibitEditWindow(Exhibit exhibit, IDbContextFactory<MuseumDbContext> contextFactory)
        {
            InitializeComponent();
            _contextFactory = contextFactory;
            EditedExhibit = exhibit ?? new Exhibit();
            Loaded += async (s, e) => await LoadDataAsync();
            DataContext = this;
        }

        private async Task LoadDataAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            Statuses = await context.ExhibitStatuses.ToListAsync();
            ReceiptActs = await context.ReceiptActs.ToListAsync();

            var allCategories = await context.Categories.ToListAsync();
            var allMaterials = await context.Materials.ToListAsync();
            var allTechniques = await context.Techniques.ToListAsync();

            List<int> selectedCategoryIds = new List<int>();
            List<int> selectedMaterialIds = new List<int>();
            List<int> selectedTechniqueIds = new List<int>();

            if (EditedExhibit.Id != 0)
            {
                // Для существующего экспоната загружаем текущие связи
                var existing = await context.Exhibits
                    .Include(e => e.ExhibitCategories)
                    .Include(e => e.ExhibitMaterials)
                    .Include(e => e.ExhibitTechniques)
                    .FirstOrDefaultAsync(e => e.Id == EditedExhibit.Id);
                if (existing != null)
                {
                    selectedCategoryIds = existing.ExhibitCategories.Select(ec => ec.CategoryFk).ToList();
                    selectedMaterialIds = existing.ExhibitMaterials.Select(em => em.MaterialFk).ToList();
                    selectedTechniqueIds = existing.ExhibitTechniques.Select(et => et.TechniqueFk).ToList();
                }
            }

            SelectableCategories = allCategories.Select(c => new SelectableCategory
            {
                Category = c,
                IsSelected = selectedCategoryIds.Contains(c.Id)
            }).ToList();

            SelectableMaterials = allMaterials.Select(m => new SelectableMaterial
            {
                Material = m,
                IsSelected = selectedMaterialIds.Contains(m.Id)
            }).ToList();

            SelectableTechniques = allTechniques.Select(t => new SelectableTechnique
            {
                Technique = t,
                IsSelected = selectedTechniqueIds.Contains(t.Id)
            }).ToList();

            SelectedStatus = Statuses.FirstOrDefault(s => s.Id == EditedExhibit.ExhibitStatusFk);
            SelectedReceiptAct = ReceiptActs.FirstOrDefault(r => r.Id == EditedExhibit.ReceiptActFk);
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(EditedExhibit.InventoryNumber))
            {
                MessageBox.Show("Инвентарный номер обязателен.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (SelectedStatus == null)
            {
                MessageBox.Show("Выберите статус.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (SelectedReceiptAct == null)
            {
                MessageBox.Show("Выберите акт поступления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            EditedExhibit.ExhibitStatusFk = SelectedStatus.Id;
            EditedExhibit.ReceiptActFk = SelectedReceiptAct.Id;

            using var context = await _contextFactory.CreateDbContextAsync();

            if (EditedExhibit.Id == 0)
            {
                // Новый экспонат
                context.Exhibits.Add(EditedExhibit);
                await context.SaveChangesAsync();
            }
            else
            {
                // Существующий: обновляем
                context.Exhibits.Attach(EditedExhibit);
                context.Entry(EditedExhibit).State = EntityState.Modified;
            }

            // Обновляем связи (удаляем старые, добавляем новые)
            var oldCategories = context.ExhibitCategories.Where(ec => ec.ExhibitFk == EditedExhibit.Id);
            var oldMaterials = context.ExhibitMaterials.Where(em => em.ExhibitFk == EditedExhibit.Id);
            var oldTechniques = context.ExhibitTechniques.Where(et => et.ExhibitFk == EditedExhibit.Id);
            context.ExhibitCategories.RemoveRange(oldCategories);
            context.ExhibitMaterials.RemoveRange(oldMaterials);
            context.ExhibitTechniques.RemoveRange(oldTechniques);

            foreach (var sc in SelectableCategories.Where(sc => sc.IsSelected))
                context.ExhibitCategories.Add(new ExhibitCategory { ExhibitFk = EditedExhibit.Id, CategoryFk = sc.Category.Id });
            foreach (var sm in SelectableMaterials.Where(sm => sm.IsSelected))
                context.ExhibitMaterials.Add(new ExhibitMaterial { ExhibitFk = EditedExhibit.Id, MaterialFk = sm.Material.Id });
            foreach (var st in SelectableTechniques.Where(st => st.IsSelected))
                context.ExhibitTechniques.Add(new ExhibitTechnique { ExhibitFk = EditedExhibit.Id, TechniqueFk = st.Technique.Id });

            await context.SaveChangesAsync();
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

    // Вспомогательные классы для отображения с флажком
    public class SelectableCategory
    {
        public Category Category { get; set; }
        public bool IsSelected { get; set; }
    }
    public class SelectableMaterial
    {
        public Material Material { get; set; }
        public bool IsSelected { get; set; }
    }
    public class SelectableTechnique
    {
        public Technique Technique { get; set; }
        public bool IsSelected { get; set; }
    }
}