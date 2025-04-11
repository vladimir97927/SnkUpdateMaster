# SnkUpdateMaster
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

## Особенности проекта
- Язык программирования: C#.
- Платформа: .NET 9.0.
- Entity Framework Core, Dapper для работы с реляционной базой данных.

## Модули проекта
- [Core - основные компоненты работы приложения.]()
- [SqlServer - реализация работы с обновлениями через реляционную базу данных.]()

## Использование
1. Создание менеджера обновлений со следующим функционалом:
- Хранение версии приложения в файле.
- Использование алгоритма SHA256 для вычисления хэша и обеспечения целостности загруженных файлов обновлений.
- Установка обновлений из ZIP архива.
- Хранение обновлений в базе данных SqlServer.

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

2. Загрузка ZIP архива с обновлениями в базу данных Sql Server. Алгоритм хэширования SHA256.

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

3. Постраничный вывод загруженных обновлений в базе данных Sql Server.

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















