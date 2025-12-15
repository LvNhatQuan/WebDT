using Microsoft.Data.SqlClient;
using WebDT.Database;
using WebDT.Models;

namespace WebDT.DAL
{
    public class UserDAL
    {
        private readonly DbConnect db = new DbConnect();

        private User MapUser(SqlDataReader r)
        {
            return new User
            {
                Id = Convert.ToInt32(r["id"]),
                Username = r["username"]?.ToString() ?? "",
                Email = r["email"]?.ToString() ?? "",
                Password = r["password"]?.ToString() ?? "",
                FullName = r["full_name"]?.ToString() ?? "",
                PhoneNumber = r["phone_number"]?.ToString() ?? "",
                Role = r["role"]?.ToString() ?? "customer",
                Address = r["address"]?.ToString() ?? "",
                Avatar = r["avatar"]?.ToString() ?? "",
                IsActive = r["is_active"] != DBNull.Value && Convert.ToBoolean(r["is_active"]),
                IsLocked = r["is_locked"] != DBNull.Value && Convert.ToBoolean(r["is_locked"]),
                CreatedAt = r["created_at"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(r["created_at"])
            };
        }

        public User? GetUserByEmail(string email)
        {
            db.openConnection();
            User? user = null;

            using (var cmd = new SqlCommand(
                "SELECT * FROM users WHERE email=@Email",
                db.getConnecttion()))
            {
                cmd.Parameters.AddWithValue("@Email", email);

                using var r = cmd.ExecuteReader();
                if (r.Read()) user = MapUser(r);
            }

            db.closeConnection();
            return user;
        }

        public User? GetUserByUsername(string username)
        {
            db.openConnection();
            User? user = null;

            using (var cmd = new SqlCommand(
                "SELECT * FROM users WHERE username=@Username",
                db.getConnecttion()))
            {
                cmd.Parameters.AddWithValue("@Username", username);

                using var r = cmd.ExecuteReader();
                if (r.Read()) user = MapUser(r);
            }

            db.closeConnection();
            return user;
        }

        public User? GetUserById(int id)
        {
            db.openConnection();
            User? user = null;

            using (var cmd = new SqlCommand(
                "SELECT * FROM users WHERE id=@Id",
                db.getConnecttion()))
            {
                cmd.Parameters.AddWithValue("@Id", id);

                using var r = cmd.ExecuteReader();
                if (r.Read()) user = MapUser(r);
            }

            db.closeConnection();
            return user;
        }

        public bool CreateUser(User u)
        {
            db.openConnection();

            using (var cmd = new SqlCommand(@"
INSERT INTO users (username, email, password, full_name, phone_number, role, address, avatar, is_active, is_locked, created_at)
VALUES (@Username, @Email, @Password, @FullName, @Phone, @Role, @Address, @Avatar, 1, 0, GETDATE())",
                db.getConnecttion()))
            {
                cmd.Parameters.AddWithValue("@Username", u.Username ?? "");
                cmd.Parameters.AddWithValue("@Email", u.Email ?? "");
                cmd.Parameters.AddWithValue("@Password", u.Password ?? ""); // nên hash nếu có lib
                cmd.Parameters.AddWithValue("@FullName", u.FullName ?? "");
                cmd.Parameters.AddWithValue("@Phone", u.PhoneNumber ?? "");
                cmd.Parameters.AddWithValue("@Role", u.Role ?? "customer");
                cmd.Parameters.AddWithValue("@Address", u.Address ?? "");
                cmd.Parameters.AddWithValue("@Avatar", u.Avatar ?? "");

                int result = cmd.ExecuteNonQuery();
                db.closeConnection();
                return result > 0;
            }
        }

        public bool UpdateProfile(User u)
        {
            db.openConnection();

            using (var cmd = new SqlCommand(@"
UPDATE users SET
    full_name=@FullName,
    email=@Email,
    phone_number=@Phone
WHERE id=@Id",
                db.getConnecttion()))
            {
                cmd.Parameters.AddWithValue("@Id", u.Id);
                cmd.Parameters.AddWithValue("@FullName", u.FullName ?? "");
                cmd.Parameters.AddWithValue("@Email", u.Email ?? "");
                cmd.Parameters.AddWithValue("@Phone", u.PhoneNumber ?? "");

                int result = cmd.ExecuteNonQuery();
                db.closeConnection();
                return result > 0;
            }
        }
    }
}
