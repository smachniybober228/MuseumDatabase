using Museum.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace Museum.Views
{
    public partial class ExhibitSelectionWindow : Window
    {
        public ObservableCollection<Exhibit> AvailableExhibits { get; set; }
        public ObservableCollection<ExhibitOnExhibition> AddedLinks { get; set; }
        public List<ExpositionPlaceType> PlaceTypes { get; set; }

        public Exhibit SelectedAvailableExhibit { get; set; }

        public ExhibitSelectionWindow(Exhibition exhibition, List<ExhibitOnExhibition> existingLinks, List<Exhibit> allExhibits, List<ExpositionPlaceType> placeTypes)
        {
            InitializeComponent();
            PlaceTypes = placeTypes;
            // Доступные – те, которые не в existingLinks
            var existingExhibitIds = existingLinks.Select(l => l.ExhibitFk).ToHashSet();
            AvailableExhibits = new ObservableCollection<Exhibit>(allExhibits.Where(e => !existingExhibitIds.Contains(e.Id)));
            AddedLinks = new ObservableCollection<ExhibitOnExhibition>(existingLinks);
            DataContext = this;
        }

        private void AddExhibitCommand(object sender, RoutedEventArgs e)
        {
            if (SelectedAvailableExhibit != null)
            {
                var newLink = new ExhibitOnExhibition
                {
                    ExhibitionFk = 0, // заполнится при сохранении
                    ExhibitFk = SelectedAvailableExhibit.Id,
                    ExpositionPlaceTypeFk = PlaceTypes.FirstOrDefault()?.Id ?? 0,
                    PlaceIdentifier = "",
                    LabelData = ""
                };
                AddedLinks.Add(newLink);
                AvailableExhibits.Remove(SelectedAvailableExhibit);
            }
        }

        private void RemoveExhibitCommand(object sender, RoutedEventArgs e)
        {
            var selected = (ExhibitOnExhibition)((dynamic)sender).DataContext; // упрощённо, нужна привязка
            if (selected != null)
            {
                var exhibit = new Exhibit { Id = selected.ExhibitFk, Title = "..." }; // нужно восстановить из БД
                AvailableExhibits.Add(exhibit);
                AddedLinks.Remove(selected);
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