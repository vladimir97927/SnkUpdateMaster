using SnkUpdateMaster.Core.ReleasePublisher;
using SnkUpdateMaster.SqlServer.Database;

namespace SnkUpdateMaster.SqlServer
{
    /// <summary>
    /// Класс реализует хранилище релизов на основе Microsoft SQL Server
    /// с использованием Entity Framework Core. Обеспечивает CRUD-операции
    /// с релизами через контекст базы данных.
    /// </summary>
    /// <param name="context">Контекст Entity Framework Core для работы с базой данных</param>
    public class SqlServerReleaseSource(SnkUpdateMasterContext context) : IReleaseSource
    {
        private readonly SnkUpdateMasterContext _context = context;

        /// <summary>
        /// Загрузка релиза по значению уникального идентификатора в базе данных
        /// </summary>
        /// <param name="id">Уникальный идентификатор релиза</param>
        /// <returns>Найденный релиз <see cref="Release"/> или null, если релиз отсутствует
        /// в базе данных</returns>
        public async Task<Release?> GetReleaseAsync(int id)
        {
            return await _context.Releases.FindAsync(id);
        }

        /// <summary>
        /// Удаления данных о релизе из базы данных
        /// </summary>
        /// <param name="release">Объект <see cref="Release"/></param>
        public async Task RemoveReleaseAsync(Release release)
        {
            _context.Remove(release);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Загрузка релиза в базу данных
        /// </summary>
        /// <param name="release">Объект <see cref="Release"/></param>
        /// <returns></returns>
        public async Task UploadReleaseAsync(Release release)
        {
            await _context.AddAsync(release);
            await _context.SaveChangesAsync();
        }
    }
}
