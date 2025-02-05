using SnkUpdateMaster.Application.FileSystem;
using SnkUpdateMaster.Application.Version;
using SnkUpdateMaster.Core.ReleasePackages;

namespace SnkUpdateMaster.Application.Update
{
    public class UpdateService : IUpdateService
    {
        private readonly IRemoteReleaseRepository _remoteReleaseRepository;

        private readonly IFileSystemService _fileSystemService;

        private readonly IVersionManager _versionManager;

        private string _packagesDir;

        public UpdateService(
            IRemoteReleaseRepository releasePackageRepository,
            IFileSystemService fileSystemService,
            IVersionManager versionManager,
            string packagesDir)
        {
            _remoteReleaseRepository = releasePackageRepository;
            _fileSystemService = fileSystemService;
            _versionManager = versionManager;
            _packagesDir = packagesDir;
        }

        public async Task<bool> CheckForUpdatesAsync()
        {
            var installedVersion = await _versionManager.GetInstalledVersionAsync();
            var remoteRelease = await _remoteReleaseRepository.GetLatestReleaseInfoAsync();
            return remoteRelease != null && remoteRelease.VersionCode > installedVersion;
        }

        public async Task DownloadUpdatesAsync()
        {
            var releaseInfo = await _remoteReleaseRepository.GetLatestReleaseInfoAsync();
            if (releaseInfo == null)
                return;

            var releasePath = Path.Combine(_packagesDir, releaseInfo.FileName);
            if (File.Exists(releasePath))
                return;

            _fileSystemService.ClearDirectory(_packagesDir);
            var pack = await _remoteReleaseRepository.GetLatestReleasePackage();
            if (pack == null)
                return;

            await _fileSystemService.WriteFileAsync(releasePath, pack.FileData);
        }

        public async Task ApplyUpdatesAsync()
        {
            var packageFiles = _fileSystemService.GetFiles(_packagesDir);
            if (packageFiles.Any())
                return;
            foreach (var packageFile in packageFiles)
            {

            }
        }
    }
}
