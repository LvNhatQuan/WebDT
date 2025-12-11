using Microsoft.Data.SqlClient;
using WebDT.Database;
using WebDT.Models;

namespace WebDT.DAL
{
    public class OrderDAL
    {
        private readonly DbConnect db = new DbConnect();

        // ============================
        // 1) TẠO ĐƠN (trả về orderId)
        // ============================
        public int CreateOrder(int userId, int? couponId, decimal subTotal, decimal shippingFee, decimal discount, decimal grandTotal, string address)
        {
            db.openConnection();

            string sql = @"
                INSERT INTO orders (user_id, coupon_id, sub_total, shipping_fee, discount_amount, grand_total, shipping_address)
                OUTPUT INSERTED.id
                VALUES (@uid, @cid, @sub, @ship, @disc, @grand, @addr)";

            using var cmd = new SqlCommand(sql, db.getConnecttion());
            cmd.Parameters.AddWithValue("@uid", userId);
            cmd.Parameters.AddWithValue("@cid", (object)couponId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@sub", subTotal);
            cmd.Parameters.AddWithValue("@ship", shippingFee);
            cmd.Parameters.AddWithValue("@disc", discount);
            cmd.Parameters.AddWithValue("@grand", grandTotal);
            cmd.Parameters.AddWithValue("@addr", address);

            int orderId = (int)cmd.ExecuteScalar();
            db.closeConnection();

            return orderId;
        }

        // ============================
        // 2) THÊM SẢN PHẨM VÀO order_items
        // ============================
        public void AddOrderItem(int orderId, int productId, int qty, decimal price)
        {
            db.openConnection();

            string sql = @"
                INSERT INTO order_items(order_id, product_id, quantity, price, total_price)
                VALUES(@oid, @pid, @qty, @price, @total)";

            using var cmd = new SqlCommand(sql, db.getConnecttion());
            cmd.Parameters.AddWithValue("@oid", orderId);
            cmd.Parameters.AddWithValue("@pid", productId);
            cmd.Parameters.AddWithValue("@qty", qty);
            cmd.Parameters.AddWithValue("@price", price);
            cmd.Parameters.AddWithValue("@total", qty * price);

            cmd.ExecuteNonQuery();
            db.closeConnection();
        }

        // ============================
        // 3) LẤY ĐƠN HÀNG THEO ID
        // ============================
        public OrderModel? GetOrderById(int orderId)
        {
            db.openConnection();
            OrderModel? order = null;

            using (var cmd = new SqlCommand("SELECT * FROM orders WHERE id=@id", db.getConnecttion()))
            {
                cmd.Parameters.AddWithValue("@id", orderId);

                using var r = cmd.ExecuteReader();
                if (r.Read())
                {
                    order = new OrderModel
                    {
                        Id = Convert.ToInt32(r["id"]),
                        UserId = r["user_id"] != DBNull.Value ? Convert.ToInt32(r["user_id"]) : 0,
                        CouponId = r["coupon_id"] != DBNull.Value ? Convert.ToInt32(r["coupon_id"]) : 0,
                        SubTotal = Convert.ToDecimal(r["sub_total"]),
                        ShippingFee = Convert.ToDecimal(r["shipping_fee"]),
                        Discount = Convert.ToDecimal(r["discount_amount"]),
                        GrandTotal = Convert.ToDecimal(r["grand_total"]),
                        ShippingAddress = r["shipping_address"].ToString(),
                        OrderDate = Convert.ToDateTime(r["order_date"])
                    };
                }
            }

            db.closeConnection();
            return order;
        }

        // ============================
        // 4) LẤY SẢN PHẨM TRONG ĐƠN
        // ============================
        public List<OrderItemModel> GetItems(int orderId)
        {
            List<OrderItemModel> items = new();
            db.openConnection();

            string sql = @"
                SELECT oi.*, p.name, p.image_url
                FROM order_items oi
                JOIN products p ON p.id = oi.product_id
                WHERE oi.order_id = @oid";

            using var cmd = new SqlCommand(sql, db.getConnecttion());
            cmd.Parameters.AddWithValue("@oid", orderId);

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                items.Add(new OrderItemModel
                {
                    ProductId = Convert.ToInt32(r["product_id"]),
                    Name = r["name"].ToString(),
                    Img = r["image_url"].ToString(),
                    Quantity = Convert.ToInt32(r["quantity"]),
                    Price = Convert.ToDecimal(r["price"]),
                    Total = Convert.ToDecimal(r["total_price"])
                });
            }

            db.closeConnection();
            return items;
        }
    }
}
