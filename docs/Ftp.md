### SnkUpdateMaster.Ftp

**Purpose:** module that retrieves update metadata and archives via FTP/SFTP.

#### Provided implementations

* `IAsyncFtpClientFactory` / `AsyncFtpClientFactory`  
  Wraps `FluentFTP.AsyncFtpClient` creation and connection. Supports reuse and automatic reconnects.
* `FtpUpdateInfoProvider : IUpdateInfoProvider`  
  Downloads a manifest file (e.g., `/manifest.json`) and parses it using `IUpdateInfoFileParser` (typically `JsonUpdateInfoFileParser`).
* `FtpUpdateDownloader : IUpdateDownloader`  
  Downloads the update archive from FTP into a local downloads directory with progress reporting and status checks.
* `UpdateManagerBuilderFtpExtensions`  
  Extension methods for `UpdateManagerBuilder`:
  * `WithFtpUpdateInfoProvider(IUpdateInfoFileParser parser, IAsyncFtpClientFactory ftpFactory, string updateFileInfoPath)`
  * `WithFtpUpdateDownloader(IAsyncFtpClientFactory ftpFactory, string downloadsDir)`

**Usage example:**

```csharp
using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Files;
using SnkUpdateMaster.Ftp;
using SnkUpdateMaster.Ftp.Configuration;

var ftpClientFactory = new AsyncFtpClientFactory("localhost", "snk", "snk@12345", 2121);
var updateInfoFileParser = new JsonUpdateInfoFileParser();
var appDir = "app";
var downloadsDir = "downloads";

var updateManager = new UpdateManagerBuilder()
    .WithFileCurrentVersionManager()
    .WithSha256IntegrityVerifier()
    .WithZipInstaller(appDir)
    .WithFtpUpdateInfoProvider(updateInfoFileParser, ftpClientFactory, "/manifest.json")
    .WithFtpUpdateDownloader(ftpClientFactory, downloadsDir)
    .Build();

var progress = new Progress<double>();
var updated = await updateManager.CheckAndInstallUpdatesAsync(progress);
```

> 🔧 Tip: set FTP client timeouts and use a dedicated downloads directory to avoid interfering with application files.