using Microsoft.Data.SqlClient;
using WebDT.Models;
using WebDT.Database;

namespace WebDT.Areas.Admin.DAL
{
    public class UserAdminDAL
    {
        DbConnect connect = new DbConnect();

        // GET ALL USERS
        public List<User> GetAll()
        {
            connect.openConnection();
            List<User> list = new();

            using var cmd = new SqlCommand("SELECT * FROM users ORDER BY id DESC", connect.getConnecttion());
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new User
                {
                    Id = (int)reader["id"],
                    Username = reader["username"].ToString()!,
                    Email = reader["email"].ToString()!,
                    Password = reader["password"].ToString()!,
                    FullName = reader["full_name"].ToString()!,
                    PhoneNumber = reader["phone_number"].ToString()!,
                    Role = reader["role"].ToString()!,
                    IsLocked = reader["is_locked"] != DBNull.Value && (bool)reader["is_locked"]
                });
            }

            connect.closeConnection();
            return list;
        }

        // GET USER BY ID
        public User? GetById(int id)
        {
            connect.openConnection();
            User? u = null;

            using var cmd = new SqlCommand("SELECT * FROM users WHERE id = @id", connect.getConnecttion());
            cmd.Parameters.AddWithValue("@id", id);

            var r = cmd.ExecuteReader();
            if (r.Read())
            {
                u = new User
                {
                    Id = (int)r["id"],
                    Username = r["username"].ToString()!,
                    Email = r["email"].ToString()!,
                    Password = r["password"].ToString()!,
                    FullName = r["full_name"].ToString()!,
                    PhoneNumber = r["phone_number"].ToString()!,
                    Role = r["role"].ToString()!,
                    IsLocked = r["is_locked"] != DBNull.Value && (bool)r["is_locked"]
                };
            }

            connect.closeConnection();
            return u;
        }

        // CREATE USER
        public bool Create(User u)
        {
            connect.openConnection();

            using var cmd = new SqlCommand(@"
                INSERT INTO users (username, email, password, full_name, phone_number, role, is_locked)
                VALUES (@Username, @Email, @Password, @FullName, @PhoneNumber, @Role, @IsLocked)",
                connect.getConnecttion());

            cmd.Parameters.AddWithValue("@Username", u.Username);
            cmd.Parameters.AddWithValue("@Email", u.Email);
            cmd.Parameters.AddWithValue("@Password", u.Password);
            cmd.Parameters.AddWithValue("@FullName", u.FullName);
            cmd.Parameters.AddWithValue("@PhoneNumber", u.PhoneNumber);
            cmd.Parameters.AddWithValue("@Role", u.Role);
            cmd.Parameters.AddWithValue("@IsLocked", u.IsLocked);

            int result = cmd.ExecuteNonQuery();
            connect.closeConnection();

            return result > 0;
        }

        // UPDATE USER
        public bool Update(User u)
        {
            connect.openConnection();

            using var cmd = new SqlCommand(@"
                UPDATE users SET
                    username = @Username,
                    email = @Email,
                    full_name = @FullName,
                    phone_number = @PhoneNumber,
                    role = @Role,
                    password = @Password,
                    is_locked = @IsLocked
                WHERE id = @Id",
                connect.getConnecttion());

            cmd.Parameters.AddWithValue("@Id", u.Id);
            cmd.Parameters.AddWithValue("@Username", u.Username);
            cmd.Parameters.AddWithValue("@Email", u.Email);
            cmd.Parameters.AddWithValue("@FullName", u.FullName);
            cmd.Parameters.AddWithValue("@PhoneNumber", u.PhoneNumber);
            cmd.Parameters.AddWithValue("@Role", u.Role);
            cmd.Parameters.AddWithValue("@Password", u.Password);
            cmd.Parameters.AddWithValue("@IsLocked", u.IsLocked);

            int result = cmd.ExecuteNonQuery();
            connect.closeConnection();

            return result > 0;
        }

        // CHỈ UPDATE TRẠNG THÁI KHÓA/MỞ
        public bool UpdateLockStatus(int id, bool isLocked)
        {
            connect.openConnection();

            using var cmd = new SqlCommand(@"
                UPDATE users SET is_locked = @IsLocked
                WHERE id = @Id",
                connect.getConnecttion());

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@IsLocked", isLocked);

            int result = cmd.ExecuteNonQuery();
            connect.closeConnection();

            return result > 0;
        }

        // DELETE USER
        public bool Delete(int id)
        {
            connect.openConnection();

            using var cmd = new SqlCommand("DELETE FROM users WHERE id = @id", connect.getConnecttion());
            cmd.Parameters.AddWithValue("@id", id);

            int result = cmd.ExecuteNonQuery();
            connect.closeConnection();

            return result > 0;
        }
    }
}
