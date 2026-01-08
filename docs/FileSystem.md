### SnkUpdateMaster.FileSystem

**Purpose:** A module that reads metadata and files updated from the local file system.

### Available Implementations

- `FileSystemUpdateInfoProvider: IUpdateInfoProvider`
	Reads file metadata (e.g., `C:\releases\manifest.json`) and parses it using `IUpdateInfoFileParser`.
- `FileSystemUpdateDownloader: IUpdateDownloader`
	Copies update files from local release folders to the download path.
- `UpdateManagerBuilderFileSystemExtensions`
	Extension methods for `UpdateManagerBuilder`:
	- `WithFileSystemUpdateInfoProvider(IUpdateInfoFileParser updateInfoFileParser, string updateFileInfoPath)`
	- `WithFileSystemUpdateDownloader(string DownloadsDir)`

**Usage example:**

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