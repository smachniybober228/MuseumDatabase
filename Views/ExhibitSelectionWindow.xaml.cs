using Museum.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Museum.Views
{
    public partial class ExhibitSelectionWindow : Window
    {
        private readonly int _exhibitionId;
        private readonly Dictionary<int, Exhibit> _allExhibitsDict;

        public ObservableCollection<Exhibit> AvailableExhibits { get; set; }
        public ObservableCollection<ExhibitOnExhibition> AddedLinks { get; set; }
        public ObservableCollection<ExpositionPlaceType> PlaceTypes { get; set; }

        public Exhibit SelectedAvailableExhibit { get; set; }

        public ExhibitSelectionWindow(Exhibition exhibition,
                                      List<ExhibitOnExhibition> existingLinks,
                                      List<Exhibit> allExhibits,
                                      List<ExpositionPlaceType> placeTypes)
        {
            InitializeComponent();

            _exhibitionId = exhibition.Id;
            _allExhibitsDict = allExhibits.ToDictionary(e => e.Id);

            PlaceTypes = new ObservableCollection<ExpositionPlaceType>(placeTypes);
            var existingExhibitIds = existingLinks.Select(l => l.ExhibitFk).ToHashSet();
            AvailableExhibits = new ObservableCollection<Exhibit>(allExhibits.Where(e => !existingExhibitIds.Contains(e.Id)));
            AddedLinks = new ObservableCollection<ExhibitOnExhibition>(existingLinks);

            DataContext = this;
        }

        private void AddExhibit_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedAvailableExhibit != null)
            {
                var newLink = new ExhibitOnExhibition
                {
                    ExhibitionFk = _exhibitionId,           // правильный Id выставки
                    ExhibitFk = SelectedAvailableExhibit.Id,
                    ExpositionPlaceTypeFk = PlaceTypes.FirstOrDefault()?.Id ?? 0,
                    PlaceIdentifier = "",
                    LabelData = "",
                    ExhibitFkNavigation = SelectedAvailableExhibit      // для отображения
                };
                AddedLinks.Add(newLink);
                AvailableExhibits.Remove(SelectedAvailableExhibit);
            }
        }

        private void RemoveExhibit_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is ExhibitOnExhibition selectedLink)
            {
                // Восстанавливаем полный экспонат из словаря
                if (_allExhibitsDict.TryGetValue(selectedLink.ExhibitFk, out var fullExhibit))
                {
                    AvailableExhibits.Add(fullExhibit);
                }
                AddedLinks.Remove(selectedLink);
            }
        }

        public List<ExhibitOnExhibition> UpdatedLinks => AddedLinks.ToList();

        private void Save_Click(object sender, RoutedEventArgs e)
        {
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