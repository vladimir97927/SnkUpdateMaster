namespace SnkUpdateMaster.SqlServer.IntegrationTests.SeedWork
{
    internal static class IntegrationTestConfig
    {
        public static string SqlConnectionString =>
            Environment.GetEnvironmentVariable("SNK_UPDATE_MASTER__SQL_CONN")
                ?? "Server=localhost,1455;Database=SnkUpdateMasterDb;User Id=sa;Password=Snk@12345;Encrypt=False;TrustServerCertificate=True";
    }
}
