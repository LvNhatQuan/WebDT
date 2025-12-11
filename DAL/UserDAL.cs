using Microsoft.Data.SqlClient;
using WebD_T.Models;
using WebDT.Database;

namespace WebD_T.DAL
{
    public class UserDAL
    {
        private readonly DbConnect connect = new DbConnect();

        public User? GetUserByEmail(string email)
        {
            connect.openConnection();
            User? user = null;

            using var cmd = new SqlCommand("SELECT * FROM users WHERE email = @Email", connect.getConnecttion());
            cmd.Parameters.AddWithValue("@Email", email);

            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                user = new User
                {
                    Id = (int)reader["id"],
                    Username = reader["username"].ToString()!,
                    Email = reader["email"].ToString()!,
                    Password = reader["password"].ToString()!,
                    FullName = reader["full_name"].ToString()!,
                    PhoneNumber = reader["phone_number"].ToString()!,
                    Role = reader["role"].ToString()!
                };
            }

            connect.closeConnection();
            return user;
        }

        public bool UpdateProfile(User u)
        {
            connect.openConnection();

            using var cmd = new SqlCommand(@"
                UPDATE users SET
                    full_name = @FullName,
                    email = @Email,
                    phone_number = @PhoneNumber
                WHERE id = @Id
            ", connect.getConnecttion());

            cmd.Parameters.AddWithValue("@Id", u.Id);
            cmd.Parameters.AddWithValue("@FullName", u.FullName);
            cmd.Parameters.AddWithValue("@Email", u.Email);
            cmd.Parameters.AddWithValue("@PhoneNumber", u.PhoneNumber);

            int result = cmd.ExecuteNonQuery();

            connect.closeConnection();
            return result > 0;
        }

        public bool CreateUser(User u)
        {
            connect.openConnection();

            using var cmd = new SqlCommand(@"
                INSERT INTO users (username, email, password, full_name, phone_number, role)
                VALUES (@Username, @Email, @Password, @FullName, @PhoneNumber, @Role)",
                connect.getConnecttion());

            cmd.Parameters.AddWithValue("@Username", u.Username);
            cmd.Parameters.AddWithValue("@Email", u.Email);
            cmd.Parameters.AddWithValue("@Password", u.Password); // no hash
            cmd.Parameters.AddWithValue("@FullName", u.FullName);
            cmd.Parameters.AddWithValue("@PhoneNumber", u.PhoneNumber);
            cmd.Parameters.AddWithValue("@Role", u.Role);

            int result = cmd.ExecuteNonQuery();
            connect.closeConnection();

            return result > 0;
        }
        public User? GetUserByUsername(string username)
        {
            connect.openConnection();
            User? user = null;

            using var cmd = new SqlCommand("SELECT * FROM users WHERE username = @Username", connect.getConnecttion());
            cmd.Parameters.AddWithValue("@Username", username);

            var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                user = new User
                {
                    Id = (int)reader["id"],
                    Username = reader["username"].ToString()!,
                    Email = reader["email"].ToString()!,
                    Password = reader["password"].ToString()!,
                    FullName = reader["full_name"].ToString()!,
                    PhoneNumber = reader["phone_number"].ToString()!,
                    Role = reader["role"].ToString()!
                };
            }

            connect.closeConnection();
            return user;
        }
        public User? GetUserById(int id)
        {
            connect.openConnection();
            User? user = null;

            using var cmd = new SqlCommand("SELECT * FROM users WHERE id = @Id", connect.getConnecttion());
            cmd.Parameters.AddWithValue("@Id", id);

            var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                user = new User
                {
                    Id = (int)reader["id"],
                    Username = reader["username"].ToString()!,
                    Email = reader["email"].ToString()!,
                    Password = reader["password"].ToString()!,
                    FullName = reader["full_name"].ToString()!,
                    PhoneNumber = reader["phone_number"].ToString()!,
                    Role = reader["role"].ToString()!
                };
            }

            connect.closeConnection();
            return user;
        }

    }
}