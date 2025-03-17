using SnkUpdateMaster.Core.ReleasePublisher.Packager;

namespace SnkUpdateMaster.Core.ReleasePublisher
{
    public class ReleaseManager(IReleasePackager releasePackager, IReleaseSource releaseSource)
    {
        private readonly IReleasePackager _releasePackager = releasePackager;

        private readonly IReleaseSource _releaseSource = releaseSource;

        public async Task<int> PulishReleaseAsync(string appDir, Version version, IProgress<double> progress, CancellationToken cancellationToken = default)
        {
            var destPath = Path.Combine(Environment.CurrentDirectory, "Releases");
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }
            var release = await _releasePackager.PackAsync(appDir, destPath, version);
            await _releaseSource.UploadReleaseAsync(release);

            return release.Id;
        }
    }
}
