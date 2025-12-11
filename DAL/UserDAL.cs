using Microsoft.Data.SqlClient;
using WebDT.Models;
using WebDT.Database;
using WebDT.Models;

namespace WebDT.DAL
{
    public class UserDAL
    {
        private readonly DbConnect db = new DbConnect();

        // ============================
        // MAP USER TỪ READER
        // ============================
        private User MapUser(SqlDataReader r)
        {
            return new User
            {
                Id = Convert.ToInt32(r["id"]),
                Username = r["username"].ToString(),
                Email = r["email"].ToString(),
                Password = r["password"].ToString(),
                FullName = r["full_name"].ToString(),
                PhoneNumber = r["phone_number"].ToString(),
                Role = r["role"].ToString(),
                IsLocked = false
            };
        }

        // ============================
        // LẤY USER THEO EMAIL (LOGIN)
        // ============================
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

        // ============================
        // LẤY USER THEO USERNAME
        // ============================
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

        // ============================
        // LẤY USER THEO ID
        // ============================
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

        // ============================
        // TẠO USER MỚI (REGISTER)
        // ============================
        public bool CreateUser(User u)
        {
            db.openConnection();

            using (var cmd = new SqlCommand(@"
                INSERT INTO users (username, email, password, full_name, phone_number, role)
                VALUES (@Username, @Email, @Password, @FullName, @Phone, @Role)",
                db.getConnecttion()))
            {
                cmd.Parameters.AddWithValue("@Username", u.Username);
                cmd.Parameters.AddWithValue("@Email", u.Email);
                cmd.Parameters.AddWithValue("@Password", u.Password); // chưa hash
                cmd.Parameters.AddWithValue("@FullName", u.FullName);
                cmd.Parameters.AddWithValue("@Phone", u.PhoneNumber);
                cmd.Parameters.AddWithValue("@Role", u.Role);

                int result = cmd.ExecuteNonQuery();
                db.closeConnection();
                return result > 0;
            }
        }

        // ============================
        // CẬP NHẬT THÔNG TIN PROFILE
        // ============================
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
                cmd.Parameters.AddWithValue("@FullName", u.FullName);
                cmd.Parameters.AddWithValue("@Email", u.Email);
                cmd.Parameters.AddWithValue("@Phone", u.PhoneNumber);

                int result = cmd.ExecuteNonQuery();
                db.closeConnection();
                return result > 0;
            }
        }
    }
}
