namespace SnkUpdateMaster.Core.Integrity
{
    public interface IIntegrityVerifier
    {
        bool VerifyFile(string filePath, string expectedChecksum);
    }
}
