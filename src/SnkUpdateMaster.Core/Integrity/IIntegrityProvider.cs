namespace SnkUpdateMaster.Core.Integrity
{
    public interface IIntegrityProvider
    {
        string ComputeFileChecksum(string filePath);
    }
}
