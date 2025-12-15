using AccessControlAPI.Database;
using AccessControlAPI.Models;
using AccessControlAPI.Repositories.Interface;
using Dapper;

namespace AccessControlAPI.Repositories
{
    public class UserFunctionRepository : IUserFunctionRepository
    {
        private readonly IOracleDb _oracleDb;
        public UserFunctionRepository(IOracleDb oracleDb)
        {
            _oracleDb = oracleDb;
        }

        ////gán quyền cho user
        //public bool AddFunctionsForUser(int userId, List<string> functionIds)
        //{
        //    using (var conn = _oracleDb.GetConnection())
        //    {
        //        conn.Open();
        //        using(var transaction = conn.BeginTransaction())
        //        {   
        //            try
        //            {
        //                string sql = "insert into users_functions (user_id, function_id) values (:userId, :functionId)";
        //                foreach (var functionId in functionIds)
        //                {
        //                    conn.Execute(sql, new { userId, functionId }, transaction: transaction);
        //                }
        //                transaction.Commit();
        //                return true;
        //            }
        //            catch (Exception e)
        //            {
        //                transaction.Rollback();
        //                throw;
        //                //return false;
        //            }
        //        }
        //    }
        //}

        //xoá toàn bộ quyền của user

        public bool DeleteFunctionsFromUser(int userId)
        {
            using (var conn = _oracleDb.GetConnection())
            {
                string sql = "delete from users_functions where user_id = :userId";
                return conn.Execute(sql, new { userId }) > 0;
            }
        }

        //cập nhật quyền của user
        public bool UpdateFunctionsForUser(int userId, List<string> functionIds)
        {
            using (var conn = _oracleDb.GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // xóa chức năng cũ
                        string deleteSql = "DELETE FROM users_functions WHERE user_id = :userId";
                        conn.Execute(deleteSql, new { userId }, transaction: transaction);

                        // chèn chức năng mới (nếu có)
                        if (functionIds != null && functionIds.Count > 0)
                        {
                            string insertSql = "INSERT INTO users_functions (user_id, function_id) VALUES (:userId, :functionId)";

                            foreach (var functionId in functionIds)
                            {
                                conn.Execute(insertSql, new { userId, functionId = functionId.Trim().ToUpper() }, transaction: transaction);
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                        //return false;
                    }
                }
            }
        }


        //lấy danh sách quyền của user
        public List<Function> GetFunctionsByUserId(int userId)
        {
            using (var conn = _oracleDb.GetConnection())
            {
                string sql = @"
                                SELECT f.*
                                FROM functions f
                                INNER JOIN users_functions uf ON f.id = uf.function_id
                                WHERE uf.user_id = :userId";

                return conn.Query<Function>(sql, new { userId }).ToList();
            }
        }
    }
}
