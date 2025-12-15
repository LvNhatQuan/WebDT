using Microsoft.Data.SqlClient;
using WebDT.Database;
using WebDT.Models;

namespace WebDT.DAL
{
    public class OrderDAL
    {
        private readonly DbConnect db = new DbConnect();

        // =====================================================
        // CREATE ORDER + ORDER ITEMS (TRANSACTION)
        // =====================================================
        public int CreateOrder(
            int userId,
            int? couponId,
            List<CartItem> cart,
            decimal shippingFee,
            decimal discount,
            string address
        )
        {
            if (cart == null || cart.Count == 0)
                return -1;

            db.openConnection();
            var tran = db.getConnecttion().BeginTransaction();

            try
            {
                decimal subTotal = cart.Sum(x => x.Total);
                decimal grandTotal = subTotal + shippingFee - discount;

                // 1️⃣ INSERT ORDER
                string sqlOrder = @"
                    INSERT INTO orders
                    (user_id, coupon_id, sub_total, shipping_fee, discount_amount, grand_total, shipping_address, order_date)
                    OUTPUT INSERTED.id
                    VALUES
                    (@uid, @cid, @sub, @ship, @disc, @grand, @addr, GETDATE())";

                int orderId;
                using (var cmd = new SqlCommand(sqlOrder, db.getConnecttion(), tran))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@cid", (object)couponId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@sub", subTotal);
                    cmd.Parameters.AddWithValue("@ship", shippingFee);
                    cmd.Parameters.AddWithValue("@disc", discount);
                    cmd.Parameters.AddWithValue("@grand", grandTotal);
                    cmd.Parameters.AddWithValue("@addr", address ?? "");

                    orderId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // 2️⃣ INSERT ORDER ITEMS
                string sqlItem = @"
                    INSERT INTO order_items
                    (order_id, product_id, quantity, price, total_price)
                    VALUES
                    (@oid, @pid, @qty, @price, @total)";

                foreach (var item in cart)
                {
                    using var cmd = new SqlCommand(sqlItem, db.getConnecttion(), tran);
                    cmd.Parameters.AddWithValue("@oid", orderId);
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

        // =====================================================
        // GET ORDER BY ID
        // =====================================================
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
                        UserId = Convert.ToInt32(r["user_id"]),
                        CouponId = r["coupon_id"] == DBNull.Value
                            ? null
                            : Convert.ToInt32(r["coupon_id"]),
                        SubTotal = Convert.ToDecimal(r["sub_total"]),
                        ShippingFee = Convert.ToDecimal(r["shipping_fee"]),
                        Discount = Convert.ToDecimal(r["discount_amount"]),
                        GrandTotal = Convert.ToDecimal(r["grand_total"]),
                        ShippingAddress = r["shipping_address"]?.ToString(),
                        OrderDate = Convert.ToDateTime(r["order_date"])
                    };
                }
            }

            db.closeConnection();
            return order;
        }

        // =====================================================
        // GET ORDER ITEMS
        // =====================================================
        public List<OrderItemModel> GetItems(int orderId)
        {
            List<OrderItemModel> items = new();
            db.openConnection();

            string sql = @"
                SELECT 
                    oi.product_id,
                    oi.quantity,
                    oi.price,
                    oi.total_price,
                    p.name,
                    p.image_url
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
