### SnkUpdateMaster.Core

**Purpose:** the base module with business logic, interfaces, and ready-made implementations for the update pipeline.

#### Typical usage

**Key interfaces:**
- `IUpdateInfoProvider` - fetches information about the available update.
- `IUpdateDownloader` - downloads the update file.
- `IInstaller` - applies the downloaded file to the application.
- `IIntegrityVerifier` - validates the checksum before installation.
- `ICurrentVersionManager` - reads and updates the current application version.

**Built-in implementations:**
- `ZipInstaller` - unpacks an update from a ZIP archive with backup.
- `ShaIntegrityVerifier` - SHA‑256 integrity check.
- `FileVersionManager` - stores the version in a text file (`major.minor.build`).

Use `UpdateManager` to orchestrate download and installation. Build it via `UpdateManagerBuilder`:
```csharp
var updateManager = new UpdateManagerBuilder()
    .WithZipInstaller("path to app folder")
    .WithFileCurrentVersionManager()
    .WithSha256IntegrityVerifier()
    .AddDependency<IUpdateInfoProvider>(customImplementation)
    .AddDependency<IUpdateDownloader>(customImplementation)
    .Build();
```

Check for updates and install:
```csharp
var updated = await updateManager.CheckAndInstallUpdatesAsync(progress);
```

> 💡 Besides the built-ins, you can register custom interface implementations via `AddDependency` and mix them with provided components.

#### Main subsystems

1. **Application update (`UpdateManager`)**  
   Files: `UpdateManager.cs`, `UpdateManagerBuilder.cs`, `UpdateInfo.cs`  
   Types:
   * `UpdateManager` - orchestrates the full update flow:
     1. get current version (`ICurrentVersionManager`);
     2. get available update info (`IUpdateInfoProvider`);
     3. download update file (`IUpdateDownloader`);
     4. verify checksum (`IIntegrityVerifier`);
     5. install update (`IInstaller`);
     6. update stored version (`ICurrentVersionManager.UpdateCurrentVersionAsync`).  
     Main method:
     ```csharp
     Task<bool> CheckAndInstallUpdatesAsync(
         IProgress<double> progress,
         CancellationToken cancellationToken = default);
     ```
     Returns `true` if an update was found and installed.
   * `UpdateInfo` - metadata: `Id`, `Version` (`System.Version`), `FileName`, `Checksum` (SHA‑256 hex), `ReleaseDate` (UTC), `FileDir`.
   * `UpdateManagerBuilder` (inherits `DependencyBuilder<UpdateManager>`) - fluent builder with shortcuts:
     * `WithFileCurrentVersionManager()` - stores version in `version` file under `Environment.CurrentDirectory`;
     * `WithSha256IntegrityVerifier()` - SHA‑256 integrity check;
     * `WithZipInstaller(string appDir)` - expands ZIP archive over `appDir` with backup.  
     Sources and downloaders are added directly via `AddDependency<T>` or through module extensions (`Ftp`, `SqlServer`).
2. **Version management (`VersionManager`)**  
   Files: `VersionManager/ICurrentVersionManager.cs`, `VersionManager/FileVersionManager.cs`  
   * `ICurrentVersionManager` - abstraction over version storage.  
   * `FileVersionManager` - stores version in a `version` text file (`major.minor.build`), creates the file on first update.
1. **Update providers (`UpdateSource`)**  
   File: `UpdateSource/IUpdateInfoProvider.cs`  
   * `IUpdateInfoProvider` - gets the latest available update from any source (FTP, DB, file system, etc.).  
   Concrete implementations live in `SnkUpdateMaster.Ftp` and `SnkUpdateMaster.SqlServer`.
4. **Downloaders (`Downloader`)**  
   File: `Downloader/IUpdateDownloader.cs`  
   * `IUpdateDownloader` - async download of the update file (HTTP, FTP, DB, local copy, etc.), returning a local path.  
   Implementations reside in `SnkUpdateMaster.Ftp` and `SnkUpdateMaster.SqlServer`.
5. **Installers (`Installer`)**  
   Files: `Installer/IInstaller.cs`, `Installer/ZipInstaller.cs`  
   * `IInstaller` - “how to apply the downloaded update.”  
   * `ZipInstaller`:
     * makes a full backup of the application directory;
     * unpacks the ZIP over the app;
     * on failure, attempts rollback by restoring the backup.  
   > ⚠️ `ZipInstaller` performs destructive file-system operations (delete/move directories). Use only when the app is stopped.
6. **Integrity and checksums (`Integrity`)**  
   Files: `Integrity/IChecksumCalculator.cs`, `Integrity/IIntegrityProvider.cs`, `Integrity/IIntegrityVerifier.cs`, `Integrity/ShaChecksumCalculator.cs`, `Integrity/ShaIntegrityProvider.cs`, `Integrity/ShaIntegrityVerifier.cs`, `Integrity/IntegrityProviderType.cs`  
   Types:
   * `IChecksumCalculator` - compute file checksum;
   * `IIntegrityProvider` - a wrapper around the calculator, can add extra checks;
   * `IIntegrityVerifier` - compares calculated checksum against expected;
   * `ShaChecksumCalculator` / `ShaIntegrityProvider` / `ShaIntegrityVerifier` - SHA‑256 implementations.
1. **Parsing update manifests (`Files`)**  
   Files: `Files/IUpdateInfoFileParser.cs`, `Files/JsonUpdateInfoFileParser.cs`  
   * `IUpdateInfoFileParser` - parses update metadata files.  
   * `JsonUpdateInfoFileParser` - JSON parser. Expected format (see `tests/ftp-data/manifest.json`):
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