use бобор;
GO

-- ======================================================================
-- СКРИПТ СОЗДАНИЯ БАЗЫ ДАННЫХ МУЗЕЯ И РЕСТАВРАЦИОННОЙ МАСТЕРСКОЙ
-- ======================================================================

-- ======================================================================
-- СОЗДАНИЕ ТАБЛИЦ
-- ======================================================================

-- 1. Справочные таблицы (без внешних ключей)
create table ExhibitStatus (
    Id int identity primary key,
    Title varchar(50) not null unique
);

create table ReceiptMethod (
    Id int identity primary key,
    Title varchar(50) not null unique
);

create table PhotoType (
    Id int identity primary key,
    Title varchar(50) not null unique
);

create table Category (
    Id int identity primary key,
    Title varchar(100) not null unique
);

create table Material (
    Id int identity primary key,
    Title varchar(100) not null unique
);

create table Technique (
    Id int identity primary key,
    Title varchar(100) not null unique
);

create table FloorEntity (
    Id int identity primary key,
    FloorNumber int not null,
    Title varchar(100) not null
);

create table ExhibitionStatus (
    Id int identity primary key,
    Title varchar(50) not null unique
);

create table ExpositionPlaceType (
    Id int identity primary key,
    Title varchar(50) not null unique
);

create table RestorationWorkType (
    Id int identity primary key,
    Title varchar(100) not null unique
);

create table RoleEntity (
    Id int identity primary key,
    Title varchar(50) not null unique
);

create table TicketStatus (
    Id int identity primary key,
    Title varchar(50) not null unique
);

create table PersonType (
    Id int identity primary key,
    TypeName varchar(50) not null unique
);

-- 2. Таблица Человек
create table Person (
    Id int identity primary key,
    FullName varchar(200) not null,
    ContactPhone varchar(20) not null,
    ContactEmail varchar(100) not null
);

-- 3. Таблица Акт поступления
create table ReceiptAct (
    Id int identity primary key,
    ActDate date not null
);

-- 4. Таблица Экспонат
create table Exhibit (
    Id int identity primary key,
    InventoryNumber varchar(50) not null unique,
    Title varchar(200) not null,
    LengthCm float not null default 0,
    WidthCm float not null default 0,
    HeightCm float not null default 0,
    CreationDate varchar(100) not null default 'Неизвестно',
    Author varchar(200) not null default 'Неизвестно',
    OriginPlace varchar(200) not null default 'Неизвестно'
);

-- 5. Таблица Фото экспоната
create table ExhibitPhoto (
    Id int identity primary key,
    FileLink varchar(500) not null,
    ShootingDate date not null
);

-- 6. Связующие таблицы для экспоната
create table ExhibitCategory (
    Id int identity primary key
);

create table ExhibitMaterial (
    Id int identity primary key
);

create table ExhibitTechnique (
    Id int identity primary key
);

-- 7. Таблица Зал
create table Hall (
    Id int identity primary key,
    Title varchar(100) not null,
    HallNumber varchar(20) not null
);

-- 8. Таблица Выставка
create table Exhibition (
    Id int identity primary key,
    Title varchar(200) not null,
    StartDate date not null,
    EndDate date not null
);

-- 9. Таблица Экспонат на выставке
create table ExhibitOnExhibition (
    Id int identity primary key,
    PlaceIdentifier varchar(50) not null,
    LabelData varchar(4000) not null
);

-- 10. Таблица Реставрационный заказ
create table RestorationOrderEntity (
    Id int identity primary key,
    OrderNumber varchar(50) not null unique,
    ReceiptDate date not null,
    ReasonDirection varchar(4000) not null,
    PlannedCompletionDate date not null
);

-- 11. Таблица Требуемые виды работ для заказа
create table RequiredWorkType (
    Id int identity primary key
);

-- 12. Таблица Журнал работ
create table WorkLogEntry (
    Id int identity primary key,
    ExecutionDate date not null
);

-- 13. Таблица Акт реставрации
create table RestorationAct (
    Id int identity primary key,
    CompletionDate date not null,
    FinalReport varchar(4000) not null,
    TotalCost float not null
);

-- 14. Таблица Акт возврата
create table ReturnAct (
    Id int identity primary key,
    ReturnDate date not null
);

-- 15. Таблица Акт списания
create table WriteOffAct (
    Id int identity primary key,
    WriteOffDate date not null,
    WriteOffReason varchar(4000) not null
);

-- 16. Таблица Билет
create table Ticket (
    Id int identity primary key,
    TicketNumber varchar(50) not null unique,
    SaleDateTime datetime2 not null,
    SalePrice float not null,
    VisitDate date not null
);

-- 17. Таблица Роли человека
create table PersonRole (
    Id int identity primary key
);

-- ======================================================================
-- ДОБАВЛЕНИЕ ПОЛЕЙ-ССЫЛОК И ВНЕШНИХ КЛЮЧЕЙ (исправленные каскады)
-- ======================================================================

-- Акт поступления (два FK на Person – NO ACTION, чтобы избежать множественных путей)
alter table ReceiptAct add
    SourceFk int not null,
    ReceiptMethodFk int not null,
    ResponsiblePersonFk int not null,
    constraint fk_ReceiptAct_Source foreign key (SourceFk) references Person(Id) ON DELETE NO ACTION,
    constraint fk_ReceiptAct_Method foreign key (ReceiptMethodFk) references ReceiptMethod(Id) ON DELETE CASCADE,
    constraint fk_ReceiptAct_Responsible foreign key (ResponsiblePersonFk) references Person(Id) ON DELETE NO ACTION;

-- Экспонат (при удалении акта удаляем экспонаты, при удалении статуса – запрещаем)
alter table Exhibit add
    ExhibitStatusFk int not null,
    ReceiptActFk int not null,
    constraint fk_Exhibit_Status foreign key (ExhibitStatusFk) references ExhibitStatus(Id) ON DELETE NO ACTION,
    constraint fk_Exhibit_ReceiptAct foreign key (ReceiptActFk) references ReceiptAct(Id) ON DELETE CASCADE;

-- Фото экспоната (при удалении экспоната удаляем фото, тип фото – запрещаем)
alter table ExhibitPhoto add
    PhotoTypeFk int not null,
    ExhibitFk int not null,
    constraint fk_ExhibitPhoto_Type foreign key (PhotoTypeFk) references PhotoType(Id) ON DELETE NO ACTION,
    constraint fk_ExhibitPhoto_Exhibit foreign key (ExhibitFk) references Exhibit(Id) ON DELETE CASCADE;

-- Связка экспонат-категория (при удалении экспоната удаляем связь, категорию – запрещаем)
alter table ExhibitCategory add
    ExhibitFk int not null,
    CategoryFk int not null,
    constraint uq_ExhibitCategory unique (ExhibitFk, CategoryFk),
    constraint fk_ExhibitCategory_Exhibit foreign key (ExhibitFk) references Exhibit(Id) ON DELETE CASCADE,
    constraint fk_ExhibitCategory_Category foreign key (CategoryFk) references Category(Id) ON DELETE NO ACTION;

-- Связка экспонат-материал
alter table ExhibitMaterial add
    ExhibitFk int not null,
    MaterialFk int not null,
    constraint uq_ExhibitMaterial unique (ExhibitFk, MaterialFk),
    constraint fk_ExhibitMaterial_Exhibit foreign key (ExhibitFk) references Exhibit(Id) ON DELETE CASCADE,
    constraint fk_ExhibitMaterial_Material foreign key (MaterialFk) references Material(Id) ON DELETE NO ACTION;

-- Связка экспонат-техника
alter table ExhibitTechnique add
    ExhibitFk int not null,
    TechniqueFk int not null,
    constraint uq_ExhibitTechnique unique (ExhibitFk, TechniqueFk),
    constraint fk_ExhibitTechnique_Exhibit foreign key (ExhibitFk) references Exhibit(Id) ON DELETE CASCADE,
    constraint fk_ExhibitTechnique_Technique foreign key (TechniqueFk) references Technique(Id) ON DELETE NO ACTION;

-- Зал (при удалении этажа удаляем залы)
alter table Hall add
    FloorFk int not null,
    constraint fk_Hall_Floor foreign key (FloorFk) references FloorEntity(Id) ON DELETE CASCADE;

-- Выставка (при удалении зала удаляем выставки; куратор и статус – запрет)
alter table Exhibition add
    HallFk int not null,
    ResponsibleCuratorFk int not null,
    ExhibitionStatusFk int not null,
    constraint fk_Exhibition_Hall foreign key (HallFk) references Hall(Id) ON DELETE CASCADE,
    constraint fk_Exhibition_Curator foreign key (ResponsibleCuratorFk) references Person(Id) ON DELETE NO ACTION,
    constraint fk_Exhibition_Status foreign key (ExhibitionStatusFk) references ExhibitionStatus(Id) ON DELETE NO ACTION;

-- Экспонат на выставке (при удалении выставки или экспоната удаляем связь)
alter table ExhibitOnExhibition add
    ExhibitionFk int not null,
    ExhibitFk int not null,
    ExpositionPlaceTypeFk int not null,
    constraint uq_ExhibitOnExhibition unique (ExhibitionFk, ExhibitFk),
    constraint fk_EOE_Exhibition foreign key (ExhibitionFk) references Exhibition(Id) ON DELETE CASCADE,
    constraint fk_EOE_Exhibit foreign key (ExhibitFk) references Exhibit(Id) ON DELETE CASCADE,
    constraint fk_EOE_PlaceType foreign key (ExpositionPlaceTypeFk) references ExpositionPlaceType(Id) ON DELETE NO ACTION;

-- Реставрационный заказ (только для Exhibit – каскад, для Person – запрет)
alter table RestorationOrderEntity add
    ExhibitFk int not null,
    RestorerFk int not null,
    InitiatorFk int not null,
    constraint fk_RestorationOrder_Exhibit foreign key (ExhibitFk) references Exhibit(Id) ON DELETE CASCADE,
    constraint fk_RestorationOrder_Restorer foreign key (RestorerFk) references Person(Id) ON DELETE NO ACTION,
    constraint fk_RestorationOrder_Initiator foreign key (InitiatorFk) references Person(Id) ON DELETE NO ACTION;

-- Требуемые виды работ (при удалении заказа удаляем требуемые виды)
alter table RequiredWorkType add
    RestorationOrderFk int not null,
    WorkTypeFk int not null,
    constraint uq_RequiredWorkType unique (RestorationOrderFk, WorkTypeFk),
    constraint fk_RequiredWorkType_Order foreign key (RestorationOrderFk) references RestorationOrderEntity(Id) ON DELETE CASCADE,
    constraint fk_RequiredWorkType_WorkType foreign key (WorkTypeFk) references RestorationWorkType(Id) ON DELETE NO ACTION;

-- Журнал работ (аналогично)
alter table WorkLogEntry add
    RestorationOrderFk int not null,
    WorkTypeFk int not null,
    constraint fk_WorkLogEntry_Order foreign key (RestorationOrderFk) references RestorationOrderEntity(Id) ON DELETE CASCADE,
    constraint fk_WorkLogEntry_WorkType foreign key (WorkTypeFk) references RestorationWorkType(Id) ON DELETE NO ACTION;

-- Акт реставрации (при удалении заказа удаляем акт)
alter table RestorationAct add
    RestorationOrderFk int not null,
    constraint fk_RestorationAct_Order foreign key (RestorationOrderFk) references RestorationOrderEntity(Id) ON DELETE CASCADE;

-- Акт возврата (при удалении заказа удаляем акт возврата)
alter table ReturnAct add
    RestorationOrderFk int not null,
    constraint fk_ReturnAct_Order foreign key (RestorationOrderFk) references RestorationOrderEntity(Id) ON DELETE CASCADE;

-- Акт списания (при удалении экспоната удаляем акт списания)
alter table WriteOffAct add
    ExhibitFk int not null,
    constraint fk_WriteOffAct_Exhibit foreign key (ExhibitFk) references Exhibit(Id) ON DELETE CASCADE;

-- Билет (при удалении выставки удаляем билеты, статус – запрет)
alter table Ticket add
    ExhibitionFk int not null,
    TicketStatusFk int not null,
    constraint fk_Ticket_Exhibition foreign key (ExhibitionFk) references Exhibition(Id) ON DELETE CASCADE,
    constraint fk_Ticket_Status foreign key (TicketStatusFk) references TicketStatus(Id) ON DELETE NO ACTION;

-- Роли человека (при удалении человека удаляем его роли, роль не удаляется)
alter table PersonRole add
    PersonFk int not null,
    RoleFk int not null,
    constraint uq_PersonRole unique (PersonFk, RoleFk),
    constraint fk_PersonRole_Person foreign key (PersonFk) references Person(Id) ON DELETE CASCADE,
    constraint fk_PersonRole_Role foreign key (RoleFk) references RoleEntity(Id) ON DELETE NO ACTION;

-- Тип лица (запрещаем удаление типа, если есть человек)
alter table Person add
    PersonTypeFk int not null,
    constraint fk_Person_Type foreign key (PersonTypeFk) references PersonType(Id) ON DELETE NO ACTION;

GO