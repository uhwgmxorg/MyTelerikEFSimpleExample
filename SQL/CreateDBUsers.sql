use TestDB
GO

IF NOT EXISTS(SELECT name FROM SYS.SCHEMAS WHERE name = 'myschema')
BEGIN
	Exec('CREATE SCHEMA myschema')
END ELSE BEGIN  
  PRINT 'SCHEMA myschema already exists to current databas.'  
END
GO

IF NOT EXISTS(SELECT name FROM MASTER.DBO.SYSLOGINS WHERE name = 'mylogin')
BEGIN
    CREATE LOGIN mylogin WITH  PASSWORD = 'password', DEFAULT_DATABASE  = TestDB, CHECK_POLICY = OFF
END ELSE BEGIN  
  PRINT 'LOGIN mylogin already exists to current server.'  
END
GO

IF NOT EXISTS(SELECT name FROM DBO.SYSUSERS WHERE name = 'myuser')
BEGIN
	CREATE USER myuser FOR LOGIN mylogin WITH DEFAULT_SCHEMA = myschema;
END ELSE BEGIN  
  PRINT 'USER myuser already granted access to current database.'  
END
GO

ALTER AUTHORIZATION ON SCHEMA::[myschema] TO [myuser]
GO

ALTER AUTHORIZATION ON SCHEMA::[db_owner] TO [myuser]
GO
