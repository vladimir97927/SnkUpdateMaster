namespace SnkUpdateMaster.Core.Parser
{
    /// <summary>
    /// Определяет интерфейс для парсинга файлов с информацией об обновлениях
    /// </summary>
    public interface IUpdateInfoParser
    {
        /// <summary>
        /// Парсит файл с информацией об обновлениях и возвращает объект UpdateInfo
        /// </summary>
        /// <param name="filePath">Путь до файла с информацией об обновлениях</param>
        /// <returns>Объект <see cref="UpdateInfo"/></returns>
        UpdateInfo Parse(string filePath);

        /// <summary>
        /// Парсит массив байтов с информацией об обновлениях и возвращает объект UpdateInfo
        /// </summary>
        /// <param name="dataBytes">Массив байтов, представляющий данные об обновлении. Не может быть пустым или иметь значение NULL</param>
        /// <returns>An <see cref="UpdateInfo"/>Объект <see cref="UpdateInfo"/></returns>
        UpdateInfo Parse(byte[] dataBytes);
    }
}
