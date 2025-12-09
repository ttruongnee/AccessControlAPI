using AccessControlAPI.Database;
using AccessControlAPI.Models;
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

        public Function GetById(string id)
        {
            using (var conn = _oracleDb.GetConnection())
            {
                string sql = "select * from functions where id = :id";
                return conn.QueryFirstOrDefault<Function>(sql, new { id });
            }
        }
    }
}
