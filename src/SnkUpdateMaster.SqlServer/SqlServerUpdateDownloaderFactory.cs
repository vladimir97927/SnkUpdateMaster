using SnkUpdateMaster.Core.Downloader;
using SnkUpdateMaster.SqlServer.Configuration.Data;

namespace SnkUpdateMaster.SqlServer
{
    public class SqlServerUpdateDownloaderFactory(string connectionString, string downloadsDir) : IUpdateDownloaderFactory
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory = new SqlConnectionFactory(connectionString);

        private readonly string _downloadsDir = downloadsDir;

        public IUpdateDownloader Create()
        {
            return new SqlServerUpdateDownloader(_sqlConnectionFactory, _downloadsDir);
        }
    }
}
