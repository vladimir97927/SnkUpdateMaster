using System.Security.Cryptography;

namespace SnkUpdateMaster.Core.Integrity
{
    /// <summary>
    /// Реализация расчета контрольных сумм с использованием алгоритма SHA-256
    /// </summary>
    /// <remarks>
    /// Особенности реализации:
    /// <list type="bullet">
    /// <item><description>Использует криптографический алгоритм SHA-256</description></item>
    /// <item><description>Возвращает хеш в формате lowercase hex без разделителей</description></item>
    /// <item><description>Автоматически освобождает ресурсы</description></item>
    /// </list>
    /// 
    /// Пример выходных данных: 
    /// <code>9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f121a2</code>
    /// </remarks>
    public class ShaChecksumCalculator : IChecksumCalculator
    {
        /// <summary>
        /// Вычисляет SHA-256 хеш для указанного файла
        /// </summary>
        /// <param name="filePath">
        /// Абсолютный путь к файлу. Файл должен существовать и быть доступным для чтения.
        /// </param>
        /// <returns>
        /// 64-символьная строка с hex-представлением хеша в нижнем регистре
        /// </returns>
        /// <exception cref="FileNotFoundException">
        /// Файл не существует
        /// </exception>
        /// <exception cref="IOException">
        /// Ошибка ввода-вывода при чтении файла
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// Нет прав на чтение файла
        /// </exception>
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
