using Microsoft.Data.SqlClient;
using WebDT.Database;
using WebDT.Areas.Admin.Models;

namespace WebDT.Areas.Admin.DAL
{
    public class CustomerAdminDAL
    {
        private readonly DbConnect _db = new DbConnect();

        // ===============================
        // LẤY DANH SÁCH KHÁCH HÀNG
        // ===============================
        public List<CustomerAdmin> GetAllCustomers()
        {
            var list = new List<CustomerAdmin>();

            string sql = @"SELECT id, username, email, full_name, phone_number, created_at
                           FROM users
                           WHERE role = 'customer'
                           ORDER BY created_at DESC";

            using SqlConnection con =
                new SqlConnection(_db.getConnecttion().ConnectionString);
            using SqlCommand cmd = new SqlCommand(sql, con);

            con.Open();
            using SqlDataReader rd = cmd.ExecuteReader();

            while (rd.Read())
            {
                list.Add(new CustomerAdmin
                {
                    Id = (int)rd["id"],
                    Username = rd["username"].ToString(),
                    Email = rd["email"].ToString(),
                    FullName = rd["full_name"].ToString(),
                    PhoneNumber = rd["phone_number"].ToString(),
                    CreatedAt = Convert.ToDateTime(rd["created_at"])
                });
            }

            return list;
        }

        // ===============================
        // LẤY KHÁCH HÀNG THEO ID
        // ===============================
        public CustomerAdmin? GetCustomerById(int id)
        {
            string sql = @"SELECT id, username, email, full_name, phone_number, created_at
                           FROM users
                           WHERE id = @id";

            using SqlConnection con =
                new SqlConnection(_db.getConnecttion().ConnectionString);
            using SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", id);

            con.Open();
            using SqlDataReader rd = cmd.ExecuteReader();

            if (!rd.Read())
                return null;

            return new CustomerAdmin
            {
                Id = (int)rd["id"],
                Username = rd["username"].ToString(),
                Email = rd["email"].ToString(),
                FullName = rd["full_name"].ToString(),
                PhoneNumber = rd["phone_number"].ToString(),
                CreatedAt = Convert.ToDateTime(rd["created_at"])
            };
        }

        // ===============================
        // LỊCH SỬ ĐƠN HÀNG CỦA KHÁCH
        // ===============================
        public List<dynamic> GetCustomerOrders(int userId)
        {
            var list = new List<dynamic>();

            string sql = @"SELECT id, order_date, grand_total, shipping_address
                           FROM orders
                           WHERE user_id = @uid
                           ORDER BY order_date DESC";

            // ⭐ TẠO CONNECTION MỚI – FIX LỖI DETAILS
            using SqlConnection con =
                new SqlConnection(_db.getConnecttion().ConnectionString);
            using SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@uid", userId);

            con.Open();
            using SqlDataReader rd = cmd.ExecuteReader();

            while (rd.Read())
            {
                list.Add(new
                {
                    Id = (int)rd["id"],
                    OrderDate = Convert.ToDateTime(rd["order_date"]),
                    Total = Convert.ToDecimal(rd["grand_total"]),
                    Address = rd["shipping_address"].ToString()
                });
            }

            return list;
        }
    }
}
