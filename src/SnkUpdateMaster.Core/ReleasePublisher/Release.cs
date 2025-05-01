namespace SnkUpdateMaster.Core.ReleasePublisher
{
    /// <summary>
    /// Предоставляет информацию о релизе
    /// </summary>
    public class Release(
        int id,
        Version version,
        string fileName,
        string checksum,
        byte[] fileData)
    {
        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        public int Id { get; } = id;

        /// <summary>
        /// Версия релиза 
        /// </summary>
        public Version Version { get; } = version;

        /// <summary>
        /// Имя файла релиза 
        /// </summary>
        public string FileName { get; } = fileName;

        /// <summary>
        /// Значение контрольной суммы
        /// </summary>
        public string Checksum { get; } = checksum;

        /// <summary>
        /// Дата и время выпуска релиза в UTC
        /// </summary>
        public DateTime ReleaseDate { get; } = DateTime.UtcNow;

        /// <summary>
        /// Бинарные данные файла релиза
        /// </summary>
        public byte[] FileData { get; } = fileData;

        /// <summary>
        /// Фабричный метод создания нового релиза
        /// </summary>
        /// <param name="version">Версия релиза</param>
        /// <param name="fileName">Имя файла релиза</param>
        /// <param name="checksum">Контрольная сумма файла релиза</param>
        /// <param name="fileData">Бинарные данные файла релиза</param>
        /// <returns></returns>
        public static Release CreateNew(Version version, string fileName, string checksum, byte[] fileData)
        {
            return new Release(default, version, fileName, checksum, fileData);
        }
    }
}
