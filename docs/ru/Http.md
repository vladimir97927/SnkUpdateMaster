### SnkUpdateMaster.Http

**Назначение:** модуль, который позволяет получать метаданные обновлений и архивы через HTTP/HTTPS.

#### Предоставляемые реализации

- `HttpUpdateInfoProvider : IUpdateInfoProvider`
	Скачивает JSON‑манифест (например, `/manifest.json`) и парсит его через `IUpdateInfoFileParser` (например `JsonUpdateInfoFileParser`).
- `HttpUpdateDownloader : IUpdateDownloader`
	Скачивает архив обновления по HTTP/HTTPS в локальную директорию загрузок.
- `UpdateManagerBuilderHttpExtensions`
	Extension‑методы для `UpdateManagerBuilder`:
	- `WithHttpUpdateInfoProvider(HttpClient httpClient, IUpdateInfoParser updateInfoParser, string updateInfoUrl)`.
	- `WithHttpUpdateDownloader(HttpClient httpClient, string downloadsDir)`.

**Пример использования:**

```csharp
using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Files;
using SnkUpdateMaster.Http;
using SnkUpdateMaster.Http.Configuration;

var httpClient = new HttpClient
{
	BaseAddress = new Uri("https://updates.example.com/")
};

var updateInfoFileParser = new JsonUpdateInfoFileParser();
var appDir = "app";
var downloadsDir = "downloads";

var updateManager = new UpdateManagerBuilder()
.WithFileCurrentVersionManager()
.WithSha256IntegrityVerifier()
.WithZipInstaller(appDir)
.WithHttpUpdateInfoProvider(updateInfoFileParser, httpClient, "manifest.json")
.WithHttpUpdateDownloader(httpClient, downloadsDir)
.Build();

var progress = new Progress<double>();
var updated = await updateManager.CheckAndInstallUpdatesAsync(progress);
```