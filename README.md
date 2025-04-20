# SnkUpdateMaster

[![WIP](https://img.shields.io/badge/Status-Work%20In%20Progress-orange)](https://github.com/vladimir97927/SnkUpdateMaster)

> **⚠️ Внимание**: Проект в разработке. API и структура могут меняться.

Проект является универсальной библиотекой для управления обновлениями приложений.

**Основная идея**

Проект представляет собой модульную библиотеку, предназначенную для управления процессом обновления настольных приложений. Ключевой особенностью является поддержка кастомизации на всех этапах работы: от получения метаданных до установки новых версий, что позволяет адаптировать решение под уникальные требования инфраструктуры.

**Ключевые возможности**

1. Выбор источника обновлений
   - HTTP/S-сервер.
   - FTP/SFTP-сервер.
   - Реляционные базы данных.
   - GitHub Releases.
2. Выбор установщика
   - Архивы.
   - Исполняемые файлы.
   - Скрипты.
3. Контроль целостности данных. Выбор алгоритма хэширования

## Зависимости проекта
- Язык программирования: C# 12.0.
- Платформа: .NET 8.0.
- Microsoft.EntityFrameworkCore 9.0.2.
- Microsoft.EntityFrameworkCore.Relational 9.0.2.
- Microsoft.EntityFrameworkCore.SqlServer 9.0.2.
- Microsoft.Data.SqlClient 6.0.1.
- Dapper 2.1.66.

## Модули проекта
- [Core - основные компоненты работы приложения.](docs/README-CORE.md)
- [SqlServer - реализация работы с обновлениями через реляционную базу данных.](docs/README-SQL-SERVER.md)
- [SnkUpdateMasterDb - проект базы данных.]()

## Приложения

**ReleasePublisher.CLI**

Программа, использующая разработанную библиотеку для загрузки новых и просмотра существующих ZIP архивов с обновлениями в БД SQL Server.

Для установки приложения клонируйте репозиторий:

```
git clone https://github.com/vladimir97927/SnkUpdateMaster.git
```

Соберите проект из папки решения

```
dotnet publish src/SnkUpdateMaster.ReleasePublisher.CLI/SnkUpdateMaster.ReleasePublisher.CLI.csproj --artifacts-path build-release-publisher-cli
```

## Установка








