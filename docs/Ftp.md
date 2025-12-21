### SnkUpdateMaster.Ftp

**Назначение:** реализация источника информации об обновлениях и загрузчика обновлений через FTP.

#### Предоставляемые реализации

*   `IAsyncFtpClientFactory` / `AsyncFtpClientFactory`  
    Инкапсулирует создание и подключение `FluentFTP.AsyncFtpClient`. Позволяет переиспользовать подключение, автоматически переподключаться.
*   `FtpUpdateInfoProvider : IUpdateInfoProvider`  
    Скачивает файл манифеста (например, `/manifest.json`) и парсит его через `IUpdateInfoFileParser` (обычно `JsonUpdateInfoFileParser`).
*   `FtpUpdateDownloader : IUpdateDownloader`  
    Скачивает архив обновления из FTP в локальную директорию загрузок, с поддержкой прогресса и проверки статуса FTP‑операции.
*   `UpdateManagerBuilderFtpExtensions`  
    Extension‑методы для `UpdateManagerBuilder`:
    *   `WithFtpUpdateInfoProvider(IUpdateInfoFileParser parser, IAsyncFtpClientFactory ftpFactory, string updateFileInfoPath)`
    *   `WithFtpUpdateDownloader(IAsyncFtpClientFactory ftpFactory, string downloadsDir)`

**Пример использования:**

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
