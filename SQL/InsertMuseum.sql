use бобор;
GO

-- ======================================================================
-- ЗАПОЛНЕНИЕ БАЗЫ ДАННЫХ ТЕСТОВЫМИ ДАННЫМИ (SQL Server)
-- ======================================================================

-- (Очистка таблиц, если нужна, закомментирована)
-- delete from WorkLogEntry; delete from RequiredWorkType; delete from RestorationAct;
-- delete from ReturnAct; delete from WriteOffAct; delete from Ticket; delete from PersonRole;
-- delete from ExhibitPhoto; delete from ExhibitOnExhibition; delete from Exhibition;
-- delete from Hall; delete from RestorationOrderEntity; delete from Exhibit;
-- delete from ReceiptAct; delete from Person; delete from ExhibitCategory;
-- delete from ExhibitMaterial; delete from ExhibitTechnique; delete from Category;
-- delete from Material; delete from Technique; delete from ExhibitStatus;
-- delete from ReceiptMethod; delete from PhotoType; delete from ExpositionPlaceType;
-- delete from ExhibitionStatus; delete from RestorationWorkType; delete from TicketStatus;
-- delete from RoleEntity; delete from PersonType; delete from FloorEntity;

-- ======================================================================
-- 2. Заполнение справочных таблиц
-- ======================================================================
insert into ExhibitStatus (Title) values
('В основном фонде'),
('В запасниках'),
('На реставрации'),
('В постоянной экспозиции'),
('На временной выставке'),
('Списан');

insert into ReceiptMethod (Title) values
('покупка'),
('дарение'),
('обмен'),
('передача из другого музея'),
('находка');

insert into PhotoType (Title) values
('общий вид'),
('деталь'),
('повреждение');

insert into Category (Title) values
('живопись'),
('скульптура'),
('графика'),
('прикладное искусство'),
('археология'),
('документы');

insert into Material (Title) values
('дерево'),
('холст'),
('металл'),
('керамика'),
('бумага');

insert into Technique (Title) values
('масло'),
('акварель'),
('резьба'),
('литьё'),
('гравировка');

insert into FloorEntity (FloorNumber, Title) values
(1, 'Первый этаж'),
(2, 'Второй этаж');

insert into ExhibitionStatus (Title) values
('Проект'),
('Активна'),
('Завершена'),
('Отменена');

insert into ExpositionPlaceType (Title) values
('витрина'),
('стена'),
('подиум'),
('пьедестал');

insert into RestorationWorkType (Title) values
('очистка'),
('укрепление грунта'),
('тонирование'),
('восполнение утрат');

insert into RoleEntity (Title) values
('Куратор'),
('Хранитель'),
('Реставратор'),
('Директор'),
('Билетёр'),
('Даритель');

insert into TicketStatus (Title) values
('Продан'),
('Использован');

insert into PersonType (TypeName) values
('Физическое'),
('Юридическое');

-- ======================================================================
-- 3. Заполнение основных таблиц
-- ======================================================================

-- 3.1. Люди
insert into Person (FullName, PersonTypeFk, ContactPhone, ContactEmail) values
('Иванов Иван Иванович', 1, '+7-123-456-78-90', 'ivanov@museum.ru'),
('Петрова Анна Сергеевна', 1, '+7-123-456-78-91', 'petrova@museum.ru'),
('Сидоров Василий Петрович', 1, '+7-123-456-78-92', 'sidorov@museum.ru'),
('ООО "АнтикварЪ"', 2, '+7-495-123-45-67', 'info@antikvar.ru'),
('Государственный Эрмитаж', 2, '+7-812-123-45-67', 'hermitage@museum.ru');

-- 3.2. Акты поступления
insert into ReceiptAct (ActDate, SourceFk, ReceiptMethodFk, ResponsiblePersonFk) values
('2024-01-15', 4, 1, 1),
('2024-02-20', 5, 4, 2),
('2024-03-10', 1, 2, 1);

-- 3.3. Экспонаты
insert into Exhibit (InventoryNumber, Title, LengthCm, WidthCm, HeightCm, CreationDate, Author, OriginPlace, ExhibitStatusFk, ReceiptActFk) values
('INV-001', 'Утро в сосновом лесу', 150.5, 120.0, 2.5, '1889', 'И.И. Шишкин', 'Россия', 1, 1),
('INV-002', 'Венера Милосская (копия)', 45.0, 30.0, 120.0, 'II век до н.э.', 'Неизвестен', 'Греция', 4, 2),
('INV-003', 'Гжельский чайник', 25.0, 20.0, 30.0, 'XX век', 'Гжельский завод', 'Россия', 2, 1),
('INV-004', 'Находка из кургана', 10.0, 8.0, 5.0, 'VIII век', 'Неизвестен', 'Сибирь', 2, 2),
('INV-005', 'Письмо А.С. Пушкина', 21.0, 29.7, 0.1, '1830', 'А.С. Пушкин', 'Россия', 1, 3);

-- 3.4. Связки экспонатов
insert into ExhibitCategory (ExhibitFk, CategoryFk) values (1, 1), (2, 2), (3, 4), (4, 5), (5, 6);
insert into ExhibitMaterial (ExhibitFk, MaterialFk) values (1, 2), (2, 4), (3, 4), (4, 3), (5, 5);
insert into ExhibitTechnique (ExhibitFk, TechniqueFk) values (1, 1), (2, 4), (3, 3), (5, 5);

-- 3.5. Фото экспонатов
insert into ExhibitPhoto (FileLink, PhotoTypeFk, ShootingDate, ExhibitFk) values
('C:\photos\exhibit1_01.jpg', 1, '2024-01-20', 1),
('C:\photos\exhibit1_02.jpg', 2, '2024-01-20', 1),
('C:\photos\exhibit2_01.jpg', 1, '2024-02-25', 2),
('C:\photos\exhibit5_01.jpg', 1, '2024-03-15', 5);

-- 3.6. Залы
insert into Hall (Title, HallNumber, FloorFk) values
('Зал живописи', '101', 1),
('Зал скульптуры', '102', 1),
('Зал прикладного искусства', '103', 1),
('Археологический зал', '201', 2),
('Рукописный зал', '202', 2);

-- 3.7. Выставки
insert into Exhibition (Title, StartDate, EndDate, HallFk, ResponsibleCuratorFk, ExhibitionStatusFk) values
('Шедевры русской живописи', '2025-05-01', '2025-08-31', 1, 1, 2),
('Искусство Древней Греции', '2025-06-01', '2025-09-30', 2, 2, 2),
('Народные промыслы России', '2025-07-01', '2025-10-31', 3, 1, 2),
('Постоянная экспозиция', '2000-01-01', '2099-12-31', 1, 2, 2);

-- 3.8. Экспонаты на выставках
insert into ExhibitOnExhibition (ExhibitionFk, ExhibitFk, ExpositionPlaceTypeFk, PlaceIdentifier, LabelData) values
(1, 1, 2, 'Стена 1', 'И.И. Шишкин "Утро в сосновом лесу", 1889, холст, масло'),
(2, 2, 4, 'Пьедестал 3', 'Копия Венеры Милосской, II век до н.э.'),
(3, 3, 1, 'Витрина 5', 'Гжельский чайник, XX век'),
(4, 5, 1, 'Витрина 2', 'Письмо А.С. Пушкина, 1830');

-- 3.9. Реставрационные заказы
insert into RestorationOrderEntity (OrderNumber, ExhibitFk, ReceiptDate, ReasonDirection, PlannedCompletionDate, RestorerFk, InitiatorFk) values
('R-001', 4, '2025-01-10', 'Коррозия металла', '2025-04-10', 3, 2),
('R-002', 2, '2025-02-15', 'Сколы на поверхности', '2025-05-15', 3, 2);

-- 3.10. Требуемые виды работ
insert into RequiredWorkType (RestorationOrderFk, WorkTypeFk) values
(1, 1), (1, 2), (2, 1);

-- 3.11. Журнал работ
insert into WorkLogEntry (RestorationOrderFk, ExecutionDate, WorkTypeFk) values
(1, '2025-01-15', 1),
(1, '2025-02-10', 2),
(2, '2025-02-20', 1);

-- 3.12. Акты реставрации
insert into RestorationAct (RestorationOrderFk, CompletionDate, FinalReport, TotalCost) values
(1, '2025-03-01', 'Очистка и укрепление выполнены, состояние удовлетворительное', 15000.0);

-- 3.13. Акты возврата
insert into ReturnAct (RestorationOrderFk, ReturnDate) values
(1, '2025-03-05');

-- 3.14. Билеты
insert into Ticket (TicketNumber, ExhibitionFk, SaleDateTime, SalePrice, VisitDate, TicketStatusFk) values
('T-001', 1, '2025-05-01 10:30:00', 500.0, '2025-05-15', 1),
('T-002', 1, '2025-05-01 11:00:00', 500.0, '2025-05-15', 2),
('T-003', 2, '2025-05-02 09:15:00', 600.0, '2025-05-20', 1),
('T-004', 4, '2025-05-03 14:00:00', 300.0, '2025-05-20', 2);

-- 3.15. Роли людей
insert into PersonRole (PersonFk, RoleFk) values
(1, 1), (2, 2), (3, 3), (1, 6), (2, 5), (4, 6), (5, 6);

-- 3.16. Акт списания
insert into WriteOffAct (ExhibitFk, WriteOffDate, WriteOffReason) values
(3, '2025-01-20', 'Необратимое повреждение при транспортировке');

update Exhibit set ExhibitStatusFk = 6 where Id = 3;

GO