# SnkUpdateMaster.SqlServer
Загрузка и доставка обновлений средствами SQL Server.

## Скачивание обновлений
Модуль предоставляет реализацию интерфейсов `IUpdateSource` и `IUpdateDownloader`

**Использование**

Создание менеджера обновлений со следующим функционалом:
- Хранение версии приложения в файле.
- Использование алгоритма SHA256 для вычисления хэша и обеспечения целостности загруженных файлов обновлений.
- Установка обновлений из ZIP архива.
- Хранение обновлений в базе данных SqlServer.

Создайте таблицу в БД:

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

Для проверки наличия обновлений и установки используется класс `UpdateManager`. Экземпляр класса создается через `UpdateManagerBuilder`.
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
Модуль предоставляет расширение для класса `UpdateManagerBuilder`. Добавлен метод `WithSqlServerUpdateProvider(string connectionString, string downloadsDir)`.
## Публикация обновлений

Модуль предоставляет реализацию интерфейсов `IReleaseSource` и `IReleaseInfoSource`

**Использование**

Загрузка ZIP архива с обновлениями в базу данных SQL Server. Алгоритм хэширования SHA256.

Для публикации обновлений используется класс `ReleaseManager`. Экземпляр класса создается через `ReleaseManagerBuilder`.
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
Модуль предоставляет расширение для класса `ReleaseManagerBuilder`. Добавлен метод `WithSqlServerReleaseSource(string connectionString)`.

Постраничный вывод загруженных обновлений в базе данных SQL Server.

Для получения информации об обновлениях используется метод `GetReleaseInfosPagedAsync` класса `ReleaseManager`

```csharp
 int currentPage = 1; // Отображаемая страница.
 int pageSize = 20; // Число элементов на странице.

var releases = await manager.GetReleaseInfosPagedAsync(currentPage, pageSize);
```
Метод `GetReleaseInfosPagedAsync` возвращает тип данных `PagedData`

```csharp
public class PagedData<T>
{
    public PagedData(T data, int? pageNumber, int? pageSize, int totalCount)
    {
        Data = data;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public T Data { get; } // Данные.

    public int? PageNumber { get; } // Отображаемая страница.

    public int? PageSize { get; } // Число элементов на странице.

    public int TotalCount { get; } // Число всех элементов на сервере.
}
```
