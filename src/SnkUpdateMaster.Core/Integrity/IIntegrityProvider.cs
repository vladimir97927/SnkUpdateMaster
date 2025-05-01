namespace SnkUpdateMaster.Core.Integrity
{
    /// <summary>
    /// Определяет интерфейс для проверки целостности файлов через вычисление контрольных сумм
    /// </summary>
    public interface IIntegrityProvider
    {
        /// <summary>
        /// Вычисляет контрольную сумму файла для проверки целостности
        /// </summary>
        /// <param name="filePath">
        /// Абсолютный путь к проверяемому файлу. 
        /// Должен указывать на существующий файл с правами на чтение.
        /// </param>
        /// <returns>
        /// Строковое представление контрольной суммы в формате, определенном реализацией:
        /// </returns>
        string ComputeFileChecksum(string filePath);
    }
}
