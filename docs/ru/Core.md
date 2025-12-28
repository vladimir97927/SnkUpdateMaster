### SnkUpdateMaster.Core

**Назначение:** базовый модуль, в котором сосредоточена бизнес‑логика, интерфейсы и готовые реализации для построения конвейера обновления.

#### Основной вариант использования

**Для работы модуль требует реализации интерфейсов:**
- `IUpdateInfoProvider` - получение информации об обновлениях из внешнего источника.
- `IUpdateDownloader` - загрузка файлов обновлений из внешнего источника.

**Предоставлены реализации интерфейсов:**
- `IInstaller` - установка загруженных обновлений:
	- `ZipInstaller` - распаковка файлов обновлений из ZIP архива.
- `IIntegrityVerifier` - проверка целостности файлов путем сравнения контрольных сумм:
	- `ShaIntegrityVerifier` - проверка целостности файлов с использованием алгоритма SHA-256.
- `ICurrentVersionManager` - управление данными о текущей установленной версии приложения:
	- `FileVersionManager` - хранение версии в текстовом файле. Формат версии `major.minor.build`.

Для загрузки и установки обновлений используется класс `UpdateManager`. Создать экземпляр класса можно через `UpdateManagerBuilder`:
```csharp
var updateManager = new UpdateManagerBuilder()
	.WithZipInstaller("path to app folder")
	.WithFileCurrentVersionManager()
	.WithSha256IntegrityVerifier()
	.AddDependency<IUpdateInfoProvider>(customImplementation)
	.AddDependency<IUpdateDownloader>(customImplementation)
	.Build();
```

Проверка наличия обновлений и установка:
```csharp
var updated = await updateManager.CheckAndInstallUpdatesAsync(progress);
```
#### Основные подсистемы

1.  **Обновление приложения (`UpdateManager`)**
    Файлы:
    *   `UpdateManager.cs`
    *   `UpdateManagerBuilder.cs`
    *   `UpdateInfo.cs`
    Ключевые типы:
    *   `UpdateManager`  
        Основной класс, который реализует полный цикл обновления:
        1.  получение текущей версии приложения (`ICurrentVersionManager`);
        2.  получение информации о доступном обновлении (`IUpdateInfoProvider`);
        3.  загрузка файла обновления (`IUpdateDownloader`);
        4.  проверка контрольной суммы (`IIntegrityVerifier`);
        5.  установка обновления (`IInstaller`);
        6.  обновление текущей версии (`ICurrentVersionManager.UpdateCurrentVersionAsync`).
        Основной метод:
        ```csharp
        Task<bool> CheckAndInstallUpdatesAsync(
            IProgress<double> progress,
            CancellationToken cancellationToken = default);
        ```
        Возвращает `true`, если обновление было найдено и установлено.
    *   `UpdateInfo`  
        Класс с метаданными обновления:
        *   `Id` - идентификатор;
        *   `Version` - `System.Version`;
        *   `FileName`;
        *   `Checksum` (SHA‑256, hex);
        *   `ReleaseDate` (UTC);
        *   `FileDir` - каталог файла (для FTP / файловых сценариев).
    *   `UpdateManagerBuilder` (наследуется от `DependencyBuilder<UpdateManager>`)  
        Fluent‑билдер для конфигурации `UpdateManager`. Встроенные методы:
        *   `WithFileCurrentVersionManager()` - хранение текущей версии в файле `version` в `Environment.CurrentDirectory`;
        *   `WithSha256IntegrityVerifier()` - проверка целостности через SHA‑256;
        *   `WithZipInstaller(string appDir)` - установщик, который разворачивает ZIP‑архив поверх директории приложения `appDir` с созданием бэкапа.
        Источники обновлений и загрузчики подключаются либо напрямую через `AddDependency<T>`, либо через extension‑методы из модулей `Ftp` и `SqlServer`.
2.  **Управление версией (`VersionManager`)**
    Файлы:
    *   `VersionManager/ICurrentVersionManager.cs`
    *   `VersionManager/FileVersionManager.cs`
    *   `ICurrentVersionManager` - абстракция над хранением текущей версии приложения.
    *   `FileVersionManager` - реализация через обычный текстовый файл `version`:
        *   формат: `major.minor.build` (например, `1.0.2`);
        *   умеет читать и обновлять версию;
        *   автоматически создаёт файл при первом обновлении.
3.  **Провайдеры обновлений (`UpdateSource`)**
    Файл:
    *   `UpdateSource/IUpdateInfoProvider.cs`
    *   `IUpdateInfoProvider` - интерфейс для получения информации о последнем доступном обновлении из любого внешнего источника (FTP, БД, файловая система и т.п.).
    Конкретные реализации находятся в проектах `SnkUpdateMaster.Ftp` и `SnkUpdateMaster.SqlServer`.
4.  **Загрузчики обновлений (`Downloader`)**
    Файл:
    *   `Downloader/IUpdateDownloader.cs`
    *   `IUpdateDownloader` - интерфейс для асинхронной загрузки файла обновления (HTTP, FTP, БД, локальное копирование и т.п.).  
        Возвращает путь к локальному файлу.
    Реализации - в проектах `SnkUpdateMaster.Ftp` и `SnkUpdateMaster.SqlServer`.
5.  **Установщики обновлений (`Installer`)**
    Файлы:
    *   `Installer/IInstaller.cs`
    *   `Installer/ZipInstaller.cs`
    *   `IInstaller` - абстракция “как применить загруженный файл обновления”.
    *   `ZipInstaller`:
        *   создаёт полную резервную копию директории приложения;
        *   распаковывает ZIP‑архив поверх приложения;
        *   при ошибке пытается откатиться, удаляя новую версию и возвращая бэкап.
    > ⚠️ Важно: `ZipInstaller` активно манипулирует файловой системой (удаляет/перемещает директории). Использовать только когда приложение остановлено.
6.  **Целостность и контрольные суммы (`Integrity`)**
    Файлы:
    *   `Integrity/IChecksumCalculator.cs`
    *   `Integrity/IIntegrityProvider.cs`
    *   `Integrity/IIntegrityVerifier.cs`
    *   `Integrity/ShaChecksumCalculator.cs`
    *   `Integrity/ShaIntegrityProvider.cs`
    *   `Integrity/ShaIntegrityVerifier.cs`
    *   `Integrity/IntegrityProviderType.cs`
    Основные типы:
    *   `IChecksumCalculator` - расчёт контрольной суммы файла;
    *   `IIntegrityProvider` - “обёртка” над калькулятором, может выполнять дополнительные проверки;
    *   `IIntegrityVerifier` - сравнение вычисленной суммы с ожидаемой;
    *   `ShaChecksumCalculator` / `ShaIntegrityProvider` / `ShaIntegrityVerifier` - реализация через SHA‑256.
7.  **Парсинг файлов с описанием обновлений (`Files`)**
    Файлы:
    *   `Files/IUpdateInfoFileParser.cs`
    *   `Files/JsonUpdateInfoFileParser.cs`
    *   `IUpdateInfoFileParser` - абстракция над парсингом файла с метаданными обновления.
    *   `JsonUpdateInfoFileParser` - реализация для JSON‑файла.  
        Ожидаемый формат (пример `tests/ftp-data/manifest.json`):
        ```json
        {
          "id": 1,
          "version": "1.0.1",
          "fileName": "release-1.0.1.zip",
          "fileDir": "/",
          "checksum": "…sha256…",
          "releaseDate": "2025-11-29"
        }
        ```