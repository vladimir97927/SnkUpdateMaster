namespace SnkUpdateMaster.Core.Integrity
{
    /// <summary>
    /// Реализация провайдера целостности файлов с использованием SHA-256
    /// </summary>
    /// <remarks>
    /// Класс предоставляет:
    /// <list type="bullet">
    /// <item><description>Обертку над SHA-256 <see cref="ShaChecksumCalculator"/></description></item>
    /// <item><description>Дополнительную проверку существования файла</description></item>
    /// <item><description>Интеграцию с системой контроля целостности</description></item>
    /// </list>
    /// </remarks>
    public class ShaIntegrityProvider : IIntegrityProvider
    {
        private readonly ShaChecksumCalculator _checksumCalculator = new();

        /// <summary>
        /// Вычисляет SHA-256 контрольную сумму файла с предварительной проверкой существования
        /// </summary>
        /// <param name="filePath">
        /// Абсолютный путь к файлу
        /// </param>
        /// <returns>
        /// 64-символьная hex-строка в нижнем регистре. Пример:
        /// <code>"9f86d081884c7d659a2feaa0c55ad015..."</code>
        /// </returns>
        /// <exception cref="FileNotFoundException">
        /// Указанный файл не существует
        /// </exception>
        /// <exception cref="IOException">
        /// Ошибка чтения файла (недостаточно прав, файл заблокирован)
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// Нет разрешения на доступ к файлу
        /// </exception>
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
