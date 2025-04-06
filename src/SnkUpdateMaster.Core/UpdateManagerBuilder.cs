using SnkUpdateMaster.Core.Downloader;
using SnkUpdateMaster.Core.Installer;
using SnkUpdateMaster.Core.Integrity;
using SnkUpdateMaster.Core.UpdateSource;
using SnkUpdateMaster.Core.VersionManager;

namespace SnkUpdateMaster.Core
{
    public class UpdateManagerBuilder
    {
        private readonly Dictionary<Type, object> _dependencies = [];

        internal T? GetDependency<T>()
        {
            if (_dependencies.TryGetValue(typeof(T), out var dependency))
            {
                try
                {
                    return (T?)dependency;
                }
                catch
                {
                    return default;
                }
            }

            return default;
        }

        public UpdateManagerBuilder AddDependency<T>(T dependency)
        {
            if (dependency == null)
                throw new ArgumentNullException(nameof(dependency));
            _dependencies[typeof(T)] = dependency;
            return this;
        }

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

        public UpdateManager Build()
        {
            var currentVersionManager = GetDependency<ICurrentVersionManager?>();
            var updateSource = GetDependency<IUpdateSource?>();
            var integrityVerifier = GetDependency<IIntegrityVerifier?>();
            var installer = GetDependency<IInstaller?>();
            var downloader = GetDependency<IUpdateDownloader?>();

            return new UpdateManager(
                currentVersionManager ?? throw new ArgumentNullException(nameof(currentVersionManager), $"Add {nameof(ICurrentVersionManager)} dependency"),
                updateSource ?? throw new ArgumentNullException(nameof(updateSource), $"Add {nameof(IUpdateSource)} dependency"),
                integrityVerifier ?? throw new ArgumentNullException(nameof(integrityVerifier), $"Add {nameof(IIntegrityVerifier)} dependency"),
                installer ?? throw new ArgumentNullException(nameof(installer), $"Add {nameof(IInstaller)} dependency"),
                downloader ?? throw new ArgumentNullException(nameof(downloader), $"Add {nameof(IUpdateDownloader)} dependency"));
        }
    }
}
