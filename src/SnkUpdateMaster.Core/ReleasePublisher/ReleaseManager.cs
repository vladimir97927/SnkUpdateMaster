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
            progress.Report(0.2);
            var release = await _releasePackager.PackAsync(appDir, destPath, version);
            progress.Report(0.5);
            await _releaseSource.UploadReleaseAsync(release);
            progress.Report(1);

            return release.Id;
        }
    }
}
