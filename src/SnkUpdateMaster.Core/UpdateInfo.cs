namespace SnkUpdateMaster.Core
{
    public class UpdateInfo
    {
        public UpdateInfo(Version version, string fileName, string checksum, DateTime releaseDate)
        {
            Version = version;
            FileName = fileName;
            Checksum = checksum;
            ReleaseDate = releaseDate;
        }

        public Version Version { get; }

        public string FileName { get; }

        public string Checksum { get; }

        public DateTime ReleaseDate { get; }
    }
}
