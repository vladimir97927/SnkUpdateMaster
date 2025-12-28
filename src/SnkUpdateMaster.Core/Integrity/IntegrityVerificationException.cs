namespace SnkUpdateMaster.Core.Integrity
{
    public class IntegrityVerificationException : Exception
    {
        public IntegrityVerificationException(string filePath, string expectedChecksum)
            : base($"Integrity verification failed for file '{filePath}'. Expected checksum: {expectedChecksum}.")
        {
            FilePath = filePath;
            ExpectedChecksum = expectedChecksum;
        }

        public string FilePath { get; }

        public string ExpectedChecksum { get; }
    }
}
