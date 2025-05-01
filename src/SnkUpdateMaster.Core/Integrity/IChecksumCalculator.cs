namespace SnkUpdateMaster.Core.Integrity
{
    /// <summary>
    /// Определяет интерфейс для вычисления контрольных сумм файлов
    /// </summary>
    /// <remarks>
    /// Интерфейс предоставляет абстракцию для различных алгоритмов хеширования.
    /// Реализации должны гарантировать:
    /// <list type="bullet">
    /// <item><description>Корректность вычислений для больших файлов</description></item>
    /// <item><description>Потокобезопасность операций</description></item>
    /// </list>
    /// </remarks>
    public interface IChecksumCalculator
    {
        /// <summary>
        /// Вычисляет контрольную сумму для указанного файла
        /// </summary>
        /// <param name="filePath">
        /// Абсолютный путь к файлу для вычисления контрольной суммы.
        /// Файл должен существовать и быть доступным для чтения.
        /// </param>
        /// <returns>
        /// Строковое представление контрольной суммы в HEX-формате.
        /// </returns>
        /// <exception cref="FileNotFoundException">
        /// Указанный файл не существует
        /// </exception>
        /// <exception cref="IOException">
        /// Ошибка чтения файла
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// Нет прав на чтение файла
        /// </exception>
        string ComputeChecksum(string filePath);
    }
}
