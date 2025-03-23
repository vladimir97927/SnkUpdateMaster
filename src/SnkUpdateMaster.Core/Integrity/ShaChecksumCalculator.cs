using System.Security.Cryptography;

namespace SnkUpdateMaster.Core.Integrity
{
    public class ShaChecksumCalculator : IChecksumCalculator
    {
        public string ComputeChecksum(string filePath)
        {
            using var sha256 = SHA256.Create();
            using var stream = File.OpenRead(filePath);

            var checksum = BitConverter.ToString(sha256.ComputeHash(stream))
                .Replace("-", "")
                .ToLowerInvariant();

            return checksum;
        }
    }
}
