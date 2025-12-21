using Microsoft.Data.SqlClient;
using WebDT.Database;
using WebDT.Areas.Admin.Models;

namespace WebDT.Areas.Admin.DAL
{
    public class AdminLogDAL
    {
        DbConnect db = new DbConnect();

        // ===============================
        // GHI LOG
        // ===============================
        public void InsertLog(int userId, string action, string module, string? description)
        {
            string sql = @"
INSERT INTO admin_logs (user_id, action, module, description)
VALUES (@uid, @act, @mod, @des)";

            using SqlConnection con =
                new SqlConnection(db.getConnecttion().ConnectionString);
            using SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@uid", userId);
            cmd.Parameters.AddWithValue("@act", action);
            cmd.Parameters.AddWithValue("@mod", module);
            cmd.Parameters.AddWithValue("@des", description ?? (object)DBNull.Value);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        // ===============================
        // DANH SÁCH LOG
        // ===============================
        public List<AdminLog> GetAll()
        {
            var list = new List<AdminLog>();

            string sql = @"
SELECT l.id, l.action, l.module, l.description, l.created_at,
       u.username, u.role
FROM admin_logs l
JOIN users u ON l.user_id = u.id
ORDER BY l.created_at DESC";

            using SqlConnection con =
                new SqlConnection(db.getConnecttion().ConnectionString);
            using SqlCommand cmd = new SqlCommand(sql, con);

            con.Open();
            using SqlDataReader rd = cmd.ExecuteReader();

            while (rd.Read())
            {
                list.Add(new AdminLog
                {
                    Id = (int)rd["id"],
                    Username = rd["username"].ToString()!,
                    Role = rd["role"].ToString()!,
                    Action = rd["action"].ToString()!,
                    Module = rd["module"].ToString()!,
                    Description = rd["description"]?.ToString(),
                    CreatedAt = Convert.ToDateTime(rd["created_at"])
                });
            }

            return list;
        }
    }
}
