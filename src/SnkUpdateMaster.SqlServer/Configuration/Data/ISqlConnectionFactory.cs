using System.Data;

namespace SnkUpdateMaster.SqlServer.Configuration.Data
{
    public interface ISqlConnectionFactory
    {
        IDbConnection GetOpenConnection();
    }
}
