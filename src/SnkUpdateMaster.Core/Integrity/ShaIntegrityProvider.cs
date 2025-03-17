using System.Security.Cryptography;

namespace SnkUpdateMaster.Core.Integrity
{
    public class ShaIntegrityProvider : IIntegrityProvider
    {
        public string ComputeFileChecksum(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Файл не найден при расчете контрольной суммы", filePath);
            }

            using var sha256 = SHA256.Create();
            using var stream = File.OpenRead(filePath);

            var checksum = BitConverter.ToString(sha256.ComputeHash(stream))
                .Replace("-", "")
                .ToLowerInvariant();

            return checksum;
        }
    }
}
