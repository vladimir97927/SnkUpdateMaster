using SnkUpdateMaster.Core.Downloader;
using SnkUpdateMaster.Core.Installer;
using SnkUpdateMaster.Core.Integrity;
using SnkUpdateMaster.Core.UpdateSource;
using SnkUpdateMaster.Core.VersionManager;

namespace SnkUpdateMaster.Core
{
    class UpdateManagerBuilder
    {
        private ICurrentVersionManager _currentVersionProvider;

        private IUpdateSource _updateSource;

        private IUpdateDownloader _updateDownloader;

        private IIntegrityVerifier _integrityVerifier;

        private IInstaller _installer;


    }
}
