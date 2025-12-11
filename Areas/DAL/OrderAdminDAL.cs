using Microsoft.Data.SqlClient;
using WebDT.Database;
using WebD_T.Areas.Admin.Models;

namespace WebD_T.Areas.Admin.DAL
{
    public class OrderAdminDAL
    {
        private readonly DbConnect connect = new DbConnect();

        // ============================================
        // LẤY TẤT CẢ ĐƠN HÀNG
        // ============================================
        public List<OrderAdmin> GetAll()
        {
            connect.openConnection();
            List<OrderAdmin> list = new();

            string sql = @"
                SELECT 
                    o.id,
                    o.user_id,
                    u.username,
                    o.order_date,
                    o.sub_total,
                    o.shipping_fee,
                    o.discount_amount,
                    o.grand_total,
                    o.shipping_address
                FROM orders o
                LEFT JOIN users u ON o.user_id = u.id
                ORDER BY o.order_date DESC";

            using var cmd = new SqlCommand(sql, connect.getConnecttion());
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new OrderAdmin
                {
                    Id = (int)reader["id"],
                    UserId = reader["user_id"] != DBNull.Value ? Convert.ToInt32(reader["user_id"]) : null,
                    UserName = reader["username"]?.ToString(),
                    OrderDate = Convert.ToDateTime(reader["order_date"]),
                    SubTotal = Convert.ToDecimal(reader["sub_total"]),
                    ShippingFee = Convert.ToDecimal(reader["shipping_fee"]),
                    DiscountAmount = Convert.ToDecimal(reader["discount_amount"]),
                    GrandTotal = Convert.ToDecimal(reader["grand_total"]),
                    ShippingAddress = reader["shipping_address"].ToString()!
                });
            }

            connect.closeConnection();
            return list;
        }

        // ============================================
        // LẤY 1 ĐƠN HÀNG THEO ID
        // ============================================
        public OrderAdmin? GetById(int id)
        {
            connect.openConnection();
            OrderAdmin? order = null;

            string sqlOrder = @"
                SELECT 
                    o.id,
                    o.user_id,
                    u.username,
                    o.order_date,
                    o.sub_total,
                    o.shipping_fee,
                    o.discount_amount,
                    o.grand_total,
                    o.shipping_address
                FROM orders o
                LEFT JOIN users u ON o.user_id = u.id
                WHERE o.id = @Id";

            using var cmd = new SqlCommand(sqlOrder, connect.getConnecttion());
            cmd.Parameters.AddWithValue("@Id", id);
            var r = cmd.ExecuteReader();

            if (r.Read())
            {
                order = new OrderAdmin
                {
                    Id = (int)r["id"],
                    UserId = r["user_id"] != DBNull.Value ? Convert.ToInt32(r["user_id"]) : null,
                    UserName = r["username"]?.ToString(),
                    OrderDate = Convert.ToDateTime(r["order_date"]),
                    SubTotal = Convert.ToDecimal(r["sub_total"]),
                    ShippingFee = Convert.ToDecimal(r["shipping_fee"]),
                    DiscountAmount = Convert.ToDecimal(r["discount_amount"]),
                    GrandTotal = Convert.ToDecimal(r["grand_total"]),
                    ShippingAddress = r["shipping_address"].ToString()!,
                    Items = new List<OrderItemAdmin>()
                };
            }

            r.Close();

            if (order == null)
            {
                connect.closeConnection();
                return null;
            }

            // ============================================
            // LOAD DANH SÁCH SẢN PHẨM TRONG ĐƠN (order_items)
            // ============================================
            string sqlItems = @"
                SELECT 
                    oi.id,
                    oi.product_id,
                    oi.quantity,
                    oi.price,
                    oi.total_price,
                    p.name AS product_name
                FROM order_items oi
                LEFT JOIN products p ON oi.product_id = p.id
                WHERE oi.order_id = @OrderId";

            using var cmdItems = new SqlCommand(sqlItems, connect.getConnecttion());
            cmdItems.Parameters.AddWithValue("@OrderId", id);

            var ri = cmdItems.ExecuteReader();
            while (ri.Read())
            {
                order.Items.Add(new OrderItemAdmin
                {
                    Id = (int)ri["id"],
                    OrderId = id,
                    ProductId = ri["product_id"] != DBNull.Value ? Convert.ToInt32(ri["product_id"]) : null,
                    ProductName = ri["product_name"]?.ToString(),
                    Quantity = (int)ri["quantity"],
                    Price = Convert.ToDecimal(ri["price"]),
                    TotalPrice = Convert.ToDecimal(ri["total_price"])
                });
            }

            connect.closeConnection();
            return order;
        }

        // ============================================
        // CẬP NHẬT TRẠNG THÁI ĐƠN HÀNG (NẾU CÓ STATUS)
        // ============================================
        public bool UpdateStatus(int id, string status)
        {
            connect.openConnection();

            string sql = "UPDATE orders SET status = @Status WHERE id = @Id";

            using var cmd = new SqlCommand(sql, connect.getConnecttion());
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Status", status);

            int result = cmd.ExecuteNonQuery();
            connect.closeConnection();

            return result > 0;
        }

        // ============================================
        // XOÁ ĐƠN HÀNG
        // ============================================
        public bool Delete(int id)
        {
            connect.openConnection();

            using var cmd = new SqlCommand("DELETE FROM orders WHERE id = @Id", connect.getConnecttion());
            cmd.Parameters.AddWithValue("@Id", id);

            int result = cmd.ExecuteNonQuery();
            connect.closeConnection();

            return result > 0;
        }

        public void DeleteReviewsByOrderId(int orderId)
        {
            connect.openConnection();

            using var cmd = new SqlCommand(
                "DELETE FROM reviews WHERE order_id = @OrderId",
                connect.getConnecttion()
            );

            cmd.Parameters.AddWithValue("@OrderId", orderId);
            cmd.ExecuteNonQuery();

            connect.closeConnection();
        }
    }
}
