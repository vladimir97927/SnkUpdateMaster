
namespace SnkUpdateMaster.Application.FileSystem
{
    public class FileSystemService : IFileSystemService
    {
        public void ClearDirectory(string dir)
        {
            var dirInfo = new DirectoryInfo(dir);
            foreach (var file in dirInfo.GetFiles())
            {
                file.Delete();
            }
            foreach (var d in dirInfo.GetDirectories())
            {
                d.Delete();
            }
        }

        public string CreateFileIfDoesNotExists(string dir, string fileName)
        {
            var filePath = Path.Combine(dir, fileName);
            File.Create(filePath);
            return filePath;
        }

        public string CreateSubDirIfDoesNotExists(string baseDir, string newDir)
        {
            var dirInfo = new DirectoryInfo(Path.Combine(baseDir, newDir));
            if (!dirInfo.Exists)
                dirInfo.Create();
            return dirInfo.FullName;
        }

        public IEnumerable<string> GetFilesAsync(string dir)
        {
            var dirInfo = new DirectoryInfo(dir);
            return dirInfo.GetFiles().Select(x => x.FullName);
        }

        public async Task<string> ReadTextFileAsync(string filePath)
        {
            return await File.ReadAllTextAsync(filePath);
        }

        public async Task WriteFileAsync(string filePath, byte[] data)
        {
            await File.WriteAllBytesAsync(filePath, data); 
        }
    }
}
