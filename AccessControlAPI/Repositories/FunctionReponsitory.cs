using AccessControlAPI.Database;
using AccessControlAPI.Models;
using AccessControlAPI.Repositories.Interface;
using Dapper;

namespace AccessControlAPI.Repositories
{
    public class FunctionReponsitory : IFunctionRepository
    {
        private readonly IOracleDb _oracleDb;
        public FunctionReponsitory(IOracleDb oracleDb)
        {
            _oracleDb = oracleDb;
        }
        public List<Function> GetAll()
        {
            using (var conn = _oracleDb.GetConnection())
            {
                string sql = "select * from functions";
                return conn.Query<Function>(sql).ToList();
            }
        }
    }
}
