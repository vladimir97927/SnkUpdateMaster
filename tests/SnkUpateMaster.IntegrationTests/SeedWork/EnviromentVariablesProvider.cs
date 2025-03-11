namespace SnkUpateMaster.IntegrationTests.SeedWork
{
    public static class EnviromentVariablesProvider
    {
        public static string? GetConnectionStringEnviromentVariable()
        {
            const string variableName = "NET_SnkUpdateMaster_ConnectionString";
            var enviromentVariable = Environment.GetEnvironmentVariable(variableName);
            if (!string.IsNullOrEmpty(enviromentVariable))
            {
                return enviromentVariable;
            }
            enviromentVariable = Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.User);
            if (!string.IsNullOrEmpty(enviromentVariable))
            {
                return enviromentVariable;
            }

            return Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.Machine);
        }
    }
}
