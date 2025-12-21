CREATE DATABASE [SnkUpdateMasterDb]
GO

IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'SnkUpdateMasterDb')
    BEGIN
        ALTER DATABASE [SnkUpdateMasterDb]
            SET AUTO_CLOSE OFF 
            WITH ROLLBACK IMMEDIATE;
    END
GO