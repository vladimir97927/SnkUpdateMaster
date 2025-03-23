namespace SnkUpdateMaster.Core.Integrity
{
    public class ShaIntegrityVerifier : IIntegrityVerifier
    {
        private readonly ShaChecksumCalculator _checksumCalculator = new();

        public bool VerifyFile(string filePath, string expectedChecksum)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Файл не найден при проверке целостности", filePath);
            }

            var actualChecksum = _checksumCalculator.ComputeChecksum(filePath);

            return string.Equals(expectedChecksum, actualChecksum, StringComparison.OrdinalIgnoreCase);
        }
    }
}
