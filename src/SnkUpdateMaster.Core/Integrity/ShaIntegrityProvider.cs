namespace SnkUpdateMaster.Core.Integrity
{
    public class ShaIntegrityProvider : IIntegrityProvider
    {
        private readonly ShaChecksumCalculator _checksumCalculator = new();

        public string ComputeFileChecksum(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Файл не найден при расчете контрольной суммы", filePath);
            }

            var checksum = _checksumCalculator.ComputeChecksum(filePath);

            return checksum;
        }
    }
}
