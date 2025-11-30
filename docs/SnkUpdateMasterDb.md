### Database

**Назначение:** окружение для SQL Server, скрипты создания базы и структуры.

Файлы:

*   `Database/Dockerfile`
*   `Database/entrypoint.sh`
*   `Database/SnkUpdateMasterDb/scripts/CreateDatabase.sql`
*   `Database/SnkUpdateMasterDb/scripts/CreateStructure.sql`
*   `Database/SnkUpdateMasterDb/dbo/Tables/*.sql` — таблицы `UpdateInfo`, `UpdateFile`

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

*   `UpdateInfo` — метаданные доступных обновлений.
*   `UpdateFile` — BLOB‑ы файлов обновлений.

> ⚠️ Логин/пароль `sa / Snk@12345` и настройки из Docker предназначены только для разработки и тестов, не использовать в продакшене.
