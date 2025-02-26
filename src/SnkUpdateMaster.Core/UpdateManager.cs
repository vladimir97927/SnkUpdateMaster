using SnkUpdateMaster.Core.Installer;
using SnkUpdateMaster.Core.Integrity;
using SnkUpdateMaster.Core.VersionManager;

namespace SnkUpdateMaster.Core
{
    class UpdateManager(
        ICurrentVersionManager currentVersionManager,
        IUpdateSource updateSource,
        IIntegrityVerifier integrityVerifier,
        IInstaller installer)
    {
        private readonly ICurrentVersionManager _currentVersionManager = currentVersionManager;

        private readonly IUpdateSource _updateSource = updateSource;

        private readonly IIntegrityVerifier _integrityVerifier = integrityVerifier;

        private readonly IInstaller _installer = installer;

        public async Task<bool> CheckAndInstallUpdatesAsync(IProgress<double> progress, CancellationToken cancellationToken = default)
        {
            var currentVersion = await _currentVersionManager.GetCurrentVersionAsync();
            var lastUpdateInfo = await _updateSource.GetLastUpdatesAsync();
            if (lastUpdateInfo == null || lastUpdateInfo.Version <= currentVersion)
            {
                return false;
            }

            var updatesPath = Path.Combine(Environment.CurrentDirectory, "updates");
            var downloadProgress = new Progress<double>(p => progress.Report(p * 0.7));
            var updateFilePath = await _updateSource.DownloadUpdateAsync(lastUpdateInfo, updatesPath, downloadProgress, cancellationToken);
            progress.Report(0.75);
            if (!_integrityVerifier.VerifyFile(updateFilePath, lastUpdateInfo.Checksum))
            {
                return false;
            }

            progress.Report(0.8);
            var installProgress = new Progress<double>(p => progress.Report(0.8 + p * 0.2));
            await _installer.InstallAsync(updateFilePath, installProgress, cancellationToken);
            await _currentVersionManager.UpdateCurrentVersionAsync(lastUpdateInfo.Version);
            progress.Report(1);

            return true;
        }
    }
}
