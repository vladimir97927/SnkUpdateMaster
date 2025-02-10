
namespace SnkUpdateMaster.Core.Installer
{
    public class ZipInstaller : IInstaller
    {
        private readonly string _appDir;

        private readonly string _backupDir;

        public ZipInstaller(string appDir)
        {
            _appDir = appDir;
            _backupDir = Path.Combine(_appDir, "backup");
        }

        public Task InstallAsync(string updateFilePath, IProgress<double> progress, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
