### SnkUpdateMasterDb

**Purpose:** infrastructure module with SQL Server Docker setup and schema scripts for storing update metadata and files.

Files:

* `Database/Dockerfile`
* `Database/entrypoint.sh`
* `Database/SnkUpdateMasterDb/scripts/CreateDatabase.sql`
* `Database/SnkUpdateMasterDb/scripts/CreateStructure.sql`
* `Database/SnkUpdateMasterDb/dbo/Tables/*.sql` — `UpdateInfo`, `UpdateFile` tables

`Dockerfile`:

* base image: `mcr.microsoft.com/mssql/server:2025-latest`;
* copies scripts into the container;
* sets:
  * `ACCEPT_EULA=Y`
  * `MSSQL_SA_PASSWORD=Snk@12345`
  * `MSSQL_TCP_PORT=1433`;
* runs `entrypoint.sh`, which:
  * starts SQL Server;
  * waits 30 seconds;
  * executes `CreateDatabase.sql` and `CreateStructure.sql`.

Key tables:

* `UpdateInfo` — available update metadata.
* `UpdateFile` — BLOBs with update files.

**How to use:**
- start infra with `docker compose up` in the repo root — the container creates the DB and schema automatically;
- default connection string: `Server=localhost,1455;Database=SnkUpdateMasterDb;User Id=sa;Password=Snk@12345;Encrypt=False;TrustServerCertificate=True`.

> ⚠️ The `sa / Snk@12345` credentials and Docker settings are for development and testing only. Do not use in production.