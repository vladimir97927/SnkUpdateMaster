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
- `IInstaller` - Installs downloaded updates:
	- `ZipInstaller` - Unpacks update files from a ZIP archive. Used by calling the `WithZipInstaller` method.
- `IIntegrityVerifier` - Verifies file integrity by comparing checksums:
	- `ShaIntegrityVerifier` - Verifies file integrity using the SHA-256 algorithm. Used by calling the `WithSha256IntegrityVerifier` method.
- `ICurrentVersionManager` - Manages the currently installed application version:
	- `FileVersionManager` - Stores the version in a text file. The version format is `major.minor.build`. Used by calling the `WithFileCurrentVersionManager` method.

Use `UpdateManager` to orchestrate download and installation. Build it via `UpdateManagerBuilder`:
```csharp
var updateManager = new UpdateManagerBuilder()
	.WithZipInstaller("path to app folder")
	.WithFileCurrentVersionManager()
	.WithSha256IntegrityVerifier()
	.RegisterInstance<IUpdateInfoProvider>(customProviderInstance)
	.RegisterFactory<IUpdateDownloader>(customDownloaderFactory)
	.Build();
```

The `RegisterInstance<T>` and `RegisterFactory<T>` methods allow you to add the required dependencies to the builder as a instance object or as a factory method.

Check for updates and install:
```csharp
var updated = await updateManager.CheckAndInstallUpdatesAsync(progress);
```

> 💡 Besides the built-ins, you can register custom interface implementations via `AddDependency` and mix them with provided components.

#### Logging

By default, if the `ILoggerFactory` isn't registered, `NullLogger` is used and no messages are output. To enable logging, register the logger factory by calling the `WithLogger` method in the builder:
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

This factory is used to create `ILogger` in `UpdateManager`, as well as in `IUpdateInfoProvider` and `IUpdateDownloader` implementations from other modules, such as `Ftp` and `SqlServer`.