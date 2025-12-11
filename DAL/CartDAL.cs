using Microsoft.Data.SqlClient;
using WebDT.Database;
using WebDT.Models;

namespace WebDT.DAL
{
    public class CartDAL
    {
        private readonly DbConnect db = new DbConnect();

        // ============================
        // MAP CART ITEM
        // ============================
        private CartItem MapItem(SqlDataReader r)
        {
            return new CartItem
            {
                IdProduct = Convert.ToInt32(r["productId"]),
                Name = r["name"].ToString(),
                Img = r["image_url"].ToString(),
                Price = Convert.ToInt32(r["price"]),
                Quantity = Convert.ToInt32(r["quantity"]),
            };
        }

        // ============================
        // 1) LẤY GIỎ HÀNG THEO USER
        // ============================
        public List<CartItem> GetCart(int userId)
        {
            List<CartItem> list = new();
            db.openConnection();

            string sql = @"
                SELECT 
                    c.productId,
                    c.quantity,
                    p.name,
                    p.price,
                    p.image_url
                FROM cart c
                JOIN products p ON p.id = c.productId
                WHERE c.customerId = @uid";

            using var cmd = new SqlCommand(sql, db.getConnecttion());
            cmd.Parameters.AddWithValue("@uid", userId);

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(MapItem(r));
            }

            db.closeConnection();
            return list;
        }

        // ============================
        // 2) THÊM HOẶC CỘNG DỒN VÀO GIỎ
        // ============================
        public void AddToCart(int userId, int productId, int qty)
        {
            db.openConnection();

            string sql = @"
                IF EXISTS (SELECT 1 FROM cart WHERE customerId=@uid AND productId=@pid)
                    UPDATE cart SET quantity = quantity + @qty
                    WHERE customerId=@uid AND productId=@pid
                ELSE
                    INSERT INTO cart (customerId, productId, quantity, createAt)
                    VALUES (@uid, @pid, @qty, GETDATE())";

            using var cmd = new SqlCommand(sql, db.getConnecttion());
            cmd.Parameters.AddWithValue("@uid", userId);
            cmd.Parameters.AddWithValue("@pid", productId);
            cmd.Parameters.AddWithValue("@qty", qty);

            cmd.ExecuteNonQuery();
            db.closeConnection();
        }

        // ============================
        // 3) CẬP NHẬT SỐ LƯỢNG
        // ============================
        public void UpdateQuantity(int userId, int productId, int qty)
        {
            db.openConnection();

            string sql = @"
                UPDATE cart 
                SET quantity=@qty
                WHERE customerId=@uid AND productId=@pid";

            using var cmd = new SqlCommand(sql, db.getConnecttion());
            cmd.Parameters.AddWithValue("@uid", userId);
            cmd.Parameters.AddWithValue("@pid", productId);
            cmd.Parameters.AddWithValue("@qty", qty);

            cmd.ExecuteNonQuery();
            db.closeConnection();
        }

        // ============================
        // 4) XOÁ MỘT SẢN PHẨM KHỎI GIỎ
        // ============================
        public void DeleteItem(int userId, int productId)
        {
            db.openConnection();

            using var cmd = new SqlCommand(
                "DELETE FROM cart WHERE customerId=@uid AND productId=@pid",
                db.getConnecttion());

            cmd.Parameters.AddWithValue("@uid", userId);
            cmd.Parameters.AddWithValue("@pid", productId);

            cmd.ExecuteNonQuery();
            db.closeConnection();
        }

        // ============================
        // 5) XOÁ TOÀN BỘ GIỎ (CHECKOUT XONG)
        // ============================
        public void ClearCart(int userId)
        {
            db.openConnection();

            using var cmd = new SqlCommand(
                "DELETE FROM cart WHERE customerId=@uid",
                db.getConnecttion());

            cmd.Parameters.AddWithValue("@uid", userId);
            cmd.ExecuteNonQuery();

            db.closeConnection();
        }
    }
}
