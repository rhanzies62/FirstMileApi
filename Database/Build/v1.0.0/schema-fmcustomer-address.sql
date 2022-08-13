ALTER TABLE fmcustomer
DROP COLUMN ContactNumber

ALTER TABLE fmcustomer
ADD Address nvarchar(255) null

INSERT INTO FmLookUpType(EntityCode,Description,LookUpValue,Ordinal) VALUES('EQT','Activity',5,4)

ALTER TABLE fmequipment
ADD IsActive bit not null default 1