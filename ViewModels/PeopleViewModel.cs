using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Museum.Models;
using Museum.Views;
using System.Collections.ObjectModel;
using System.Windows;

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
            .Include(p => p.PersonRoles)
            .ThenInclude(pr => pr.RoleFkNavigation)
            .ToListAsync();
        People = new ObservableCollection<Person>(list);
        IsLoading = false;
    }

    [RelayCommand]
    private async Task AddPersonAsync()
    {
        var personTypes = await GetPersonTypesAsync();
        var roles = await GetRolesAsync();

        var dialog = new PersonEditDialog(new Person(), personTypes, roles);
        if (dialog.ShowDialog() == true)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var newPerson = dialog.EditedPerson;
            context.People.Add(newPerson);
            await context.SaveChangesAsync();

            // Сохраняем роли
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

        var personTypes = await GetPersonTypesAsync();
        var roles = await GetRolesAsync();

        using var context = await _contextFactory.CreateDbContextAsync();
        // Загружаем человека с его текущими ролями из БД (отдельный экземпляр, не SelectedPerson)
        var personFromDb = await context.People
            .Include(p => p.PersonRoles)
            .FirstOrDefaultAsync(p => p.Id == SelectedPerson.Id);

        if (personFromDb == null) return;

        var existingRoleIds = personFromDb.PersonRoles.Select(pr => pr.RoleFk).ToList();

        var dialog = new PersonEditDialog(personFromDb, personTypes, roles, existingRoleIds);
        if (dialog.ShowDialog() == true)
        {
            // Обновляем поля
            personFromDb.FullName = dialog.EditedPerson.FullName;
            personFromDb.PersonTypeFk = dialog.EditedPerson.PersonTypeFk;
            personFromDb.ContactPhone = dialog.EditedPerson.ContactPhone;
            personFromDb.ContactEmail = dialog.EditedPerson.ContactEmail;

            // Обновляем роли: удаляем старые, добавляем новые
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
        if (SelectedPerson == null)
        {
            MessageBox.Show("Выберите человека для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (MessageBox.Show($"Удалить '{SelectedPerson.FullName}'? Все связанные данные будут удалены.", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            return;

        using var context = await _contextFactory.CreateDbContextAsync();
        // Загружаем человека со всеми зависимостями, которые могут блокировать удаление
        var person = await context.People
            .Include(p => p.PersonRoles)
            .Include(p => p.ReceiptActSourceFkNavigations)
                .ThenInclude(act => act.Exhibits)   // чтобы загрузить экспонаты актов
            .Include(p => p.ReceiptActResponsiblePersonFkNavigations)
                .ThenInclude(act => act.Exhibits)
            .Include(p => p.Exhibitions)            // где он куратор
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

        if (person != null)
        {
            // 1. Удаляем экспонаты, привязанные к актам поступления (чтобы не было конфликта с ReceiptActFk)
            var exhibitsToDelete = person.ReceiptActSourceFkNavigations
                .SelectMany(act => act.Exhibits)
                .Concat(person.ReceiptActResponsiblePersonFkNavigations.SelectMany(act => act.Exhibits))
                .Distinct()
                .ToList();
            context.Exhibits.RemoveRange(exhibitsToDelete);

            // 2. Удаляем сами акты поступления
            context.ReceiptActs.RemoveRange(person.ReceiptActSourceFkNavigations);
            context.ReceiptActs.RemoveRange(person.ReceiptActResponsiblePersonFkNavigations);

            // 3. Удаляем реставрационные заказы (и всё, что на них ссылается, удалится по каскаду)
            context.RestorationOrderEntities.RemoveRange(person.RestorationOrderEntityRestorerFkNavigations);
            context.RestorationOrderEntities.RemoveRange(person.RestorationOrderEntityInitiatorFkNavigations);

            // 4. Удаляем выставки, где он куратор
            context.Exhibitions.RemoveRange(person.Exhibitions);

            // 5. Удаляем роли
            context.PersonRoles.RemoveRange(person.PersonRoles);

            // 6. Удаляем самого человека
            context.People.Remove(person);

            await context.SaveChangesAsync();
            await LoadPeopleAsync();
            MessageBox.Show("Запись удалена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private async Task<List<PersonType>> GetPersonTypesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.PersonTypes.ToListAsync();
    }

    private async Task<List<RoleEntity>> GetRolesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.RoleEntities.ToListAsync();
    }
}