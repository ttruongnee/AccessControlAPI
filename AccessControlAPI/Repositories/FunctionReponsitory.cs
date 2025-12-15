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

        public Function GetById(string id)
        {
            using (var conn = _oracleDb.GetConnection())
            {
                string sql = "select * from functions where id = :id";
                return conn.QueryFirstOrDefault<Function>(sql, new { id });
            }
        }

        public List<Function> GetChildren(string parentId)
        {
            using (var conn = _oracleDb.GetConnection())
            {
                string query = @"SELECT * FROM functions WHERE parent_id = :parentId ORDER BY sort_order";
                return conn.Query<Function>(query, new { parentId }).ToList();
            }
        }

        public List<Function> GetParents()
        {
            using (var conn = _oracleDb.GetConnection())
            {
                string sql = "select * from functions where parent_id is null order by sort_order";
                return conn.Query<Function>(sql).ToList();
            }
        }
    }
}
