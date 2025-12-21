using Microsoft.Data.SqlClient;
using WebDT.Database;
using WebDT.Areas.Admin.Models;

namespace WebDT.Areas.Admin.DAL
{
    public class ShippingAdminDAL
    {
        DbConnect db = new DbConnect();

        // ===============================
        // DANH SÁCH ĐƠN CẦN GIAO
        // ===============================
        public List<ShippingOrderAdmin> GetShippingOrders()
        {
            var list = new List<ShippingOrderAdmin>();

            string sql = @"
SELECT id, receiver_name, receiver_phone, shipping_address,
       grand_total, order_date, status
FROM orders
WHERE status IN ('pending','processing','shipped')
ORDER BY order_date DESC";

            using SqlConnection con =
                new SqlConnection(db.getConnecttion().ConnectionString);
            using SqlCommand cmd = new SqlCommand(sql, con);

            con.Open();
            using SqlDataReader rd = cmd.ExecuteReader();

            while (rd.Read())
            {
                list.Add(new ShippingOrderAdmin
                {
                    Id = (int)rd["id"],
                    ReceiverName = rd["receiver_name"].ToString()!,
                    ReceiverPhone = rd["receiver_phone"].ToString()!,
                    ShippingAddress = rd["shipping_address"].ToString()!,
                    GrandTotal = Convert.ToDecimal(rd["grand_total"]),
                    OrderDate = Convert.ToDateTime(rd["order_date"]),
                    Status = rd["status"].ToString()!
                });
            }

            return list;
        }

        // ===============================
        // CẬP NHẬT TRẠNG THÁI GIAO HÀNG
        // ===============================
        public bool UpdateStatus(int orderId, string status)
        {
            string sql = @"UPDATE orders SET status = @s WHERE id = @id";

            using SqlConnection con =
                new SqlConnection(db.getConnecttion().ConnectionString);
            using SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@id", orderId);
            cmd.Parameters.AddWithValue("@s", status);

            con.Open();
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
