# Core

Основной модуль работы с загрузкой и доставкой обновлений. Содержит базовые интерфейсы и реализации. 

## 📦 Основные компоненты

### 1. Управление версиями 

* `UpdateInfo`/`Release`/`ReleaseInfo`
  
  Хранение данных обновлений: версия, контрольная сумма, дата выпуска, файловые данные.
  
* `ICurrentVersionManager`

  Управление текущей версией приложения (например, через файл).
  
* `IUpdateSource`/`IReleaseSource`/`IReleaseInfoSource`

  Источники данных для получения и сохранения данных об обновлениях.
  
### 2. Безопасность и целостность

* `IIntegrityVerifier`/`IIntegrityProvider`

  Проверяет контрольные суммы для защиты от поврежденных или измененных файлов.
  
### 3. Упаковка и доставка

* `IReleasePackager`/`ZipReleasePackager`

  Упаковывает релизы в ZIP-архивы, сохраняя структуру файлов, создают метаданные.
  
* `IUpdateDownloader`
  
  Загружает обновления с поддержкой прогресса и отмены.
  
### 4. Установка и откат

* `IInstaller`

  Устанавливает обновления транзакционно с автоматическим бэкапом и откатом при ошибках.
  
### 5. Управление жизненным циклом

* `ReleaseManager`/`UpdateManager`

  Координирует процессы: проверка обновлений → загрузка → установка → обновление версии.
  
* `PagedData<T>`

  Возвращает пагинированные списки релизов для интеграции с UI.

## 🚀Быстрый старт

### Публикация релиза

```cs
 var releaseManager = new ReleaseManagerBuilder()
    .WithZipPackager(IntegrityProviderType.Sha256)
    .AddDependency<IReleaseSource>(new CustomReleaseSource())
    .AddDependency<IReleaseInfoSource>(new CustomReleaseInfoSource())
    .Build();

var releaseId = await releaseManager.PulishReleaseAsync(
    appDir: "app/",
    version: new Version(1, 0, 0),
    progress: new Progress<double>(p => Console.WriteLine($"Упаковка: {p:P}")),
    cancellationToken: default);
```

### Проверка и установка обновлений

```cs
var updateManager = new UpdateManagerBuilder()
    .WithFileCurrentVersionManager()
    .WithSha256IntegrityVerifier()
    .WithZipInstaller(appDir: "app/")
    .AddDependency<IUpdateSource>(new CustomUpdateSource())
    .AddDependency<IUpdateDownloader>(new CustomUpdateDownloader)
    .Build();

var isSuccessful = await updateManager.CheckAndInstallUpdatesAsync(
    progress: new Progress<double>(p => UpdateUI(p)),
    cancellationToken: default);
```

## 📚 Реализации

### Готовые компоненты
| Интерфейс | Реализация | Описание |
|-----------|------------|----------|
| `ICurrentVersionManager` | **`FileVersionManager`** | Хранит версию в текстовом файле |
| `IInstaller` | **`ZipInstaller`** | Распаковывает ZIP с откатом при ошибках |
| `IIntegrityVerifier` | **`ShaIntegrityVerifier`** | Валидация через SHA-256 |
| `IReleasePackager` | **`ZipReleasePackager`** | Упаковка в ZIP + генерация метаданных |

### Кастомные реализации

Модуль требует пользовательской реализации интерфейсов: 
-`IUpdateSource`
-`IUpdateDownloader`
-`IReleaseSource`
-`IReleaseInfoSource`

В проекте интерфейсы реализованы в отдельных модулях:
- [SqlServer]()

