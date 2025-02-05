namespace SnkUpdateMaster.Application.FileSystem
{
    public interface IFileSystemService
    {
        string CreateSubDirIfDoesNotExists(string baseDir, string newDir);

        string CreateFileIfDoesNotExists(string dir, string fileName);

        Task<string> ReadTextFileAsync(string filePath);

        Task WriteFileAsync(string filePath, byte[] data);

        void ClearDirectory(string dir);

        IEnumerable<string> GetFiles(string dir);
    }
}
