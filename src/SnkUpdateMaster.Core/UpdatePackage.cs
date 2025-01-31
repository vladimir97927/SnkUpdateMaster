namespace SnkUpdateMaster.Core
{
    public class UpdatePackage
    {
        public UpdatePackage(string versionName, int versionCode, List<FileChange> fileChanges)
        {
            VersionName = versionName;
            VersionCode = versionCode;
            FileChanges = fileChanges;
        }

        public string VersionName { get; }

        public int VersionCode { get; }

        public List<FileChange> FileChanges { get; }
    }
}
