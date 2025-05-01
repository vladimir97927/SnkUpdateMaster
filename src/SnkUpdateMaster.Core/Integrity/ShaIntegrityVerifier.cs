namespace SnkUpdateMaster.Core.Integrity
{
    /// <summary>
    /// Реализация проверки целостности файлов с использованием SHA-256
    /// </summary>
    public class ShaIntegrityVerifier : IIntegrityVerifier
    {
        private readonly ShaChecksumCalculator _checksumCalculator = new();

        /// <summary>
        /// Проверяет целостность файла сравнением с эталонной контрольной суммой
        /// </summary>
        /// <param name="filePath">
        /// Абсолютный путь к проверяемому файлу
        /// </param>
        /// <param name="expectedChecksum">
        /// Ожидаемая контрольная сумма в HEX-формате. Пример:
        /// <code>"a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e"</code>
        /// </param>
        /// <returns>
        /// <c>true</c> - если вычисленный хеш совпадает с ожидаемым<br/>
        /// <c>false</c> - при несовпадении или ошибках вычислений
        /// </returns>
        /// <exception cref="FileNotFoundException">
        /// Указанный файл не существует
        /// </exception>
        /// <exception cref="IOException">
        /// Ошибка чтения файла (отказано в доступе, файл занят)
        /// </exception>
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
