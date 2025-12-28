### SnkUpdateMaster.SqlServer

Extensions that fetch update metadata and files directly from Microsoft SQL Server.

**Purpose:** integrate with SQL Server to read updates from the `UpdateInfo` / `UpdateFile` tables.

#### Provided implementations

* `SqlServerUpdateInfoProvider : IUpdateInfoProvider`  
  Uses Dapper and `ISqlConnectionFactory`; selects the latest `[dbo].[UpdateInfo]` row ordered by `ReleaseDate DESC`.
* `SqlServerUpdateDownloader : IUpdateDownloader`  
  Reads BLOBs from `[dbo].[UpdateFile]` by `UpdateInfoId` and saves them to the specified `downloadsDir`.
* `UpdateManagerBuilderSqlServerExtensions`:
  * `WithSqlServerUpdateInfoProvider(ISqlConnectionFactory sqlConnectionFactory)`
  * `WithSqlServerUpdateDownloader(ISqlConnectionFactory sqlConnectionFactory, string downloadsDir)`

**Usage example:**

Create tables first:
```sql
CREATE TABLE [dbo].[UpdateInfo]
(
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Version] NVARCHAR(256) NOT NULL,
    [FileName] NVARCHAR(256) NOT NULL,
    [FileDir] NVARCHAR(256) NULL, -- Can be empty when files are stored in [UpdateFile]
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

Build the update manager:
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

> 📌 If update archives are stored in `UpdateFile`, leave `FileDir` empty in `UpdateInfo`- the downloader will pick the right source automatically.