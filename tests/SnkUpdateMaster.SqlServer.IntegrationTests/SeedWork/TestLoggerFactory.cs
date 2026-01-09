using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace SnkUpdateMaster.SqlServer.IntegrationTests.SeedWork
{
    internal sealed class TestLoggerFactory : ILoggerFactory
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

    internal sealed class TestLogger<T> : ILogger<T>
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
