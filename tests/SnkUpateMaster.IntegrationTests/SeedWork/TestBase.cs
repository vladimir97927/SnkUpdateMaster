using Dapper;
using Microsoft.Data.SqlClient;
using System.Text;

namespace SnkUpateMaster.IntegrationTests.SeedWork
{
    internal class TestBase
    {
        protected string? ConnectionString { get; private set; }

        protected string DownloadsPath = "downloads";

        protected string AppDir = "testApp";

        [SetUp]
        public async Task BeforeEachTest()
        {
            ConnectionString = EnviromentVariablesProvider.GetConnectionStringEnviromentVariable();
            if (string.IsNullOrEmpty(ConnectionString))
            {
                throw new ApplicationException("Строка подключения отсутствует в переменных среды");
            }
            await ClearDatabase();
            await SeedDatabase();
            Directory.CreateDirectory(DownloadsPath);
            Directory.CreateDirectory(AppDir);
        }

        [TearDown]
        public async Task AfterEachTest()
        {
            await ClearDatabase();
            Directory.Delete(DownloadsPath, true);
            Directory.Delete(AppDir, true);
        }

        protected async Task ExecuteSqlScript(string scriptPath)
        {
            var sql = await File.ReadAllTextAsync(scriptPath);
            using var connection = new SqlConnection(ConnectionString);
            await connection.ExecuteScalarAsync(sql);
        }

        private async Task ClearDatabase()
        {
            await ExecuteSqlScript(@"SeedWork\ClearDatabase.sql");
        }

        private async Task SeedDatabase()
        {
            await ExecuteSqlScript(@"SeedWork\SeedDatabase.sql");
        }
    }
}
