using Microsoft.Data.SqlClient;
using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Downloader;
using SnkUpdateMaster.SqlServer.Database;

namespace SnkUpdateMaster.SqlServer
{
    /// <summary>
    /// Класс реализует загрузчик обновлений из Microsoft SQL Server.
    /// Сохраняет файлы обновлений из BLOB-поля таблицы в указанную директорию с поддержкой 
    /// прогресса и отмены.
    /// </summary>
    /// <param name="sqlConnectionFactory">Фабрика подключений к SQL Server</param>
    /// <param name="downloadsDir">Директория для сохранения файлов</param>
    public class SqlServerUpdateDownloader(
        ISqlConnectionFactory sqlConnectionFactory,
        string downloadsDir) : IUpdateDownloader
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

        private readonly string _downloadsDir = downloadsDir;

        /// <summary>
        /// Скачивает обновления в заданную директроию
        /// </summary>
        /// <param name="updateInfo">Метаданные запрашиваемого обновления</param>
        /// <param name="progress">Объект для отслеживания прогресса (0.0-1.0)</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Полный путь к скачанному файлу</returns>
        /// <exception cref="KeyNotFoundException">Файл обновления не найден в базе данных</exception>
        public async Task<string> DownloadUpdateAsync(UpdateInfo updateInfo, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
        {
            var connection = _sqlConnectionFactory.GetOpenConnection();
            using var command = new SqlCommand(
                "SELECT [FileData] " +
                "FROM [dbo].[UpdateFile] " +
                "WHERE [UpdateInfoId] = @UpdateInfoId", (SqlConnection)connection);
            command.Parameters.AddWithValue("@UpdateInfoId", updateInfo.Id);
            using var reader = await command.ExecuteReaderAsync(System.Data.CommandBehavior.SequentialAccess, cancellationToken);
            if (!await reader.ReadAsync(cancellationToken))
            {
                throw new KeyNotFoundException("Файл обновления не найден");
            }
            Directory.CreateDirectory(_downloadsDir);
            var filePath = Path.Combine(_downloadsDir, updateInfo.FileName);
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            var buffer = new byte[8192];
            long bytesReadTotal = 0;
            int bytesRead;
            using var blobStream = reader.GetStream(0);
            while ((bytesRead = await blobStream.ReadAsync(buffer, cancellationToken)) > 0)
            {
                await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);

                bytesReadTotal += bytesRead;

                if (progress != null && blobStream.CanSeek && blobStream.Length > 0)
                {
                    var percentage = (double)bytesReadTotal / blobStream.Length;
                    progress.Report(percentage);
                }
            }
            return filePath;
        }
    }
}
