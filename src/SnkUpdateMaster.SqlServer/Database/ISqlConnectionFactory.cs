using System.Data;

namespace SnkUpdateMaster.SqlServer.Database
{
    public interface ISqlConnectionFactory
    {
        IDbConnection GetOpenConnection();
    }
}
