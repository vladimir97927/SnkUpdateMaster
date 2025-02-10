namespace SnkUpdateMaster.Core.Installer
{
    public interface IInstaller
    {
        Task InstallAsync(string updateFilePath, IProgress<double> progress, CancellationToken cancellationToken);
    }
}
