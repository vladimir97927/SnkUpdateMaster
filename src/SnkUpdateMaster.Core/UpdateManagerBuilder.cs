using Microsoft.Extensions.Logging;
using SnkUpdateMaster.Core.Common;
using SnkUpdateMaster.Core.Downloader;
using SnkUpdateMaster.Core.Installer;
using SnkUpdateMaster.Core.Integrity;
using SnkUpdateMaster.Core.UpdateSource;
using SnkUpdateMaster.Core.VersionManager;

namespace SnkUpdateMaster.Core
{
    /// <summary>
    /// Строитель для создания и настройки экземпляра <see cref="UpdateManager"/>
    /// </summary>
    /// <remarks>
    /// Реализует паттерн "Строитель" для пошаговой настройки зависимостей <see cref="UpdateManager"/>.
    /// Позволяет гибко конфигурировать компоненты системы обновлений через fluent-интерфейс.
    /// 
    /// <para><strong>Обязательные зависимости:</strong></para>
    /// <list type="bullet">
    /// <item><description><see cref="IUpdateInfoProvider"/></description></item>
    /// <item><description><see cref="IUpdateDownloader"/></description></item>
    /// </list>
    /// 
    /// <para><strong>Зависимости, имеющие реализацию по умолчанию:</strong></para>
    /// <list type="bullet">
    /// <item><description><see cref="ICurrentVersionManager"/></description></item>
    /// <item><description><see cref="IIntegrityVerifier"/></description></item>
    /// <item><description><see cref="IInstaller"/></description></item>
    /// </list>
    /// </remarks>
    public class UpdateManagerBuilder : DependencyBuilder<UpdateManager>
    {
        /// <summary>
        /// Регистрирует файловую реализацию менеджера версий
        /// </summary>
        /// <returns>Текущий экземпляр строителя</returns>
        /// <remarks>
        /// Использует <see cref="FileVersionManager"/> для работы с версией через файл в рабочей директории
        /// </remarks>
        public UpdateManagerBuilder WithFileCurrentVersionManager()
        {
            AddDependency<ICurrentVersionManager>(new FileVersionManager());
            return this;
        }

        /// <summary>
        /// Регистрирует SHA-256 верификатор целостности
        /// </summary>
        /// <returns>Текущий экземпляр строителя</returns>
        /// <remarks>
        /// Использует <see cref="ShaIntegrityVerifier"/> для проверки контрольных сумм файлов
        /// </remarks>
        public UpdateManagerBuilder WithSha256IntegrityVerifier()
        {
            AddDependency<IIntegrityVerifier>(new ShaIntegrityVerifier());
            return this;
        }

        /// <summary>
        /// Регистрирует установщик из ZIP-архива
        /// </summary>
        /// <param name="appDir">Путь к корневой директории приложения</param>
        /// <returns>Текущий экземпляр строителя</returns>
        /// <remarks>
        /// Создает <see cref="ZipInstaller"/> с автоматическим созданием бэкапов
        /// </remarks>
        public UpdateManagerBuilder WithZipInstaller(string appDir)
        {
            var installer = new ZipInstaller(appDir);
            AddDependency<IInstaller>(installer);
            return this;
        }

        public UpdateManagerBuilder WithLogger(ILogger<UpdateManager> logger)
        {
            AddDependency(logger);
            return this;
        }

        /// <summary>
        /// Создает экземпляр <see cref="UpdateManager"/> с настроенными зависимостями
        /// </summary>
        /// <returns>Полностью сконфигурированный <see cref="UpdateManager"/></returns>
        public override UpdateManager Build()
        {
            var currentVersionManager = GetDependency<ICurrentVersionManager>();
            var updateSource = GetDependency<IUpdateInfoProvider>();
            var integrityVerifier = GetDependency<IIntegrityVerifier>();
            var installer = GetDependency<IInstaller>();
            var downloader = GetDependency<IUpdateDownloader>();
            TryGetDependency(out ILogger<UpdateManager>? logger);

            return new UpdateManager(
                currentVersionManager,
                updateSource,
                integrityVerifier,
                installer,
                downloader,
                logger);
        }
    }
}
