namespace SnkUpdateMaster.Core
{
    public class FileChange
    {
        public FileChange(string path, ChangeType changeType)
        {
            Path = path;
            ChangeType = changeType;
        }

        public string Path { get; }

        public ChangeType ChangeType { get; }
    }
}
