using Microsoft.Extensions.Logging;
using NUnit.Framework;
using SnkUpdateMaster.Core;
using SnkUpdateMaster.SqlServer.Configuration;
using SnkUpdateMaster.SqlServer.Database;
using SnkUpdateMaster.SqlServer.IntegrationTests.SeedWork;
using System.Collections.Concurrent;

namespace SnkUpdateMaster.SqlServer.IntegrationTests
{
    [TestFixture]
    class UpdateManagerTests : TestBase
    {
        [Test]
        public async Task CheckAndInstallUpdateTest()
        {
            var sqlConnectionFactory = new SqlConnectionFactory(ConnectionString);
            var loggerFactory = new TestLoggerFactory();

            var updateManager = new UpdateManagerBuilder()
                .WithFileCurrentVersionManager()
                .WithSha256IntegrityVerifier()
                .WithZipInstaller(AppDir)
                .WithSqlServerUpdateInfoProvider(sqlConnectionFactory)
                .WithSqlServerUpdateDownloader(sqlConnectionFactory, DownloadsPath)
                .WithLogger(loggerFactory)
                .Build();

            var mockProgress = new Progress<double>();
            var isSuccess = await updateManager.CheckAndInstallUpdatesAsync(mockProgress);

            Assert.That(isSuccess, Is.True);

            var newVersion = await updateManager.GetCurrentVersionAsync();

            var categoryName = typeof(UpdateManager).FullName ?? string.Empty;
            var updateManagerLogger = (TestLogger<object>)loggerFactory.Loggers[categoryName];

            Assert.That(newVersion, Is.Not.Null);
            Assert.That(newVersion, Is.EqualTo(new Version("1.0.2")));
            Assert.That(updateManagerLogger, Is.Not.Null);
            Assert.That(updateManagerLogger.Entries, Is.Not.Empty);
            Assert.That(updateManagerLogger.Entries.Any(e => e.Message.Contains("Update process finished successfully")), Is.True);
        }

        private sealed class TestLoggerFactory : ILoggerFactory
        {
            public ConcurrentDictionary<string, ILogger> Loggers { get; } = [];

            public void AddProvider(ILoggerProvider provider)
            {

            }
            public ILogger CreateLogger(string categoryName)
            {
                var logger = new TestLogger<object>();
                Loggers.AddOrUpdate(categoryName, logger, (key, existingLogger) => logger);
                return logger;
            }
            public void Dispose()
            {

            }
        }

        private sealed class TestLogger<T> : ILogger<T>
        {
            public IList<LogEntry> Entries { get; } = [];

            public IDisposable? BeginScope<TState>(TState state) where TState : notnull
            {
                return NullScope.Instance;
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                Entries.Add(new LogEntry(logLevel, formatter(state, exception), exception));
            }

            public record LogEntry(LogLevel Level, string Message, Exception? Exception);

            private sealed class NullScope : IDisposable
            {
                public static NullScope Instance = new();

                public void Dispose()
                {
                }
            }
        }
    }
}
