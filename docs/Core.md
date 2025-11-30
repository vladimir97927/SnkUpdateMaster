### SnkUpdateMaster.Core

**Назначение:** Здесь сосредоточена бизнес‑логика, интерфейсы и базовые реализации.

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
        *   `Id` — идентификатор;
        *   `Version` — `System.Version`;
        *   `FileName`;
        *   `Checksum` (SHA‑256, hex);
        *   `ReleaseDate` (UTC);
        *   `FileDir` — каталог файла (для FTP / файловых сценариев).
    *   `UpdateManagerBuilder` (наследуется от `DependencyBuilder<UpdateManager>`)  
        Fluent‑билдер для конфигурации `UpdateManager`. Встроенные методы:
        *   `WithFileCurrentVersionManager()` — хранение текущей версии в файле `version` в `Environment.CurrentDirectory`;
        *   `WithSha256IntegrityVerifier()` — проверка целостности через SHA‑256;
        *   `WithZipInstaller(string appDir)` — установщик, который разворачивает ZIP‑архив поверх директории приложения `appDir` с созданием бэкапа.
        Источники обновлений и загрузчики подключаются либо напрямую через `AddDependency<T>`, либо через extension‑методы из модулей `Ftp` и `SqlServer`.
2.  **Управление версией (`VersionManager`)**
    Файлы:
    *   `VersionManager/ICurrentVersionManager.cs`
    *   `VersionManager/FileVersionManager.cs`
    *   `ICurrentVersionManager` — абстракция над хранением текущей версии приложения.
    *   `FileVersionManager` — реализация через обычный текстовый файл `version`:
        *   формат: `major.minor.build` (например, `1.0.2`);
        *   умеет читать и обновлять версию;
        *   автоматически создаёт файл при первом обновлении.
3.  **Провайдеры обновлений (`UpdateSource`)**
    Файл:
    *   `UpdateSource/IUpdateInfoProvider.cs`
    *   `IUpdateInfoProvider` — интерфейс для получения информации о последнем доступном обновлении из любого внешнего источника (FTP, БД, файловая система и т.п.).
    Конкретные реализации находятся в проектах `SnkUpdateMaster.Ftp` и `SnkUpdateMaster.SqlServer`.
4.  **Загрузчики обновлений (`Downloader`)**
    Файл:
    *   `Downloader/IUpdateDownloader.cs`
    *   `IUpdateDownloader` — интерфейс для асинхронной загрузки файла обновления (HTTP, FTP, БД, локальное копирование и т.п.).  
        Возвращает путь к локальному файлу.
    Реализации — в проектах `SnkUpdateMaster.Ftp` и `SnkUpdateMaster.SqlServer`.
5.  **Установщики обновлений (`Installer`)**
    Файлы:
    *   `Installer/IInstaller.cs`
    *   `Installer/ZipInstaller.cs`
    *   `IInstaller` — абстракция “как применить загруженный файл обновления”.
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
    *   `IChecksumCalculator` — расчёт контрольной суммы файла;
    *   `IIntegrityProvider` — “обёртка” над калькулятором, может выполнять дополнительные проверки;
    *   `IIntegrityVerifier` — сравнение вычисленной суммы с ожидаемой;
    *   `ShaChecksumCalculator` / `ShaIntegrityProvider` / `ShaIntegrityVerifier` — реализация через SHA‑256.
7.  **Парсинг файлов с описанием обновлений (`Files`)**
    Файлы:
    *   `Files/IUpdateInfoFileParser.cs`
    *   `Files/JsonUpdateInfoFileParser.cs`
    *   `IUpdateInfoFileParser` — абстракция над парсингом файла с метаданными обновления.
    *   `JsonUpdateInfoFileParser` — реализация для JSON‑файла.  
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
8.  **Публикация релизов (`ReleasePublisher`)**
    Файлы:
    *   `ReleasePublisher/Release.cs`
    *   `ReleasePublisher/ReleaseInfo.cs`
    *   `ReleasePublisher/IReleaseSource.cs`
    *   `ReleasePublisher/IReleaseInfoSource.cs`
    *   `ReleasePublisher/IReleaseSourceFactory.cs`
    *   `ReleasePublisher/ReleaseManager.cs`
    *   `ReleasePublisher/ReleaseManagerBuilder.cs`
    *   `ReleasePublisher/Packager/IReleasePackager.cs`
    *   `ReleasePublisher/Packager/ZipReleasePackager.cs`
    Ключевые типы:
    *   `Release` — агрегат “релиз приложения”: версия, имя файла, checksum, бинарные данные.
    *   `ReleaseInfo` — краткая информация о релизе (Id, строковая версия, дата выпуска).
    *   `IReleaseSource` — абстракция хранилища релизов (CRUD‑операции).
    *   `IReleaseInfoSource` — источник пагинированной информации о релизах.
    *   `IReleaseSourceFactory` — фабрика для создания `IReleaseSource`.
    *   `IReleasePackager` — упаковщик релиза из каталога приложения.
    *   `ZipReleasePackager`:
        *   собирает ZIP‑архив из указанной директории;
        *   считает SHA‑256 и создаёт `Release` с бинарными данными архива.
    *   `ReleaseManager`:
        *   через `IReleasePackager` собирает релиз из исходной директории;
        *   публикует релиз в `IReleaseSource`;
        *   предоставляет пагинированный список релизов (`GetReleaseInfosPagedAsync`).
        *   метод для публикации называется **`PulishReleaseAsync`** (опечатка в имени, важно учитывать при вызове).
    *   `ReleaseManagerBuilder`:
        *   использует `DependencyBuilder<ReleaseManager>`;
        *   метод `WithZipPackager(IntegrityProviderType integrityProviderType = IntegrityProviderType.Sha256)` регистрирует ZIP‑упаковщик;
        *   интеграция с SQL Server добавляется через extension‑методы модуля `SnkUpdateMaster.SqlServer`.
9.  **Общие утилиты (`Common`)**
    Файлы:
    *   `Common/DependencyBuilder.cs` — базовый класс билдера через словарь зависимостей.
    *   `Common/PagedData.cs` — модель для пагинации (данные страницы + метаданные: `PageNumber`, `PageSize`, `TotalCount`).
