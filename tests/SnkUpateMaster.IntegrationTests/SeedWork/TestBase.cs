﻿using Dapper;
using Microsoft.Data.SqlClient;
using System.Text;

namespace SnkUpateMaster.IntegrationTests.SeedWork
{
    internal class TestBase
    {
        protected string? ConnectionString { get; private set; }

        protected string DownloadsPath = "downloads";

        protected string AppDir = "testApp";

        protected string VersionFileName = "version";

        protected Version CurrentVersion = new("1.0.0");

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
            await CreateCurrentVersionFile();
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

        private async Task CreateCurrentVersionFile()
        {
            if (File.Exists(VersionFileName))
            {
                File.Delete(VersionFileName);
            }

            var stream = File.Create(VersionFileName);
            stream.Close();
            await File.WriteAllTextAsync(VersionFileName, CurrentVersion.ToString());
        }
    }
}
