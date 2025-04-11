using SnkUpdateMaster.Core.Common;
using SnkUpdateMaster.Core.Downloader;
using SnkUpdateMaster.Core.Installer;
using SnkUpdateMaster.Core.Integrity;
using SnkUpdateMaster.Core.UpdateSource;
using SnkUpdateMaster.Core.VersionManager;

namespace SnkUpdateMaster.Core
{
    public class UpdateManagerBuilder : DependencyBuilder<UpdateManager>
    {
        public UpdateManagerBuilder WithFileCurrentVersionManager()
        {
            AddDependency<ICurrentVersionManager>(new FileVersionManager());
            return this;
        }

        public UpdateManagerBuilder WithSha256IntegrityVerifier()
        {
            AddDependency<IIntegrityVerifier>(new ShaIntegrityVerifier());
            return this;
        }

        public UpdateManagerBuilder WithZipInstaller(string appDir)
        {
            var installer = new ZipInstaller(appDir);
            AddDependency<IInstaller>(installer);
            return this;
        }

        public override UpdateManager Build()
        {
            var currentVersionManager = GetDependency<ICurrentVersionManager>();
            var updateSource = GetDependency<IUpdateSource>();
            var integrityVerifier = GetDependency<IIntegrityVerifier>();
            var installer = GetDependency<IInstaller>();
            var downloader = GetDependency<IUpdateDownloader>();

            return new UpdateManager(
                currentVersionManager,
                updateSource,
                integrityVerifier,
                installer,
                downloader);
        }
    }
}
