using Microsoft.Data.SqlClient;
using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Downloader;
using SnkUpdateMaster.SqlServer.Database;

namespace SnkUpdateMaster.SqlServer
{
    public class SqlServerUpdateDownloader(
        ISqlConnectionFactory sqlConnectionFactory,
        string downloadsDir) : IUpdateDownloader
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

        private readonly string _downloadsDir = downloadsDir;

        public async Task<string> DownloadUpdateAsync(UpdateInfo updateInfo, IProgress<double> progress, CancellationToken cancellationToken = default)
        {
            var connection = _sqlConnectionFactory.GetOpenConnection();
            using var command = new SqlCommand(
                "SELECT [FileData] " +
                "FROM [dbo].[AppUpdates] " +
                "WHERE [Id] = @Id", (SqlConnection)connection);
            command.Parameters.AddWithValue("@Id", updateInfo.Id);
            using var reader = await command.ExecuteReaderAsync(System.Data.CommandBehavior.SequentialAccess, cancellationToken);
            if (!await reader.ReadAsync(cancellationToken))
            {
                throw new Exception("Файл обновления не найден");
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

                if (blobStream.CanSeek && blobStream.Length > 0)
                {
                    var percentage = (double)bytesReadTotal / blobStream.Length;
                    progress.Report(percentage);
                }
            }
            return filePath;
        }
    }
}
