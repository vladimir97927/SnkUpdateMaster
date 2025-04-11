using SnkUpdateMaster.Core.Common;
using SnkUpdateMaster.Core.Integrity;
using SnkUpdateMaster.Core.ReleasePublisher.Packager;

namespace SnkUpdateMaster.Core.ReleasePublisher
{
    public class ReleaseManagerBuilder : DependencyBuilder<ReleaseManager>
    {
        public ReleaseManagerBuilder WithZipPackager(IntegrityProviderType integrityProviderType = IntegrityProviderType.Sha256)
        {
            var integrityProvider = new ShaIntegrityProvider();
            switch (integrityProviderType)
            {
                case IntegrityProviderType.Sha256:
                    integrityProvider = new ShaIntegrityProvider();
                break;
            }
            AddDependency<IReleasePackager>(new ZipReleasePackager(integrityProvider));
            return this;
        }

        public override ReleaseManager Build()
        {
            var packager = GetDependency<IReleasePackager>();
            var releaseSource = GetDependency<IReleaseSource>();

            return new ReleaseManager(
                packager,
                releaseSource);
        }
    }
}
