namespace SnkUpdateMaster.Ftp.IntegrationTests.SeedWork
{
    internal static class IntegrationTestConfig
    {
        public static (string host, int port, string user, string pass) Ftp =>
        (
            Environment.GetEnvironmentVariable("SNK_UPDATE_MASTER__FTP_HOST") ?? "localhost",
            int.TryParse(Environment.GetEnvironmentVariable("SNK_UPDATE_MASTER__FTP_PORT"), out var port) ? port : 2121,
            Environment.GetEnvironmentVariable("SNK_UPDATE_MASTER__FTP_USER") ?? "snk",
            Environment.GetEnvironmentVariable("SNK_UPDATE_MASTER__FTP_PASS") ?? "snk@12345"
        );
    }
}
