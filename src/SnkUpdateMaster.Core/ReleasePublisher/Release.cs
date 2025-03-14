namespace SnkUpdateMaster.Core.ReleasePublisher
{
    public class Release(
        int id,
        Version version,
        string fileName,
        string checksum,
        byte[] fileData)
    {
        public int Id { get; } = id;

        public Version Version { get; } = version;

        public string FileName { get; } = fileName;

        public string Checksum { get; } = checksum;

        public DateTime ReleaseDate { get; } = DateTime.UtcNow;

        public byte[] FileData { get; } = fileData;
    }
}
