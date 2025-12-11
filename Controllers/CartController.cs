using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebDT.DAL;
using WebDT.Database;
using WebDT.Models;

namespace WebDT.Controllers
{
    public class CartController : Controller
    {
        private readonly ProductDAL _productDal = new ProductDAL();
        private readonly UserDAL _userDal = new UserDAL();
        private readonly DbConnect _db = new DbConnect();

        // =========================
        // 1. XEM GIỎ HÀNG
        // =========================
        public IActionResult Index()
        {
            int userId = GetUserId();
            List<CartItem> items = GetCartItems(userId);
            return View(items);
        }

        // =========================
        // 2. THÊM SẢN PHẨM VÀO GIỎ
        // =========================
        public IActionResult Add(int productId, int qty = 1)
        {
            int userId = GetUserId();

            _db.openConnection();

            using (var cmd = new SqlCommand(@"
                IF EXISTS(SELECT 1 FROM cart WHERE customerId=@uid AND productId=@pid)
                    UPDATE cart SET quantity = quantity + @qty
                    WHERE customerId=@uid AND productId=@pid
                ELSE
                    INSERT INTO cart(customerId, productId, quantity, createAt)
                    VALUES(@uid, @pid, @qty, GETDATE())",
                _db.getConnecttion()))
            {
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@pid", productId);
                cmd.Parameters.AddWithValue("@qty", qty);

                cmd.ExecuteNonQuery();
            }

            _db.closeConnection();

            return RedirectToAction("Index");
        }

        // =========================
        // 3. UPDATE SỐ LƯỢNG TRONG GIỎ
        // =========================
        public IActionResult Update(int id, int qty)
        {
            _db.openConnection();

            using (var cmd = new SqlCommand(
                "UPDATE cart SET quantity=@qty WHERE id=@id",
                _db.getConnecttion()))
            {
                cmd.Parameters.AddWithValue("@qty", qty);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }

            _db.closeConnection();

            return RedirectToAction("Index");
        }

        // =========================
        // 4. XÓA SẢN PHẨM KHỎI GIỎ
        // =========================
        public IActionResult Delete(int id)
        {
            _db.openConnection();

            using (var cmd = new SqlCommand(
                "DELETE FROM cart WHERE id=@id",
                _db.getConnecttion()))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }

            _db.closeConnection();

            return RedirectToAction("Index");
        }

        // =========================
        // HÀM PHỤ — LẤY USER ID
        // =========================
        private int GetUserId()
        {
            string email = User.Identity?.Name;
            var user = _userDal.GetUserByEmail(email);
            return user.Id;
        }

        // =========================
        // HÀM PHỤ — LẤY GIỎ HÀNG
        // =========================
        private List<CartItem> GetCartItems(int userId)
        {
            var list = new List<CartItem>();

            _db.openConnection();

            using (var cmd = new SqlCommand(@"
                SELECT 
                    c.id AS CartId,
                    p.id AS ProductId,
                    p.name,
                    p.price,
                    p.image_url,
                    c.quantity
                FROM cart c
                JOIN products p ON p.id = c.productId
                WHERE c.customerId = @uid",
                _db.getConnecttion()))
            {
                cmd.Parameters.AddWithValue("@uid", userId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new CartItem
                        {
                            IdProduct = (int)reader["ProductId"],
                            Name = reader["name"].ToString(),
                            Img = reader["image_url"].ToString(),
                            Price = Convert.ToInt32(reader["price"]),
                            Quantity = Convert.ToInt32(reader["quantity"])
                        });
                    }
                }
            }

            _db.closeConnection();

            return list;
        }
    }
}
