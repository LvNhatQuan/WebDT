using Microsoft.Data.SqlClient;
using WebDT.Database;
using WebDT.Models;

namespace WebDT.DAL
{
    public class CartDAL
    {
        private readonly DbConnect _db = new DbConnect();

        // Map CartItem
        private CartItem MapItem(SqlDataReader r)
        {
            return new CartItem
            {
                IdProduct = Convert.ToInt32(r["productId"]),
                Name = r["name"].ToString() ?? "",
                Img = r["image_url"].ToString(),
                Price = Convert.ToDecimal(r["price"]),
                Quantity = Convert.ToInt32(r["quantity"]),
                Discount = 0
            };
        }

        // Lấy giỏ hàng DB
        public List<CartItem> GetCart(int userId)
        {
            List<CartItem> list = new();
            _db.openConnection();

            string sql = @"
                SELECT c.productId, c.quantity, p.name, p.price, p.image_url
                FROM cart c
                JOIN products p ON p.id = c.productId
                WHERE c.customerId = @uid";

            using var cmd = new SqlCommand(sql, _db.getConnecttion());
            cmd.Parameters.AddWithValue("@uid", userId);

            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(MapItem(r));

            _db.closeConnection();
            return list;
        }

        // ===================
        // CHECKOUT
        // ===================
        public bool CheckOut(User user, List<CartItem> cart)
        {
            string connStr = _db.GetConnectionString();  // 🔥 sửa lỗi ConnectionString

            using var conn = new SqlConnection(connStr);
            conn.Open();

            using var tran = conn.BeginTransaction();

            try
            {
                decimal subtotal = cart.Sum(i => i.Total);

                // 1. INSERT ORDER
                string sqlOrder = @"
                    INSERT INTO orders (user_id, sub_total, shipping_fee, discount_amount,
                                        grand_total, shipping_address, order_date)
                    VALUES (@uid, @sub, 0, 0, @sub, @addr, GETDATE());
                    SELECT SCOPE_IDENTITY();";

                int orderId;
                using (var cmd = new SqlCommand(sqlOrder, conn, tran))
                {
                    cmd.Parameters.AddWithValue("@uid", user.Id);
                    cmd.Parameters.AddWithValue("@sub", subtotal);
                    

                    orderId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // 2. INSERT ORDER_ITEMS
                foreach (var item in cart)
                {
                    string sqlItem = @"
                        INSERT INTO order_items (order_id, product_id, quantity, price, total_price)
                        VALUES (@oid, @pid, @qty, @price, @total)";

                    using var cmd = new SqlCommand(sqlItem, conn, tran);
                    cmd.Parameters.AddWithValue("@oid", orderId);
                    cmd.Parameters.AddWithValue("@pid", item.IdProduct);
                    cmd.Parameters.AddWithValue("@qty", item.Quantity);
                    cmd.Parameters.AddWithValue("@price", item.Price);
                    cmd.Parameters.AddWithValue("@total", item.Total);

                    cmd.ExecuteNonQuery();
                }

                // 3. Xoá giỏ
                using (var cmd = new SqlCommand("DELETE FROM cart WHERE customerId=@uid", conn, tran))
                {
                    cmd.Parameters.AddWithValue("@uid", user.Id);
                    cmd.ExecuteNonQuery();
                }

                tran.Commit();
                return true;
            }
            catch
            {
                tran.Rollback();
                return false;
            }
        }
    }
}
