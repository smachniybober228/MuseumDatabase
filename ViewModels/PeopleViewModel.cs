using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Museum.Models;
using Museum.Views;
using System.Collections.ObjectModel;
using System.Windows;

namespace Museum.ViewModels
{
    public partial class PeopleViewModel : ObservableObject
    {
        private readonly IDbContextFactory<MuseumDbContext> _contextFactory;

        [ObservableProperty]
        private ObservableCollection<Person> _people;

        [ObservableProperty]
        private Person _selectedPerson;

        [ObservableProperty]
        private bool _isLoading;

        public PeopleViewModel(IDbContextFactory<MuseumDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task LoadPeopleAsync()
        {
            IsLoading = true;
            using var context = await _contextFactory.CreateDbContextAsync();
            var list = await context.People
                .Include(p => p.PersonTypeFkNavigation)
                .Include(p => p.PersonRoles)
                    .ThenInclude(pr => pr.RoleFkNavigation)
                .ToListAsync();
            People = new ObservableCollection<Person>(list);
            IsLoading = false;
        }

        [RelayCommand]
        private async Task AddPersonAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var personTypes = await context.PersonTypes.ToListAsync();
            var roles = await context.RoleEntities.ToListAsync();
            var dialog = new PersonEditDialog(null, personTypes, roles);
            if (dialog.ShowDialog() == true)
            {
                var newPerson = dialog.EditedPerson;
                context.People.Add(newPerson);
                await context.SaveChangesAsync();

                foreach (var roleId in dialog.SelectedRoleIds)
                {
                    context.PersonRoles.Add(new PersonRole { PersonFk = newPerson.Id, RoleFk = roleId });
                }
                await context.SaveChangesAsync();
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

            using var context = await _contextFactory.CreateDbContextAsync();
            var personFromDb = await context.People
                .Include(p => p.PersonRoles)
                .FirstOrDefaultAsync(p => p.Id == SelectedPerson.Id);
            if (personFromDb == null) return;

            var personTypes = await context.PersonTypes.ToListAsync();
            var roles = await context.RoleEntities.ToListAsync();
            var existingRoleIds = personFromDb.PersonRoles.Select(pr => pr.RoleFk).ToList();

            var dialog = new PersonEditDialog(personFromDb, personTypes, roles, existingRoleIds);
            if (dialog.ShowDialog() == true)
            {
                personFromDb.FullName = dialog.EditedPerson.FullName;
                personFromDb.PersonTypeFk = dialog.EditedPerson.PersonTypeFk;
                personFromDb.ContactPhone = dialog.EditedPerson.ContactPhone;
                personFromDb.ContactEmail = dialog.EditedPerson.ContactEmail;

                // Обновляем роли
                context.PersonRoles.RemoveRange(personFromDb.PersonRoles);
                foreach (var roleId in dialog.SelectedRoleIds)
                {
                    context.PersonRoles.Add(new PersonRole { PersonFk = personFromDb.Id, RoleFk = roleId });
                }
                await context.SaveChangesAsync();
                await LoadPeopleAsync();
                MessageBox.Show("Данные обновлены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        [RelayCommand]
        private async Task DeletePersonAsync()
        {
            if (SelectedPerson == null) return;
            if (MessageBox.Show($"Удалить '{SelectedPerson.FullName}'? Все связанные данные будут удалены.", "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            using var context = await _contextFactory.CreateDbContextAsync();

            // Загружаем человека со всеми зависимостями
            var person = await context.People
                .Include(p => p.PersonRoles)
                .Include(p => p.ReceiptActSourceFkNavigations)
                    .ThenInclude(act => act.Exhibits) // загружаем экспонаты, привязанные к актам
                .Include(p => p.ReceiptActResponsiblePersonFkNavigations)
                    .ThenInclude(act => act.Exhibits)
                .Include(p => p.Exhibitions)
                .Include(p => p.RestorationOrderEntityRestorerFkNavigations)
                    .ThenInclude(o => o.RequiredWorkTypes)
                .Include(p => p.RestorationOrderEntityRestorerFkNavigations)
                    .ThenInclude(o => o.WorkLogEntries)
                .Include(p => p.RestorationOrderEntityRestorerFkNavigations)
                    .ThenInclude(o => o.RestorationActs)
                .Include(p => p.RestorationOrderEntityRestorerFkNavigations)
                    .ThenInclude(o => o.ReturnActs)
                .Include(p => p.RestorationOrderEntityInitiatorFkNavigations)
                    .ThenInclude(o => o.RequiredWorkTypes)
                .Include(p => p.RestorationOrderEntityInitiatorFkNavigations)
                    .ThenInclude(o => o.WorkLogEntries)
                .FirstOrDefaultAsync(p => p.Id == SelectedPerson.Id);

            if (person == null) return;

            // 1. Удаляем роли
            context.PersonRoles.RemoveRange(person.PersonRoles);

            // 2. Удаляем реставрационные заказы (где человек реставратор или инициатор)
            var orders = person.RestorationOrderEntityRestorerFkNavigations
                .Concat(person.RestorationOrderEntityInitiatorFkNavigations)
                .ToList();
            foreach (var order in orders)
            {
                context.RequiredWorkTypes.RemoveRange(order.RequiredWorkTypes);
                context.WorkLogEntries.RemoveRange(order.WorkLogEntries);
                context.RestorationActs.RemoveRange(order.RestorationActs);
                context.ReturnActs.RemoveRange(order.ReturnActs);
                context.RestorationOrderEntities.Remove(order);
            }

            // 3. Удаляем выставки, где человек – куратор
            context.Exhibitions.RemoveRange(person.Exhibitions);

            // 4. Удаляем акты поступления (и связанные с ними экспонаты)
            var acts = person.ReceiptActSourceFkNavigations
                .Concat(person.ReceiptActResponsiblePersonFkNavigations)
                .ToList();
            foreach (var act in acts)
            {
                // удаляем экспонаты, связанные с этим актом
                context.Exhibits.RemoveRange(act.Exhibits);
                context.ReceiptActs.Remove(act);
            }

            // 5. Удаляем самого человека
            context.People.Remove(person);

            await context.SaveChangesAsync();
            await LoadPeopleAsync();
            MessageBox.Show("Запись удалена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}