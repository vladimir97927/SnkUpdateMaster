### SnkUpdateMaster.Http

**Purpose:** A module that allows you to receive update metadata and archives via HTTP/HTTPS.

### Available implementations

- `HttpUpdateInfoProvider : IUpdateInfoProvider`
	Downloads a JSON manifest (e.g. `/manifest.json`) and parses it via `IUpdateInfoFileParser` (e.g. `JsonUpdateInfoFileParser`).
- `HttpUpdateDownloader : IUpdateDownloader`
	Downloads the update archive via HTTP/HTTPS to the local download directory.
- `UpdateManagerBuilderHttpExtensions`
	Extension‑methods for `UpdateManagerBuilder`:
	- `WithHttpUpdateInfoProvider(HttpClient httpClient, IUpdateInfoParser updateInfoParser, string updateInfoUrl)`.
	- `WithHttpUpdateDownloader(HttpClient httpClient, string downloadsDir)`.

**Usage example:**

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