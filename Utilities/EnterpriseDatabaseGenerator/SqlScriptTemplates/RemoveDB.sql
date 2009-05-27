USE [master]
IF  EXISTS (SELECT name FROM sys.databases WHERE name = N'++DB++') DROP DATABASE [++DB++]
