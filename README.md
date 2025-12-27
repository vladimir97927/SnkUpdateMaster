# SnkUpdateMaster

[![WIP](https://img.shields.io/badge/Status-Work%20In%20Progress-orange)](https://github.com/vladimir97927/SnkUpdateMaster)
![platform: .NET](https://img.shields.io/badge/platform-.NET-informational)
![language: C#](https://img.shields.io/badge/language-C%23-blue)
[![NuGet version (SnkUpdateMaster.Core)](https://img.shields.io/nuget/v/SnkUpdateMaster.Core.svg?style=flat-square)](https://www.nuget.org/packages/SnkUpdateMaster.Core/)

**Documentation:** English | [Русский](README.ru.md)

> **⚠️ Heads‑up:** The project is under active development. API and structure may change.

SnkUpdateMaster is a modular library for managing desktop application updates. It lets you store release metadata in SQL Server or on FTP, download and install ZIP archives with integrity checks, and keep track of the currently installed version.

## 🌟 Core idea

The library covers the full update cycle: discover an available version, download the file, verify its checksum, and safely apply the update with a backup. All key components - metadata provider, downloader, integrity verifier, and installer - are dependencies that can be swapped with your own implementations.

**Key capabilities**

1. Choose the update source
   - FTP/SFTP server (manifest and ZIP archives).
   - Microsoft SQL Server (`UpdateInfo` and `UpdateFile` tables).
2. Flexible installation: unzip the update over the app directory with backup.
3. SHA‑256 integrity check before installation.
4. Fluent configuration through `UpdateManagerBuilder`.

## ⚙️ Requirements

- **.NET Runtime 8.0**  
  (or a self‑contained build for the target OS)
- At least one update source configured:
  - **FTP server** - reachable externally;
  - **or SQL Server** - Microsoft SQL Server 2019+ supported.

## ⬇️ Installation

SnkUpdateMaster can be installed using the Nuget package manager or the dotnet command line interface.

**Core Components:**
```
dotnet add package SnkUpdateMaster.Core
```

**SqlServer Implementation:**
```
dotnet add package SnkUpdateMaster.SqlServer
```

**FTP Implementation:**
```
dotnet add package SnkUpdateMaster.Ftp
```

## 🚀 Quick start (SQL Server + FTP)

Example: store update metadata in SQL Server, keep update files on an FTP server. The current app version is stored in `[app directory]\version` as `major.minor.build`. Update files are ZIP archives; integrity is verified via SHA‑256.

Create the table first:
```sql
CREATE TABLE [dbo].[UpdateInfo]
(
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Version] NVARCHAR(256) NOT NULL,
    [FileName] NVARCHAR(256) NOT NULL,
    [FileDir] NVARCHAR(256) NULL, -- Path to the file on the FTP server
    [CheckSum] NVARCHAR(256) NOT NULL,
    [ReleaseDate] DATETIME NOT NULL,
)
```

Create and use `UpdateManager` to download and install updates:
```csharp
using SnkUpdateMaster.Core;
using SnkUpdateMaster.Ftp;
using SnkUpdateMaster.Ftp.Configuration;
using SnkUpdateMaster.SqlServer.Configuration;
using SnkUpdateMaster.SqlServer.Database;

var ftpClientFactory = new AsyncFtpClientFactory("localhost", "snk", "snk@12345", 2121);
var sqlConnectionFactory = new SqlConnectionFactory(ConnectionString);
var appDir = "app"; // Path to the application folder
var downloadsDir = "downloads";

var updateManager = new UpdateManagerBuilder()
    .WithFileCurrentVersionManager()
    .WithSha256IntegrityVerifier()
    .WithZipInstaller(appDir)
    .WithSqlServerUpdateInfoProvider(sqlConnectionFactory)
    .WithFtpUpdateDownloader(ftpClientFactory, downloadsDir)
    .Build();

var progress = new Progress<double>();
var updated = await updateManager.CheckAndInstallUpdatesAsync(progress);

Console.WriteLine(updated ? "Update installed" : "Already up to date");
```

> 💡 You can mix FTP and SQL as you like: keep metadata in the database and archives on the file server, or vice versa.

## 📚 Project modules
- [Core - application update building blocks.](docs/Core.md)
- [SqlServer - update flow backed by a relational database.](docs/SqlServer.md)
- [SnkUpdateMasterDb - database project.](docs/SnkUpdateMasterDb.md)
- [Ftp - update flow backed by an FTP server.](docs/Ftp.md)

## 🏗️ Library architecture

- **Core** - defines interfaces (`IUpdateInfoProvider`, `IUpdateDownloader`, `IInstaller`, `IIntegrityVerifier`, `ICurrentVersionManager`) and base implementations.
- **Ftp** - metadata provider and file downloader over FTP, plus FTP client factory.
- **SqlServer** - provider and downloader working with SQL Server tables; uses Dapper.
- **SnkUpdateMasterDb** - SQL Server infrastructure in Docker with ready-to-use schema scripts.

Extend the library by registering your own interface implementations or adding extension methods to `UpdateManagerBuilder`.

## 🧪 Testing with Docker

Integration tests rely on Docker Compose. The root `docker-compose.yml` describes required services. Tests are run via standard .NET tooling against the Docker infra.

### Requirements

- **Docker** / **Docker Desktop**;
- **Docker Compose v2**;
- **.NET SDK 8.0**.

### 1. Start the infrastructure

From the repo root:

```bash
docker compose up
```

The command starts SQL Server and FTP with test data. On first run, creating tables may take up to 30 seconds.

### 2. Run tests

When containers are up and the database is ready, run:

```bash
dotnet test SnkUpdateMaster.sln
```

or a specific test project:

```bash
dotnet test tests/ProjectName.Tests/ProjectName.Tests.csproj
```

Tests use the connection string configured for Docker. Connection parameters (port, login/password, etc.) can be overridden in `docker-compose.override.yml` or via environment variables.

### 3. Tear down and clean up

After tests, stop and remove containers:

```bash
docker compose down
```

## 🧪 Testing without Docker

Use this mode when you have a local environment with dependencies running. SQL Server and FTP must be available to the test projects.

### Requirements

- **.NET SDK 8.0**;
- configured and running **SQL Server** for the test DB;
- configured and running **FTP server** for integration tests.

### Environment variables

**FTP tests** (`SnkUpdateMaster.Ftp.IntegrationTests`):

- `SNK_UPDATE_MASTER__FTP_HOST` - FTP host (default `localhost`)
- `SNK_UPDATE_MASTER__FTP_PORT` - FTP port (default `2121`)
- `SNK_UPDATE_MASTER__FTP_USER` - login (default `snk`)
- `SNK_UPDATE_MASTER__FTP_PASS` - password (default `snk@12345`)

**SQL Server tests** (`SnkUpdateMaster.SqlServer.IntegrationTests`):

- `SNK_UPDATE_MASTER__SQL_CONN` - SQL Server connection string  
  Default:  
  `Server=localhost,1455;Database=SnkUpdateMasterDb;User Id=sa;Password=Snk@12345;Encrypt=False;TrustServerCertificate=True`

## 🧭 Useful scenarios

- **FTP only** - metadata and archives on FTP: use `WithFtpUpdateInfoProvider` and `WithFtpUpdateDownloader`.
- **SQL Server only** - metadata and files in the DB: use `WithSqlServerUpdateInfoProvider` and `WithSqlServerUpdateDownloader`.
- **Hybrid** - metadata in SQL Server, files on FTP: combine providers as in the Quick Start example.

If you need another source (REST API, file system, etc.), implement `IUpdateInfoProvider` and/or `IUpdateDownloader` and register them via `UpdateManagerBuilder.AddDependency`.

## 📃 License, Copyright, etc.

SnkUpdateMaster is copyrighted by [Vladimir Vyplaven](https://github.com/vladimir97927) and is distributed under the [Apache2](LICENSE.txt) license.