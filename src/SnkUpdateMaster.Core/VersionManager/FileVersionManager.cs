
namespace SnkUpdateMaster.Core.VersionManager
{
    class FileVersionManager : ICurrentVersionManager
    {
        private readonly string _versionFileName = "version";

        public async Task<Version> GetCurrentVersionAsync()
        {
            var versionFilePath = Path.Combine(Environment.CurrentDirectory, _versionFileName);
            if (!File.Exists(versionFilePath))
            {
                throw new FileNotFoundException("Файл версии не найден.", versionFilePath);
            }

            var versionString = (await File.ReadAllTextAsync(versionFilePath)).Trim();
            if (Version.TryParse(versionString, out var version))
            {
                return version;
            }

            throw new InvalidOperationException("Неверный формат версии в файле.");
        }

        public async Task UpdateCurrentVersionAsync(Version version)
        {
            var versionFilePath = Path.Combine(Environment.CurrentDirectory, _versionFileName);
            if (!File.Exists(versionFilePath))
            {
                throw new FileNotFoundException("Файл версии не найден.", versionFilePath);
            }

            await File.WriteAllTextAsync(versionFilePath, version.ToString());
        }
    }
}
