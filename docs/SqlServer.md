### SnkUpdateMaster.SqlServer

Набор расширений, которые позволяют получать метаданные обновлений и сами файлы напрямую из Microsoft SQL Server.

**Назначение:** интеграция с Microsoft SQL Server для получения и загрузки обновлений из таблиц `UpdateInfo` / `UpdateFile`.

#### Предоставляемые реализации

*   `SqlServerUpdateInfoProvider : IUpdateInfoProvider`  
    Использует Dapper и `ISqlConnectionFactory`, читает последнюю запись из `[dbo].[UpdateInfo]` по `ReleaseDate DESC`.
*   `SqlServerUpdateDownloader : IUpdateDownloader`  
    Читает BLOB из `[dbo].[UpdateFile]` по `UpdateInfoId`, сохраняет в указанную директорию (`downloadsDir`).
*   `UpdateManagerBuilderSqlServerExtensions`:
    *   `WithSqlServerUpdateInfoProvider(ISqlConnectionFactory sqlConnectionFactory)`
    *   `WithSqlServerUpdateDownloader(ISqlConnectionFactory sqlConnectionFactory, string downloadsDir)`

**Пример использования:**

Предварительно необходимо создать таблицы в БД:
```sql
CREATE TABLE [dbo].[UpdateInfo]
(
	[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Version] NVARCHAR(256) NOT NULL,
	[FileName] NVARCHAR(256) NOT NULL,
	[FileDir] NVARCHAR(256) NULL, --Может быть пустым при хранении файлов обновления в таблице [UpdateFile]
	[CheckSum] NVARCHAR(256) NOT NULL,
	[ReleaseDate] DATETIME NOT NULL,
)

CREATE TABLE [dbo].[UpdateFile]
(
	[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[UpdateInfoId] INT NOT NULL,
	[FileData] VARBINARY(MAX) NOT NULL
)
```

Создание менеджера обновлений:
```csharp
using SnkUpdateMaster.Core;
using SnkUpdateMaster.SqlServer.Configuration;
using SnkUpdateMaster.SqlServer.Database;

var connectionString = "Server=localhost,1455;Database=SnkUpdateMasterDb;User Id=sa;Password=Snk@12345;Encrypt=False;TrustServerCertificate=True";
var sqlConnectionFactory = new SqlConnectionFactory(connectionString);
var appDir = "app";
var downloadsDir = "downloads";

var updateManager = new UpdateManagerBuilder()
    .WithFileCurrentVersionManager()
    .WithSha256IntegrityVerifier()
    .WithZipInstaller(appDir)
    .WithSqlServerUpdateInfoProvider(sqlConnectionFactory)
    .WithSqlServerUpdateDownloader(sqlConnectionFactory, downloadsDir)
    .Build();
```

> 📌 Если файлы обновлений хранятся в таблице `UpdateFile`, указывайте пустой `FileDir` в `UpdateInfo` - загрузчик сам выберет правильный источник.