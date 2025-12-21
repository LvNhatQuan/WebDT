using Microsoft.Data.SqlClient;
using WebDT.Database;
using WebDT.Areas.Admin.Models;

namespace WebDT.Areas.Admin.DAL
{
    public class StaffAdminDAL
    {
        private readonly DbConnect _db = new DbConnect();

        // =========================
        // GET ALL STAFF
        // =========================
        public List<StaffAdmin> GetAll()
        {
            var list = new List<StaffAdmin>();

            string sql = @"SELECT id, username, email, full_name, phone_number, is_locked
                           FROM users
                           WHERE role = 'staff'
                           ORDER BY id DESC";

            using var con = new SqlConnection(_db.getConnecttion().ConnectionString);
            using var cmd = new SqlCommand(sql, con);
            con.Open();

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new StaffAdmin
                {
                    Id = (int)rd["id"],
                    Username = rd["username"].ToString()!,
                    Email = rd["email"].ToString()!,
                    FullName = rd["full_name"]?.ToString() ?? "",
                    PhoneNumber = rd["phone_number"]?.ToString() ?? "",
                    IsLocked = rd["is_locked"] != DBNull.Value && (bool)rd["is_locked"]
                });
            }

            return list;
        }

        // =========================
        // GET BY ID
        // =========================
        public StaffAdmin? GetById(int id)
        {
            string sql = @"SELECT id, username, email, full_name, phone_number, is_locked
                           FROM users
                           WHERE id = @id AND role = 'staff'";

            using var con = new SqlConnection(_db.getConnecttion().ConnectionString);
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", id);

            con.Open();
            using var rd = cmd.ExecuteReader();

            if (!rd.Read()) return null;

            return new StaffAdmin
            {
                Id = (int)rd["id"],
                Username = rd["username"].ToString()!,
                Email = rd["email"].ToString()!,
                FullName = rd["full_name"]?.ToString() ?? "",
                PhoneNumber = rd["phone_number"]?.ToString() ?? "",
                IsLocked = rd["is_locked"] != DBNull.Value && (bool)rd["is_locked"]
            };
        }

        // =========================
        // CREATE STAFF
        // =========================
        public bool Create(StaffAdmin s)
        {
            string sql = @"
INSERT INTO users (username, email, password, full_name, phone_number, role, is_locked)
VALUES (@u, @e, @p, @f, @ph, 'staff', 0)";

            using var con = new SqlConnection(_db.getConnecttion().ConnectionString);
            using var cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@u", s.Username);
            cmd.Parameters.AddWithValue("@e", s.Email);
            cmd.Parameters.AddWithValue("@p", s.Password ?? "");
            cmd.Parameters.AddWithValue("@f", s.FullName);
            cmd.Parameters.AddWithValue("@ph", s.PhoneNumber);

            con.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        // =========================
        // UPDATE STAFF
        // =========================
        public bool Update(StaffAdmin s)
        {
            string sql = @"
UPDATE users SET
    full_name = @f,
    phone_number = @ph,
    is_locked = @lock
WHERE id = @id AND role = 'staff'";

            using var con = new SqlConnection(_db.getConnecttion().ConnectionString);
            using var cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@id", s.Id);
            cmd.Parameters.AddWithValue("@f", s.FullName);
            cmd.Parameters.AddWithValue("@ph", s.PhoneNumber);
            cmd.Parameters.AddWithValue("@lock", s.IsLocked);

            con.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        // =========================
        // TOGGLE LOCK
        // =========================
        public bool ToggleLock(int id, bool lockState)
        {
            using var con = new SqlConnection(_db.getConnecttion().ConnectionString);
            using var cmd = new SqlCommand(
                "UPDATE users SET is_locked = @l WHERE id = @id AND role = 'staff'", con);

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@l", lockState);

            con.Open();
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
