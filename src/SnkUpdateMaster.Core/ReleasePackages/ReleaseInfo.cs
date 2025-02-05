namespace SnkUpdateMaster.Core.ReleasePackages
{
    public class ReleaseInfo
    {
        private int _versionCode;

        private string _fileName;

        public ReleaseInfo(int versionCode, string fileName)
        {
            _versionCode = versionCode;
            _fileName = fileName;
        }

        public int VersionCode => _versionCode;

        public string FileName => _fileName;
    }
}
