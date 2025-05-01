namespace SnkUpdateMaster.Core.ReleasePublisher.Packager
{
    /// <summary>
    /// Интерфейс для упаковки релизов из исходных файлов
    /// </summary>
    public interface IReleasePackager
    {
        /// <summary>
        /// Асинхронно упаковывает релиз из исходной директории
        /// </summary>
        /// <param name="sourceDir">Исходная директория с файлами для релиза</param>
        /// <param name="destDir">Целевая директория для сохранения пакета</param>
        /// <param name="version">Версия создаваемого релиза</param>
        /// <param name="progress">Объект для отслеживания прогресса (0.0-1.0)</param>
        /// <returns>Объект <see cref="Release"/></returns>
        Task<Release> PackAsync(string sourceDir, string destDir, Version version, IProgress<double> progress);
    }
}
