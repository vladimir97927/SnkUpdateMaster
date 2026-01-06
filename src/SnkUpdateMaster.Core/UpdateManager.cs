using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SnkUpdateMaster.Core.Downloader;
using SnkUpdateMaster.Core.Installer;
using SnkUpdateMaster.Core.Integrity;
using SnkUpdateMaster.Core.UpdateSource;
using SnkUpdateMaster.Core.VersionManager;

namespace SnkUpdateMaster.Core
{
    /// <summary>
    /// Основной класс для управления процессом обновления приложения
    /// </summary>
    /// <remarks>
    /// Реализует полный цикл обновления:
    /// <list type="number">
    /// <item><description>Проверка доступных обновлений</description></item>
    /// <item><description>Загрузка файлов обновления</description></item>
    /// <item><description>Проверка целостности файлов</description></item>
    /// <item><description>Установка обновления</description></item>
    /// <item><description>Обновление информации о версии</description></item>
    /// </list>
    /// 
    /// <para><strong>Зависимости:</strong></para>
    /// <param name="currentVersionManager">Менеджер текущей версии</param>
    /// <param name="updateInfoProvider">Источник информации об обновлениях</param>
    /// <param name="integrityVerifier">Верификатор целостности файлов</param>
    /// <param name="installer">Установщик обновлений</param>
    /// <param name="updateDownloader">Загрузчик обновлений</param>
    /// <param name="logger">Логгер для отслеживания процесса проверки, загрузки и установки обновлений</param>
    /// </remarks>
    public class UpdateManager(
        ICurrentVersionManager currentVersionManager,
        IUpdateInfoProvider updateInfoProvider,
        IIntegrityVerifier integrityVerifier,
        IInstaller installer,
        IUpdateDownloader updateDownloader,
        ILogger? logger = null)
    {
        private readonly ICurrentVersionManager _currentVersionManager = currentVersionManager;

        private readonly IUpdateInfoProvider _updateInfoProvider = updateInfoProvider;

        private readonly IIntegrityVerifier _integrityVerifier = integrityVerifier;

        private readonly IInstaller _installer = installer;

        private readonly IUpdateDownloader _updateDownloader = updateDownloader;

        private readonly ILogger _logger = logger ?? NullLogger.Instance;

        /// <summary>
        /// Выполняет полный цикл проверки и установки обновлений
        /// </summary>
        /// <param name="progress">Объект для отслеживания прогресса (0.0-1.0)</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>
        /// <c>true</c> - если обновление было успешно установлено<br/>
        /// <c>false</c> - если обновление не требуется или произошла ошибка
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// Операция была отменена через токен
        /// </exception>
        /// <exception cref="IOException">
        /// Ошибка ввода-вывода при работе с файлами
        /// </exception>
        public async Task<bool> CheckAndInstallUpdatesAsync(IProgress<double>? progress = null, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting update check and installation process.");
            try
            {
                var currentVersion = await _currentVersionManager.GetCurrentVersionAsync(cancellationToken);
                _logger.LogInformation("Current application version: {Version}", currentVersion?.ToString() ?? "undefined");

                var lastUpdateInfo = await _updateInfoProvider.GetLastUpdatesAsync(cancellationToken);
                if (lastUpdateInfo == null)
                {
                    _logger.LogInformation("No updates available.");
                    return false;
                }
                if (lastUpdateInfo.Version <= currentVersion)
                {
                    _logger.LogInformation("No new updates found. Latest version: {Version}", lastUpdateInfo.Version);
                    return false;
                }

                _logger.LogInformation("Starting download of update version {Version}.", lastUpdateInfo.Version);
                var downloadProgress = progress == null ? null : new Progress<double>(p => progress.Report(p * 0.7));
                var updateFilePath = await _updateDownloader.DownloadUpdateAsync(lastUpdateInfo, downloadProgress, cancellationToken);
                _logger.LogInformation("Update version {Version} downloaded to {FilePath}.", lastUpdateInfo.Version, updateFilePath);

                progress?.Report(0.75);
                _logger.LogInformation("Verifying checksum for {FilePath}. Expected checksum: {Checksum}.", updateFilePath, lastUpdateInfo.Checksum);
                if (!_integrityVerifier.VerifyFile(updateFilePath, lastUpdateInfo.Checksum))
                {
                    SafeDeleteFile(updateFilePath);
                    _logger.LogError("Checksum verification failed for {FilePath}. Expected checksum: {Checksum}.", updateFilePath, lastUpdateInfo.Checksum);
                    throw new IntegrityVerificationException(updateFilePath, lastUpdateInfo.Checksum);
                }

                _logger.LogInformation("Checksum verification succeeded for {FilePath}.", updateFilePath);
                progress?.Report(0.8);

                _logger.LogInformation("Starting installation of update version {Version}.", lastUpdateInfo.Version);
                var installProgress = progress == null ? null : new Progress<double>(p => progress.Report(0.8 + p * 0.2));
                await _installer.InstallAsync(updateFilePath, installProgress, cancellationToken);
                _logger.LogInformation("Update version {Version} installed successfully.", lastUpdateInfo.Version);

                _logger.LogInformation("Updating stored version to {Version}.", lastUpdateInfo.Version);
                await _currentVersionManager.UpdateCurrentVersionAsync(lastUpdateInfo.Version, cancellationToken);
                progress?.Report(1);

                _logger.LogInformation("Update process finished successfully. Current version is now {Version}.", lastUpdateInfo.Version);

                return true;
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning(ex, "Update process was canceled.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the update process.");
                throw;
            }
        }

        /// <summary>
        /// Получает текущую версию приложения
        /// </summary>
        /// <returns>
        /// Текущая версия приложения или null, если версия не определена
        /// </returns>
        public async Task<Version?> GetCurrentVersionAsync(CancellationToken cancellationToken = default)
        {
            return await _currentVersionManager.GetCurrentVersionAsync(cancellationToken);
        }

        private static void SafeDeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
