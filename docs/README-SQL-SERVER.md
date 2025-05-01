# SqlServer
Загрузка и доставка обновлений средствами SQL Server.

## 🔧 Требования

* Язык программирования: C# 12.0
* Платформа: .NET 8.0
* Microsoft.EntityFrameworkCore 9.0.2
* Microsoft.EntityFrameworkCore.Relational 9.0.2
* Microsoft.EntityFrameworkCore.SqlServer 9.0.2
* Microsoft.Data.SqlClient 6.0.1
* Dapper 2.1.66

## 📦 Основные компоненты

### Расширения для билдеров

**Для UpdateManagerBuilder**

```cs
.WithSqlServerUpdateProvider(
    connectionString: string,      // Строка подключения к SQL
    downloadsDir: string)          // Папка для загруженных файлов
```

Регистрирует:
* `IUpdateSource` - получение метаданных из SQL
* `IUpdateDownloader` - загрузка BLOB-данных

**Для ReleaseManagerBuilder**

```cs
.WithSqlServerReleaseSource(
    connectionString: string)      // Строка подключения к SQL
```

Регистрирует:
* `IReleaseSource` - CRUD операций с релизами
* `IReleaseInfoSource` - постраничный список версий

## 🚀 Быстрый старт

### Подготовка базы данных

```tsql
CREATE TABLE [dbo].[AppUpdates]
(
	[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Version] NVARCHAR(256) NOT NULL,
	[FileName] NVARCHAR(256) NOT NULL,
	[Checksum] NVARCHAR(256) NOT NULL,
	[ReleaseDate] DATETIME NOT NULL,
	[FileData] VARBINARY(MAX) NOT NULL
)
```
### Публикация релиза

```csharp
string appDir; // Путь до папки с обновлениями.
string connectionString; // Строка подключения к БД.

var manager = new ReleaseManagerBuilder()
    .WithZipPackager(IntegrityProviderType.Sha256)
    .WithSqlServerReleaseSource(connectionString)
    .Build();
var newVersion = new Version(1, 1, 3);

var progress = new Progress<double>();
await manager.PulishReleaseAsync(appDir, newVersion, progress);
```

### Проверка и установка обновлений

```csharp
string appDir; // Путь до папки с обновляемым приложением.
string downloadsPath; // Путь, по которому хранятся скачанные обновления.
string connectionString; // Строка подключения к БД. 

var updateManager = new UpdateManagerBuilder()
    .WithFileCurrentVersionManager()
    .WithSha256IntegrityVerifier()
    .WithZipInstaller(appDir)
    .WithSqlServerUpdateProvider(connectionString, downloadsPath)
    .Build();

var progress = new Progress<double>();
var isSuccess = await updateManager.CheckAndInstallUpdatesAsync(progress);
```

### Просмотр истории релизов

```csharp
 int currentPage = 1; // Отображаемая страница.
 int pageSize = 20; // Число элементов на странице.

var releases = await manager.GetReleaseInfosPagedAsync(currentPage, pageSize);
```
