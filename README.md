# SnkUpdateMaster

[![WIP](https://img.shields.io/badge/Status-Work%20In%20Progress-orange)](https://github.com/vladimir97927/SnkUpdateMaster)

> **⚠️ Внимание**: Проект в разработке. API и структура могут меняться.

Проект является универсальной библиотекой для управления обновлениями приложений.

**Основная идея**

Проект представляет собой модульную библиотеку, предназначенную для управления процессом обновления настольных приложений. Ключевой особенностью является поддержка кастомизации на всех этапах работы: от получения метаданных до установки новых версий, что позволяет адаптировать решение под уникальные требования инфраструктуры.

**Ключевые возможности**

1. Выбор источника обновлений
   - HTTP/S-сервер.
   - FTP/SFTP-сервер.
   - Реляционные базы данных.
   - GitHub Releases.
2. Выбор установщика
   - Архивы.
   - Исполняемые файлы.
   - Скрипты.
3. Контроль целостности данных. Выбор алгоритма хэширования

## Зависимости проекта
- Язык программирования: C# 12.0.
- Платформа: .NET 8.0.
- Microsoft.EntityFrameworkCore 9.0.2.
- Microsoft.EntityFrameworkCore.Relational 9.0.2.
- Microsoft.EntityFrameworkCore.SqlServer 9.0.2.
- Microsoft.Data.SqlClient 6.0.1.
- Dapper 2.1.66.

## Модули проекта
- [Core - основные компоненты работы приложения.](docs/README-CORE.md)
- [SqlServer - реализация работы с обновлениями через реляционную базу данных.]()
- [SnkUpdateMasterDb - проект базы данных.]()

## Использование SQL Server
1. Создание менеджера обновлений со следующим функционалом:
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

Для проверки наличия обновлений и установки используется класс UpdateManager. Экземпляр класса создается через UpdateManagerBuilder.
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

2. Загрузка ZIP архива с обновлениями в базу данных SQL Server. Алгоритм хэширования SHA256.

Для публикации обновлений используется класс ReleaseManager. Экземпляр класса создается через ReleaseManagerBuilder.
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

3. Постраничный вывод загруженных обновлений в базе данных SQL Server.

Для получения информации об обновлениях используется класс SqlServerReleaseInfoSource. Класс требует зависимости от ISqlConnectionFactory, который предоставляет функции для подключения к БД.

```csharp
 string connectionString; // Строка подключения к БД.
 int currentPage = 1; // Отображаемая страница.
 int pageSize = 20; // Число элементов на странице.

var sqlConnectionFactory = new SqlConnectionFactory(connectionString);
var releaseInfoSource = new SqlServerReleaseInfoSource(sqlConnectionFactory);

var releases = await releaseInfoSource.GetReleaseInfosPagedAsync(currentPage, pageSize);
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
## Приложения

**ReleasePublisher.CLI**

Программа, использующая разработанную библиотеку для загрузки новых и просмотра существующих ZIP архивов с обновлениями в БД SQL Server.

Для работы необходимо переопределить переменную ``connectionString`` в классе ``Program``.

Для установки приложения клонируйте репозиторий:

```
git clone https://github.com/vladimir97927/SnkUpdateMaster.git
```

Переопределите переменную ``connectionString`` в классе ``Program``.

Соберите проект из папки решения

```
dotnet publish src/SnkUpdateMaster.ReleasePublisher.CLI/SnkUpdateMaster.ReleasePublisher.CLI.csproj --artifacts-path build-release-publisher-cli
```

## Установка








