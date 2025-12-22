using Microsoft.Data.SqlClient;
using WebDT.Database;
using WebDT.Models;

namespace WebDT.DAL
{
    public class OrderDAL
    {
        private readonly DbConnect db = new DbConnect();

        // ================= CREATE ORDER =================
        public int CreateOrder(
            int userId,
            List<CartItem> cart,
            decimal shippingFee,
            decimal discountAmount,
            string address,
            string receiverName,
            string receiverPhone
        )
        {
            if (cart == null || cart.Count == 0)
                return -1;

            db.openConnection();
            var tran = db.getConnecttion().BeginTransaction();

            try
            {
                decimal subTotal = cart.Sum(x => x.Total);
                decimal grandTotal = subTotal + shippingFee - discountAmount;

                // ✅ INSERT ORDERS – KHỚP 100% DB DT
                string sqlOrder = @"
INSERT INTO orders
(
    user_id,
    order_date,
    sub_total,
    shipping_fee,
    discount_amount,
    grand_total,
    shipping_address,
    receiver_name,
    receiver_phone,
    status
)
OUTPUT INSERTED.id
VALUES
(
    @uid,
    GETDATE(),
    @sub,
    @ship,
    @disc,
    @grand,
    @addr,
    @rname,
    @rphone,
    'pending'
)";

                int orderId;
                using (var cmd = new SqlCommand(sqlOrder, db.getConnecttion(), tran))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@sub", subTotal);
                    cmd.Parameters.AddWithValue("@ship", shippingFee);
                    cmd.Parameters.AddWithValue("@disc", discountAmount);
                    cmd.Parameters.AddWithValue("@grand", grandTotal);
                    cmd.Parameters.AddWithValue("@addr", address);
                    cmd.Parameters.AddWithValue("@rname", receiverName);
                    cmd.Parameters.AddWithValue("@rphone", receiverPhone);

                    orderId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // ================= INSERT ORDER ITEMS =================
                string sqlItem = @"
INSERT INTO order_items
(order_id, coupon_id, product_id, quantity, price, total_price)
VALUES
(@oid, @cid, @pid, @qty, @price, @total)";

                foreach (var item in cart)
                {
                    using var cmd = new SqlCommand(sqlItem, db.getConnecttion(), tran);
                    cmd.Parameters.AddWithValue("@oid", orderId);
                    cmd.Parameters.AddWithValue("@cid", (object?)item.CouponId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@pid", item.IdProduct);
                    cmd.Parameters.AddWithValue("@qty", item.Quantity);
                    cmd.Parameters.AddWithValue("@price", item.Price);
                    cmd.Parameters.AddWithValue("@total", item.Total);
                    cmd.ExecuteNonQuery();
                }

                tran.Commit();
                return orderId;
            }
            catch
            {
                tran.Rollback();
                return -1;
            }
            finally
            {
                db.closeConnection();
            }
        }

        // ================= GET ORDER =================
        public OrderModel? GetOrderById(int orderId)
        {
            db.openConnection();
            OrderModel? order = null;

            string sql = "SELECT * FROM orders WHERE id = @id";

            using (var cmd = new SqlCommand(sql, db.getConnecttion()))
            {
                cmd.Parameters.AddWithValue("@id", orderId);

                using var r = cmd.ExecuteReader();
                if (r.Read())
                {
                    order = new OrderModel
                    {
                        Id = Convert.ToInt32(r["id"]),
                        UserId = r["user_id"] == DBNull.Value ? null : Convert.ToInt32(r["user_id"]),
                        SubTotal = Convert.ToDecimal(r["sub_total"]),
                        ShippingFee = Convert.ToDecimal(r["shipping_fee"]),
                        Discount = Convert.ToDecimal(r["discount_amount"]),
                        GrandTotal = Convert.ToDecimal(r["grand_total"]),
                        ShippingAddress = r["shipping_address"]?.ToString() ?? "",
                        ReceiverName = r["receiver_name"]?.ToString() ?? "",
                        ReceiverPhone = r["receiver_phone"]?.ToString() ?? "",
                        OrderDate = Convert.ToDateTime(r["order_date"]),
                        Status = r["status"]?.ToString() ?? "pending"
                    };
                }
            }

            db.closeConnection();
            return order;
        }

        // ================= GET ITEMS =================
        public List<OrderItemModel> GetItems(int orderId)
        {
            List<OrderItemModel> items = new();
            db.openConnection();

            string sql = @"
SELECT 
    oi.id,
    oi.order_id,
    oi.coupon_id,
    oi.product_id,
    oi.quantity,
    oi.price,
    oi.total_price,
    p.name,
    p.image_url
FROM order_items oi
JOIN products p ON p.id = oi.product_id
WHERE oi.order_id = @oid
ORDER BY oi.id ASC";

            using var cmd = new SqlCommand(sql, db.getConnecttion());
            cmd.Parameters.AddWithValue("@oid", orderId);

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                items.Add(new OrderItemModel
                {
                    Id = Convert.ToInt32(r["id"]),
                    OrderId = Convert.ToInt32(r["order_id"]),
                    CouponId = r["coupon_id"] == DBNull.Value ? null : Convert.ToInt32(r["coupon_id"]),
                    ProductId = r["product_id"] == DBNull.Value ? null : Convert.ToInt32(r["product_id"]),
                    Name = r["name"]?.ToString() ?? "",
                    Img = r["image_url"]?.ToString() ?? "",
                    Quantity = Convert.ToInt32(r["quantity"]),
                    Price = Convert.ToDecimal(r["price"]),
                    Total = Convert.ToDecimal(r["total_price"])
                });
            }

            db.closeConnection();
            return items;
        }
        // =========================
        // STAFF: LẤY DANH SÁCH ĐƠN
        // =========================
        public List<OrderModel> GetAllForStaff()
        {
            List<OrderModel> list = new();
            db.openConnection();

            string sql = @"
SELECT *
FROM orders
ORDER BY order_date DESC";

            using var cmd = new SqlCommand(sql, db.getConnecttion());
            using var r = cmd.ExecuteReader();

            while (r.Read())
            {
                list.Add(new OrderModel
                {
                    Id = Convert.ToInt32(r["id"]),
                    UserId = r["user_id"] == DBNull.Value ? null : Convert.ToInt32(r["user_id"]),
                    OrderDate = Convert.ToDateTime(r["order_date"]),
                    GrandTotal = Convert.ToDecimal(r["grand_total"]),
                    ShippingAddress = r["shipping_address"]?.ToString() ?? "",
                    ReceiverName = r["receiver_name"]?.ToString() ?? "",
                    ReceiverPhone = r["receiver_phone"]?.ToString() ?? "",
                    Status = r["status"]?.ToString() ?? "pending"
                });
            }

            db.closeConnection();
            return list;
        }

        // =========================
        // STAFF: CẬP NHẬT TRẠNG THÁI
        // =========================
        public bool UpdateStatus(int orderId, string status)
        {
            db.openConnection();

            string sql = "UPDATE orders SET status = @st WHERE id = @id";

            using var cmd = new SqlCommand(sql, db.getConnecttion());
            cmd.Parameters.AddWithValue("@st", status);
            cmd.Parameters.AddWithValue("@id", orderId);

            int rows = cmd.ExecuteNonQuery();
            db.closeConnection();

            return rows > 0;
        }

    }
}
