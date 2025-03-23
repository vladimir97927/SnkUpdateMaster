namespace SnkUpdateMaster.Core.Integrity
{
    public interface IChecksumCalculator
    {
        string ComputeChecksum(string filePath);
    }
}
