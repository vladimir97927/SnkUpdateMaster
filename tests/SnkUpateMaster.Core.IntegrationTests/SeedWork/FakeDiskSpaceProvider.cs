using SnkUpdateMaster.Core.Installer;

namespace SnkUpateMaster.Core.IntegrationTests.SeedWork
{
    internal class FakeDiskSpaceProvider : IDiskSpaceProvider
    {
        private readonly long _availableBytes;

        public FakeDiskSpaceProvider(long availableBytes)
        {
            _availableBytes = availableBytes;
        }

        public long GetAvailableFreeSpace(string path)
        {
            return _availableBytes;
        }
    }
}
