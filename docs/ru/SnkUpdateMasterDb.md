### SnkUpdateMasterDb

**Назначение:** инфраструктурный модуль с Docker-окружением SQL Server и скриптами создания схемы данных для хранения метаданных и файлов обновлений.

Файлы:

*   `Database/Dockerfile`
*   `Database/entrypoint.sh`
*   `Database/SnkUpdateMasterDb/scripts/CreateDatabase.sql`
*   `Database/SnkUpdateMasterDb/scripts/CreateStructure.sql`
*   `Database/SnkUpdateMasterDb/dbo/Tables/*.sql` - таблицы `UpdateInfo`, `UpdateFile`

`Dockerfile`:

*   базовый образ: `mcr.microsoft.com/mssql/server:2025-latest`;
*   копирует скрипты в контейнер;
*   задаёт:
    *   `ACCEPT_EULA=Y`
    *   `MSSQL_SA_PASSWORD=Snk@12345`
    *   `MSSQL_TCP_PORT=1433`;
*   запускает `entrypoint.sh`, который:
    *   стартует SQL Server;
    *   ждёт 30 секунд;
    *   выполняет `CreateDatabase.sql` и `CreateStructure.sql`.

Основные таблицы:

*   `UpdateInfo` - метаданные доступных обновлений.
*   `UpdateFile` - BLOB‑ы файлов обновлений.

**Как использовать:**
- запустите инфраструктуру через `docker compose up` в корне репозитория - контейнер создаст базу и структуру автоматически;
- строка подключения по умолчанию: `Server=localhost,1455;Database=SnkUpdateMasterDb;User Id=sa;Password=Snk@12345;Encrypt=False;TrustServerCertificate=True`.

> ⚠️ Логин/пароль `sa / Snk@12345` и настройки из Docker предназначены только для разработки и тестов. Не использовать в продакшене.
