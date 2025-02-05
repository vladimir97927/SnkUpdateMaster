
using SnkUpdateMaster.Application.FileSystem;
using System.Text;

namespace SnkUpdateMaster.Application.Version
{
    public class VersionManager : IVersionManager
    {
        private readonly IFileSystemService _fileSystemService;

        private readonly string _versionFilePath;

        public VersionManager(IFileSystemService fileSystemService, string versionFilePath)
        {
            _fileSystemService = fileSystemService;
            _versionFilePath = versionFilePath;
        }

        public async Task<int> GetInstalledVersionAsync()
        {
            var versionCode = await _fileSystemService.ReadTextFileAsync(_versionFilePath);
            if (string.IsNullOrEmpty(versionCode))
                return 0;
            return int.Parse(versionCode);
        }

        public async Task SetInstalledVersionAsync(int version)
        {
            await _fileSystemService.WriteFileAsync(_versionFilePath, Encoding.UTF8.GetBytes(version.ToString()));
        }
    }
}
