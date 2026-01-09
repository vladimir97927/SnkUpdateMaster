### SnkUpdateMaster.Core

**Назначение:** базовый модуль, в котором сосредоточена бизнес‑логика, интерфейсы и готовые реализации для построения конвейера обновления.

#### Основной вариант использования

**Для работы модуль требует реализации интерфейсов:**
- `IUpdateInfoProvider` - получение информации об обновлениях из внешнего источника.
- `IUpdateDownloader` - загрузка файлов обновлений из внешнего источника.

**Предоставлены реализации интерфейсов:**
- `IInstaller` - установка загруженных обновлений:
	- `ZipInstaller` - распаковка файлов обновлений из ZIP архива. Используется путем вызова метода `WithZipInstaller`.
- `IIntegrityVerifier` - проверка целостности файлов путем сравнения контрольных сумм:
	- `ShaIntegrityVerifier` - проверка целостности файлов с использованием алгоритма SHA-256. Используется путем вызова метода `WithSha256IntegrityVerifier`.
- `ICurrentVersionManager` - управление данными о текущей установленной версии приложения:
	- `FileVersionManager` - хранение версии в текстовом файле. Формат версии `major.minor.build`. Используется путем вызова метода `WithFileCurrentVersionManager`.

Для загрузки и установки обновлений используется класс `UpdateManager`. Создать экземпляр класса можно через `UpdateManagerBuilder`:
```csharp
var updateManager = new UpdateManagerBuilder()
	.WithZipInstaller("path to app folder")
	.WithFileCurrentVersionManager()
	.WithSha256IntegrityVerifier()
	.RegisterInstance<IUpdateInfoProvider>(customProviderInstance)
	.RegisterFactory<IUpdateDownloader>(customDownloaderFactory)
	.Build();
```

Методы `RegisterInstance<T>` и `RegisterFactory<T>` позволяют добавлять необходимые зависимости в билдер как готовый объект или как фабричный метод.

Проверка наличия обновлений и установка:
```csharp
var updated = await updateManager.CheckAndInstallUpdatesAsync(progress);
```

#### Логирование

По умолчанию, если `ILoggerFactory` не зарегистрирован, используется `NullLogger` и сообщения не выводятся. Чтобы подключить логирование, зарегистрируйте фабрику логгеров путем вызова метода `WithLogger` в билдере:
```csharp
var updateManager = new UpdateManagerBuilder()
.WithLogger(loggerFactory)
.WithZipInstaller("path to app folder")
.WithFileCurrentVersionManager()
.WithSha256IntegrityVerifier()
.RegisterInstance<IUpdateInfoProvider>(customProviderInstance)
.RegisterFactory<IUpdateDownloader>(customDownloaderFactory)
.Build();
```

Эта фабрика используется для создания `ILogger` в `UpdateManager`, а также в реализациях `IUpdateInfoProvider` и `IUpdateDownloader` из других модулей, например, `Ftp` и `SqlServer`.