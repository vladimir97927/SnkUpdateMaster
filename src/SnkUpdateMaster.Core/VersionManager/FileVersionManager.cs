
namespace SnkUpdateMaster.Core.VersionManager
{
    public class FileVersionManager : ICurrentVersionManager
    {
        private readonly string _versionFileName = "version";

        public async Task<Version?> GetCurrentVersionAsync()
        {
            var versionFilePath = Path.Combine(Environment.CurrentDirectory, _versionFileName);
            if (!File.Exists(versionFilePath))
            {
                // throw new FileNotFoundException("Файл версии не найден", versionFilePath);
                return null;
            }

            var versionString = (await File.ReadAllTextAsync(versionFilePath)).Trim();
            if (Version.TryParse(versionString, out var version))
            {
                return version;
            }

            //throw new InvalidOperationException("Неверный формат версии в файле");
            return null;
        }

        public async Task UpdateCurrentVersionAsync(Version version)
        {
            var versionFilePath = Path.Combine(Environment.CurrentDirectory, _versionFileName);
            if (!File.Exists(versionFilePath))
            {
                var stream = File.Create(versionFilePath);
                stream.Close();
            }

            await File.WriteAllTextAsync(versionFilePath, version.ToString());
        }
    }
}
