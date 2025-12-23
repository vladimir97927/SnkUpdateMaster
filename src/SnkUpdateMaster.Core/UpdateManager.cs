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
    /// </remarks>
    public class UpdateManager(
        ICurrentVersionManager currentVersionManager,
        IUpdateInfoProvider updateInfoProvider,
        IIntegrityVerifier integrityVerifier,
        IInstaller installer,
        IUpdateDownloader updateDownloader)
    {
        private readonly ICurrentVersionManager _currentVersionManager = currentVersionManager;

        private readonly IUpdateInfoProvider _updateInfoProvider = updateInfoProvider;

        private readonly IIntegrityVerifier _integrityVerifier = integrityVerifier;

        private readonly IInstaller _installer = installer;

        private readonly IUpdateDownloader _updateDownloader = updateDownloader;

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
            var currentVersion = await _currentVersionManager.GetCurrentVersionAsync(cancellationToken);
            var lastUpdateInfo = await _updateInfoProvider.GetLastUpdatesAsync(cancellationToken);
            if (lastUpdateInfo == null || lastUpdateInfo.Version <= currentVersion)
            {
                return false;
            }

            var downloadProgress = progress == null ? null : new Progress<double>(p => progress.Report(p * 0.7));
            var updateFilePath = await _updateDownloader.DownloadUpdateAsync(lastUpdateInfo, downloadProgress, cancellationToken);
            progress?.Report(0.75);
            if (!_integrityVerifier.VerifyFile(updateFilePath, lastUpdateInfo.Checksum))
            {
                SafeDeleteFile(updateFilePath);
                throw new IntegrityVerificationException(updateFilePath, lastUpdateInfo.Checksum);
            }

            progress?.Report(0.8);
            var installProgress = progress == null ? null : new Progress<double>(p => progress.Report(0.8 + p * 0.2));
            await _installer.InstallAsync(updateFilePath, installProgress, cancellationToken);
            await _currentVersionManager.UpdateCurrentVersionAsync(lastUpdateInfo.Version, cancellationToken);
            progress?.Report(1);

            return true;
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
