use бобор;
GO

-- 1. Удаляем все ограничения внешних ключей
DECLARE @sql NVARCHAR(MAX) = N'';
SELECT @sql += N'ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(parent_object_id)) + N'.' + QUOTENAME(OBJECT_NAME(parent_object_id)) 
    + N' DROP CONSTRAINT ' + QUOTENAME(name) + N';' + CHAR(13)
FROM sys.foreign_keys;
EXEC sp_executesql @sql;
GO

-- 2. Удаляем таблицы в порядке зависимостей
DROP TABLE IF EXISTS WorkLogEntry;
DROP TABLE IF EXISTS RequiredWorkType;
DROP TABLE IF EXISTS RestorationAct;
DROP TABLE IF EXISTS ReturnAct;
DROP TABLE IF EXISTS WriteOffAct;
DROP TABLE IF EXISTS Ticket;
DROP TABLE IF EXISTS PersonRole;
DROP TABLE IF EXISTS ExhibitPhoto;
DROP TABLE IF EXISTS ExhibitOnExhibition;
DROP TABLE IF EXISTS RestorationOrderEntity;
DROP TABLE IF EXISTS Exhibition;
DROP TABLE IF EXISTS Hall;
DROP TABLE IF EXISTS Exhibit;
DROP TABLE IF EXISTS ReceiptAct;
DROP TABLE IF EXISTS Person;
DROP TABLE IF EXISTS ExhibitCategory;
DROP TABLE IF EXISTS ExhibitMaterial;
DROP TABLE IF EXISTS ExhibitTechnique;
DROP TABLE IF EXISTS Category;
DROP TABLE IF EXISTS Material;
DROP TABLE IF EXISTS Technique;
DROP TABLE IF EXISTS ExhibitStatus;
DROP TABLE IF EXISTS ReceiptMethod;
DROP TABLE IF EXISTS PhotoType;
DROP TABLE IF EXISTS ExpositionPlaceType;
DROP TABLE IF EXISTS ExhibitionStatus;
DROP TABLE IF EXISTS RestorationWorkType;
DROP TABLE IF EXISTS TicketStatus;
DROP TABLE IF EXISTS RoleEntity;
DROP TABLE IF EXISTS PersonType;
DROP TABLE IF EXISTS FloorEntity;
GO