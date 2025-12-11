using Microsoft.Data.SqlClient;
using WebDT.Database;   // DbConnect
using WebDT.Areas.Admin.Models;

namespace WebDT.Areas.Admin.DAL
{
    public class CustomerAdminDAL
    {
        private readonly DbConnect _db;

        public CustomerAdminDAL()
        {
            _db = new DbConnect();
        }

        // Lấy danh sách khách hàng
        public List<CustomerAdmin> GetAllCustomers()
        {
            List<CustomerAdmin> list = new List<CustomerAdmin>();

            string sql = @"SELECT id, username, email, full_name, phone_number, created_at
                           FROM users
                           WHERE role = 'customer'
                           ORDER BY created_at DESC";

            using (SqlConnection con = _db.getConnecttion())
            {
                _db.openConnection();

                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader rd = cmd.ExecuteReader();

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

                _db.closeConnection();
            }

            return list;
        }

        // Lấy khách hàng theo ID
        public CustomerAdmin? GetCustomerById(int id)
        {
            CustomerAdmin? c = null;

            string sql = @"SELECT id, username, email, full_name, phone_number, created_at
                           FROM users
                           WHERE id = @id";

            using (SqlConnection con = _db.getConnecttion())
            {
                _db.openConnection();

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", id);

                SqlDataReader rd = cmd.ExecuteReader();

                if (rd.Read())
                {
                    c = new CustomerAdmin
                    {
                        Id = (int)rd["id"],
                        Username = rd["username"].ToString(),
                        Email = rd["email"].ToString(),
                        FullName = rd["full_name"].ToString(),
                        PhoneNumber = rd["phone_number"].ToString(),
                        CreatedAt = Convert.ToDateTime(rd["created_at"])
                    };
                }

                _db.closeConnection();
            }

            return c;
        }

        // Lịch sử đơn hàng của khách
        public List<dynamic> GetCustomerOrders(int userId)
        {
            var list = new List<dynamic>();

            string sql = @"SELECT id, order_date, grand_total, shipping_address
                           FROM orders
                           WHERE user_id = @uid
                           ORDER BY order_date DESC";

            using (SqlConnection con = _db.getConnecttion())
            {
                _db.openConnection();

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@uid", userId);

                SqlDataReader rd = cmd.ExecuteReader();

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

                _db.closeConnection();
            }

            return list;
        }
    }
}
