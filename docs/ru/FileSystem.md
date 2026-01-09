### SnkUpdateMaster.FileSystem

**Назначение:** модуль, читающий метаданные и файлы обновлений из локальной файловой системы.

#### Предоставляемые реализации

- `FileSystemUpdateInfoProvider : IUpdateInfoProvider`
	Читает файл метаданных (например `C:\releases\manifest.json`) и парсит его с помощью `IUpdateInfoFileParser`.
- `FileSystemUpdateDownloader : IUpdateDownloader`
	Копирует файлы обновлений из локальной папки релизов в папку загрузок.
- `UpdateManagerBuilderFileSystemExtensions`
	Extension-методы для `UpdateManagerBuilder`:
	- `WithFileSystemUpdateInfoProvider(IUpdateInfoFileParser updateInfoFileParser, string updateFileInfoPath)`
	- `WithFileSystemUpdateDownloader(string downloadsDir)`

**Пример использования:**

```csharp
using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Files;
using SnkUpdateMaster.FileSystem;
using SnkUpdateMaster.FileSystem.Configuration;

var releasesDir = @"C:\releases";
var manifestPath = Path.Combine(releasesDir, "manifest.json");
var downloadsDir = @"C:\app\downloads";
var appDir = @"C:\app";

var updateManager = new UpdateManagerBuilder()
.WithFileCurrentVersionManager()
.WithSha256IntegrityVerifier()
.WithZipInstaller(appDir)
.WithFileSystemUpdateInfoProvider(new JsonUpdateInfoFileParser(), manifestPath)
.WithFileSystemUpdateDownloader(downloadsDir)
.Build();

var progress = new Progress<double>();
var updated = await updateManager.CheckAndInstallUpdatesAsync(progress);
```