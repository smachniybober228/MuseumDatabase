using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Museum.Models;
using Museum.Repository;
using Museum.Views;
using System.Collections.ObjectModel;
using System.Windows;

namespace Museum.ViewModels
{
    public partial class PeopleViewModel : ObservableObject
    {
        private readonly IRepository<Person> _personRepo;
        private readonly IRepository<PersonType> _personTypeRepo;
        private readonly IRepository<RoleEntity> _roleRepo;
        private readonly IRepository<PersonRole> _personRoleRepo;

        [ObservableProperty]
        private ObservableCollection<Person> _people;

        [ObservableProperty]
        private Person _selectedPerson;

        [ObservableProperty]
        private bool _isLoading;

        public PeopleViewModel(
            IRepository<Person> personRepo,
            IRepository<PersonType> personTypeRepo,
            IRepository<RoleEntity> roleRepo,
            IRepository<PersonRole> personRoleRepo)
        {
            _personRepo = personRepo;
            _personTypeRepo = personTypeRepo;
            _roleRepo = roleRepo;
            _personRoleRepo = personRoleRepo;
            LoadPeopleAsync();
        }

        [RelayCommand]
        private async Task LoadPeopleAsync()
        {
            IsLoading = true;
            var list = await _personRepo.GetAllAsync();
            People = new ObservableCollection<Person>(list);
            IsLoading = false;
        }

        [RelayCommand]
        private async Task AddPersonAsync()
        {
            var personTypes = await _personTypeRepo.GetAllAsync();
            var roles = await _roleRepo.GetAllAsync();

            var dialog = new PersonEditDialog(new Person(), personTypes.ToList(), roles.ToList());
            if (dialog.ShowDialog() == true)
            {
                var newPerson = dialog.EditedPerson;
                await _personRepo.AddAsync(newPerson);
                await _personRepo.SaveAsync();

                // Сохраняем роли
                foreach (var roleId in dialog.SelectedRoleIds)
                {
                    var personRole = new PersonRole { PersonFk = newPerson.Id, RoleFk = roleId };
                    await _personRoleRepo.AddAsync(personRole);
                }
                await _personRoleRepo.SaveAsync();

                await LoadPeopleAsync();
                MessageBox.Show("Человек добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        [RelayCommand]
        private async Task EditPersonAsync()
        {
            if (SelectedPerson == null)
            {
                MessageBox.Show("Выберите человека для редактирования.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var personTypes = await _personTypeRepo.GetAllAsync();
            var roles = await _roleRepo.GetAllAsync();

            // Загружаем текущие роли человека
            var allPersonRoles = await _personRoleRepo.GetAllAsync();
            var existingRoleIds = allPersonRoles.Where(pr => pr.PersonFk == SelectedPerson.Id).Select(pr => pr.RoleFk).ToList();

            var dialog = new PersonEditDialog(SelectedPerson, personTypes.ToList(), roles.ToList(), existingRoleIds);
            if (dialog.ShowDialog() == true)
            {
                // Обновляем основные поля
                SelectedPerson.FullName = dialog.EditedPerson.FullName;
                SelectedPerson.PersonTypeFk = dialog.EditedPerson.PersonTypeFk;
                SelectedPerson.ContactPhone = dialog.EditedPerson.ContactPhone;
                SelectedPerson.ContactEmail = dialog.EditedPerson.ContactEmail;

                _personRepo.Update(SelectedPerson);
                await _personRepo.SaveAsync();

                // Обновляем роли: удаляем старые, добавляем новые
                var toDelete = allPersonRoles.Where(pr => pr.PersonFk == SelectedPerson.Id).ToList();
                foreach (var del in toDelete)
                    _personRoleRepo.Delete(del);

                foreach (var roleId in dialog.SelectedRoleIds)
                {
                    var personRole = new PersonRole { PersonFk = SelectedPerson.Id, RoleFk = roleId };
                    await _personRoleRepo.AddAsync(personRole);
                }
                await _personRoleRepo.SaveAsync();

                await LoadPeopleAsync();
                MessageBox.Show("Данные обновлены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        [RelayCommand]
        private async Task DeletePersonAsync()
        {
            if (SelectedPerson == null)
            {
                MessageBox.Show("Выберите человека для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить '{SelectedPerson.FullName}'? Все связанные данные (акты, роли и т.д.) будут удалены автоматически.", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _personRepo.Delete(SelectedPerson);
                await _personRepo.SaveAsync();
                await LoadPeopleAsync();
                MessageBox.Show("Запись удалена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}