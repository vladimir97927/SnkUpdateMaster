### SnkUpdateMaster.SqlServer

**Назначение:** интеграция с Microsoft SQL Server для:

*   получения и загрузки обновлений из таблиц `UpdateInfo` / `UpdateFile`;
*   конфигурации билдера через extension‑методы.

Файлы (основные):

*   `Configuration/UpdateManagerBuilderSqlServerExtensions.cs`
*   `Configuration/ReleaseManagerBuilderSqlServerExtensions.cs`
*   `Database/ISqlConnectionFactory.cs`
*   `Database/SqlConnectionFactory.cs`
*   `Database/SnkUpdateMasterContext.cs`
*   `Database/ReleaseEntityTypeConfiguration.cs`
*   `Pagination/PageData.cs`
*   `Pagination/PagedQueryHelper.cs`
*   `SqlServerUpdateInfoProvider.cs`
*   `SqlServerUpdateDownloader.cs`
*   `SqlServerReleaseInfoSource.cs`
*   `SqlServerReleaseSource.cs`
*   `SqlServerReleaseSourceFactory.cs`

#### Обновления через SQL Server

*   `SqlServerUpdateInfoProvider : IUpdateInfoProvider`  
    Использует Dapper и `ISqlConnectionFactory`, читает последнюю запись из `[dbo].[UpdateInfo]` по `ReleaseDate DESC`.
*   `SqlServerUpdateDownloader : IUpdateDownloader`  
    Читает BLOB из `[dbo].[UpdateFile]` по `UpdateInfoId`, сохраняет в указанную директорию (`downloadsDir`), поддерживает прогресс.
*   `UpdateManagerBuilderSqlServerExtensions`:
    *   `WithSqlServerUpdateInfoProvider(ISqlConnectionFactory sqlConnectionFactory)`
    *   `WithSqlServerUpdateDownloader(ISqlConnectionFactory sqlConnectionFactory, string downloadsDir)`

**Пример конфигурации:**

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
