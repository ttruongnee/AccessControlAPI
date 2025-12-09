using System.Data;

namespace AccessControlAPI.Database
{
    public interface IOracleDb
    {
        IDbConnection GetConnection();
    }
}
