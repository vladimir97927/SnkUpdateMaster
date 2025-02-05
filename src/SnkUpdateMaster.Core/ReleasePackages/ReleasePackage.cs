namespace SnkUpdateMaster.Core.ReleasePackages
{
    public class ReleasePackage
    {
        private string _versionName;

        private int _versionCode;

        private string _fileName;

        private byte[] _fileData;

        private ReleasePackage(int id, string versionName, int versionCode, string fileName, byte[] fileData)
        {
            Id = id;
            _versionName = versionName;
            _versionCode = versionCode;
            _fileName = fileName;
            _fileData = fileData;
        }

        public int Id { get; }

        public string FileName => _fileName;

        public byte[] FileData => _fileData;

        public static ReleasePackage CreateNew(
            string versionName,
            int versionCode,
            string fileName,
            byte[] fileData,
            IReleasePackageCounter releaseCounter)
        {
            var releaseCount = releaseCounter.CountReleasesWithCode(versionCode);
            if (releaseCount > 0)
                throw new Exception($"Релиз с кодом {versionCode} уже существует");
            
            return new ReleasePackage(default, versionName, versionCode, fileName, fileData);
        }
    }
}
