# SnkUpdateMasterDb

Проект SQL Server базы данных для хранения обновлений приложения.

## 🔧 Требования

* SQL Server 2012

## 📃 Скрипты

1. Создание базы данных

```tsql
USE [master];
GO

IF (DB_ID(N'$(DatabaseName)') IS NOT NULL) 
BEGIN
    ALTER DATABASE [$(DatabaseName)]
    SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [$(DatabaseName)];
END
GO

CREATE DATABASE [$(DatabaseName)]
```

2. Добавление таблиц в базу

```tsql
USE [$(DatabaseName)]
GO

CREATE TABLE [dbo].[AppUpdates] (
    [Id]          INT             IDENTITY (1, 1) NOT NULL,
    [Version]     NVARCHAR (256)  NOT NULL,
    [FileName]    NVARCHAR (256)  NOT NULL,
    [Checksum]    NVARCHAR (256)  NOT NULL,
    [ReleaseDate] DATETIME        NOT NULL,
    [FileData]    VARBINARY (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
```
















