namespace SnkUpdateMaster.Core
{
    public class UpdateInfo
    {
        public UpdateInfo(int id, Version version, string fileName, string checksum, DateTime releaseDate)
        {
            Id = id;
            Version = version;
            FileName = fileName;
            Checksum = checksum;
            ReleaseDate = releaseDate;
        }

        public UpdateInfo(int id, string version, string fileName, string checksum, DateTime releaseDate)
        {
            Id = id;
            Version = new Version(version);
            FileName = fileName;
            Checksum = checksum;
            ReleaseDate = releaseDate;
        }

        public int Id { get; }

        public Version Version { get; }

        public string FileName { get; }

        public string Checksum { get; }

        public DateTime ReleaseDate { get; }
    }
}
