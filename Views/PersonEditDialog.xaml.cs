using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Museum.Models;

namespace Museum.Views
{
    public partial class PersonEditDialog : Window
    {
        public Person EditedPerson { get; private set; }
        public List<PersonType> PersonTypes { get; }
        public ObservableCollection<RoleSelection> Roles { get; }
        public List<int> SelectedRoleIds => Roles.Where(r => r.IsSelected).Select(r => r.Id).ToList();

        public PersonEditDialog(Person person, List<PersonType> personTypes, List<RoleEntity> roles, List<int> selectedRoleIds = null)
        {
            InitializeComponent();
            EditedPerson = person ?? new Person();
            PersonTypes = personTypes;
            Roles = new ObservableCollection<RoleSelection>(roles.Select(r => new RoleSelection { Id = r.Id, Title = r.Title, IsSelected = selectedRoleIds?.Contains(r.Id) ?? false }));
            DataContext = this;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EditedPerson.FullName))
            {
                MessageBox.Show("Введите ФИО или название организации.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    public class RoleSelection
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsSelected { get; set; }
    }
}