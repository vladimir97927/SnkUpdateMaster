namespace SnkUpdateMaster.Core
{
    /// <summary>
    /// Представляет информацию о программном обновлении.
    /// </summary>
    /// <remarks>
    /// Этот класс хранит метаданные обновления, включая версию, контрольную сумму файла и дату выпуска.
    /// </remarks>
    public class UpdateInfo
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="UpdateInfo"/> с использованием класса версии <see cref="System.Version"/>.
        /// </summary>
        /// <param name="id">Уникальный идентификатор обновления.</param>
        /// <param name="version">Версия обновления.</param>
        /// <param name="fileName">Имя файла обновления.</param>
        /// <param name="checksum">Контрольная сумма файла в HEX-формате.</param>
        /// <param name="releaseDate">Дата и время выпуска обновления.</param>
        public UpdateInfo(
            int id,
            Version version,
            string fileName,
            string checksum,
            DateTime releaseDate,
            string? fileDir = null)
        {
            Id = id;
            Version = version;
            FileName = fileName;
            Checksum = checksum;
            ReleaseDate = releaseDate;
            FileDir = fileDir;
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="UpdateInfo"/> с использованием строкового представления версии.
        /// </summary>
        /// <param name="id">Уникальный идентификатор обновления.</param>
        /// <param name="version">Строковое представление версии (формат: major.minor.build).</param>
        /// <param name="fileName">Имя файла обновления.</param>
        /// <param name="checksum">Контрольная сумма файла в HEX-формате.</param>
        /// <param name="releaseDate">Дата и время выпуска обновления.</param>
        /// <exception cref="System.ArgumentException">Возникает при недопустимом формате версии.</exception>
        public UpdateInfo(
            int id,
            string version,
            string fileName,
            string checksum,
            DateTime releaseDate,
            string? fileDir = null)
        {
            Id = id;
            Version = new Version(version);
            FileName = fileName;
            Checksum = checksum;
            ReleaseDate = releaseDate;
            FileDir = fileDir;
        }

        /// <summary>
        /// Уникальный идентификатор обновления.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Версия обновления в формате major.minor.build.
        /// </summary>
        public Version Version { get; }

        /// <summary>
        /// Имя файла обновления (включая расширение).
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Директория с файлом обновления (например, если он расположен в файловом хранилище)
        /// </summary>
        public string? FileDir { get; }

        /// <summary>
        /// Контрольная сумма файла для проверки целостности.
        /// </summary>
        public string Checksum { get; }

        /// <summary>
        /// Дата и время публикации обновления в UTC.
        /// </summary>
        public DateTime ReleaseDate { get; }
    }
}
