USE бобор;
GO

-- Удаляем старые ограничения (если есть)
ALTER TABLE RestorationOrderEntity DROP CONSTRAINT fk_RestorationOrder_Exhibit;
GO

-- Создаём заново с каскадным удалением
ALTER TABLE RestorationOrderEntity
ADD CONSTRAINT fk_RestorationOrder_Exhibit
FOREIGN KEY (ExhibitFk) REFERENCES Exhibit(Id) ON DELETE CASCADE;
GO

-- Также рекомендуется для других связей, чтобы при удалении экспоната чистилось всё:
-- (если ещё не настроено)
ALTER TABLE RestorationOrderEntity DROP CONSTRAINT fk_RestorationOrder_Restorer;
ALTER TABLE RestorationOrderEntity ADD CONSTRAINT fk_RestorationOrder_Restorer
FOREIGN KEY (RestorerFk) REFERENCES Person(Id) ON DELETE NO ACTION; -- лучше NO ACTION

ALTER TABLE RestorationOrderEntity DROP CONSTRAINT fk_RestorationOrder_Initiator;
ALTER TABLE RestorationOrderEntity ADD CONSTRAINT fk_RestorationOrder_Initiator
FOREIGN KEY (InitiatorFk) REFERENCES Person(Id) ON DELETE NO ACTION;
GO