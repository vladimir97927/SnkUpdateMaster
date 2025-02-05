using SnkUpdateMaster.Core.ReleasePackages;

namespace SnkUpdateMaster.Application.ReleasePackages
{
    internal class ReleasePackageService : IReleasePackageService
    {
        private readonly IRemoteReleaseRepository _releasePackageRepository;

        private readonly IReleasePackageCounter _releasePackageCounter;

        public ReleasePackageService(IRemoteReleaseRepository releasePackageRepository, IReleasePackageCounter releasePackageCounter)
        {
            _releasePackageRepository = releasePackageRepository;
            _releasePackageCounter = releasePackageCounter;
        }

        public async Task CreateReleaseAsync(string releasePath, string versionName, int versionCode, CancellationToken cancellationToken = default)
        {
            var fileName = Path.GetFileName(releasePath);
            var fileData = await File.ReadAllBytesAsync(releasePath, cancellationToken);
            var release = ReleasePackage.CreateNew(versionName, versionCode, fileName, fileData, _releasePackageCounter);
            await _releasePackageRepository.AddReleasePackageAsync(release);
        }
    }
}
