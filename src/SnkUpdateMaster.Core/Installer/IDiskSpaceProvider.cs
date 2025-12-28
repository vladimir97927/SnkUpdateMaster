namespace SnkUpdateMaster.Core.Installer
{
    public interface IDiskSpaceProvider
    {
        long GetAvailableFreeSpace(string path);
    }

    internal class DiskSpaceProvider : IDiskSpaceProvider
    {
        public long GetAvailableFreeSpace(string path)
        {
            var fullPath = Path.GetFullPath(path);
            var root = Path.GetPathRoot(fullPath);
            if (string.IsNullOrEmpty(root))
            {
                throw new ArgumentException($"Unable to determine the root directory for the path {path}", nameof(path));
            }

            var driveInfo = new DriveInfo(root);
            return driveInfo.AvailableFreeSpace;
        }
    }
}
