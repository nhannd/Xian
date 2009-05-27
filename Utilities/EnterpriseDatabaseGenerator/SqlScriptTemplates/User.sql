USE [master]
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE [name] = '++USERID++') CREATE LOGIN [++USERID++] WITH PASSWORD=N'++USERPASS++', DEFAULT_DATABASE=[++DB++], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
USE [++DB++]
CREATE USER [++USERID++] FOR LOGIN [++USERID++] WITH DEFAULT_SCHEMA=[dbo]
EXEC sp_addrolemember 'db_datareader', '++USERID++'
EXEC sp_addrolemember 'db_datawriter', '++USERID++'