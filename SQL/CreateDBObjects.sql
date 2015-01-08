USE TestDB
GO

-- If we have foreign keys, we need to delete in reverse order.

IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'myschema' AND TABLE_NAME = 'Person'))
BEGIN
	DROP TABLE myschema.Person
    PRINT 'TABLE Person has been dropped.'  
END
GO
CREATE TABLE myschema.Person(
	Id			bigint IDENTITY(1,1) NOT NULL PRIMARY KEY,
	FirstName	nvarchar(100),
	LastName	nvarchar(100),
	Age         int,
	InsertDate	datetime2 NOT NULL DEFAULT getdate()	
)
PRINT 'TABLE Name has been created.'
GO

