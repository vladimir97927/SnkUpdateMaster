namespace SnkUpdateMaster.Core.Integrity
{
    /// <summary>
    /// Определяет интерфейса для проверки целостности файлов путем сравнения контрольных сумм
    /// </summary>
    public interface IIntegrityVerifier
    {
        /// <summary>
        /// Проверяет соответствие файла ожидаемой контрольной сумме
        /// </summary>
        /// <param name="filePath">
        /// Абсолютный путь к проверяемому файлу.
        /// Должен существовать и быть доступным для чтения.
        /// </param>
        /// <param name="expectedChecksum">
        /// Ожидаемая контрольная сумма
        /// </param>
        /// <returns>
        /// <c>true</c> - если вычисленная сумма совпадает с ожидаемой<br/>
        /// <c>false</c> - при несовпадении или ошибках верификации
        /// </returns>
        /// <exception cref="FileNotFoundException">
        /// Указанный файл не существует
        /// </exception>
        /// <exception cref="IOException">
        /// Ошибка доступа к файлу
        /// </exception>
        bool VerifyFile(string filePath, string expectedChecksum);
    }
}
