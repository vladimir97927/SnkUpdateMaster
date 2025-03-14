namespace SnkUpdateMaster.Core.ReleasePublisher
{
    public class ReleaseInfo
    {
        public int Id { get; set; }

        public string Version { get; set; } = null!;

        public DateTime ReleaseDate {  get; set; }
    }
}
