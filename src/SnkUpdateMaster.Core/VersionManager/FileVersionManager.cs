
namespace SnkUpdateMaster.Core.VersionManager
{
    /// <summary>
    /// Реализация менеджера версий, использующая файловую систему для хранения информации
    /// </summary>
    /// <remarks>
    /// Класс обеспечивает:
    /// <list type="bullet">
    /// <item><description>Хранение версии в текстовом файле формата "major.minor.build"</description></item>
    /// <item><description>Автоматическое создание файла версии при первом обновлении</description></item>
    /// </list>
    /// 
    /// Пример файла version:
    /// <code>1.2.3</code>
    /// 
    /// <para>Расположение файла: {текущая директория приложения}/version</para>
    /// </remarks>
    public class FileVersionManager : ICurrentVersionManager
    {
        private readonly string _versionFileName = "version";

        /// <summary>
        /// Получает текущую версию из файла
        /// </summary>
        /// <returns>
        /// <para>Version - объект версии при успешном чтении и парсинге</para>
        /// <para>null - в случаях:</para>
        /// <list type="bullet">
        /// <item><description>Файл версии не существует</description></item>
        /// <item><description>Неверный формат данных в файле</description></item>
        /// <item><description>Ошибка доступа к файлу</description></item>
        /// </list>
        /// </returns>
        /// <exception cref="UnauthorizedAccessException">
        /// Нет прав на чтение файла
        /// </exception>
        /// <exception cref="IOException">
        /// Ошибка ввода-вывода при работе с файлом
        /// </exception>
        public async Task<Version?> GetCurrentVersionAsync(CancellationToken cancellationToken = default)
        {
            var versionFilePath = Path.Combine(Environment.CurrentDirectory, _versionFileName);
            if (!File.Exists(versionFilePath))
            {
                return null;
            }

            var versionString = (await File.ReadAllTextAsync(versionFilePath, cancellationToken)).Trim();
            if (Version.TryParse(versionString, out var version))
            {
                return version;
            }

            return null;
        }

        /// <summary>
        /// Обновляет версию в файле
        /// </summary>
        /// <param name="version">Объект новой версии</param>
        /// <exception cref="IOException">
        /// Ошибка записи в файл версии
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// Нет прав на запись в файл
        /// </exception>
        /// <remarks>
        /// Особенности реализации:
        /// <list type="bullet">
        /// <item><description>Создает файл версии при первом вызове</description></item>
        /// <item><description>Полностью перезаписывает содержимое файла</description></item>
        /// </list>
        /// </remarks>
        public async Task UpdateCurrentVersionAsync(Version version, CancellationToken cancellationToken = default)
        {
            var versionFilePath = Path.Combine(Environment.CurrentDirectory, _versionFileName);
            if (!File.Exists(versionFilePath))
            {
                var stream = File.Create(versionFilePath);
                stream.Close();
            }

            await File.WriteAllTextAsync(versionFilePath, version.ToString(), cancellationToken);
        }
    }
}
