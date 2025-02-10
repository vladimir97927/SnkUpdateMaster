using System.Security.Cryptography;

namespace SnkUpdateMaster.Core.Integrity
{
    public class ShaIntegrityVerifier : IIntegrityVerifier
    {
        public bool VerifyFile(string filePath, string expectedChecksum)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Файл не найден при проверке целостности", filePath);
            }

            using var sha256 = SHA256.Create();
            using var stream = File.OpenRead(filePath);

            var actualChecksum = BitConverter.ToString(sha256.ComputeHash(stream))
                .Replace("-", "")
                .ToLowerInvariant();

            return string.Equals(expectedChecksum, actualChecksum, StringComparison.OrdinalIgnoreCase);
        }
    }
}
