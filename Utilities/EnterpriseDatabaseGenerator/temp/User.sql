USE [master]
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE [name] = 'entuser') CREATE LOGIN [entuser] WITH PASSWORD=N'entpass', DEFAULT_DATABASE=[Enterprise1], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
USE [Enterprise1]
CREATE USER [entuser] FOR LOGIN [entuser] WITH DEFAULT_SCHEMA=[dbo]
EXEC sp_addrolemember 'db_datareader', 'entuser'
EXEC sp_addrolemember 'db_datawriter', 'entuser'