using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebD_T.DAL;
using WebDT.DAL;
using WebDT.Database;
using WebDT.Models;

namespace WebD_T.Controllers
{
    public class CartController : Controller
    {
        private readonly ProductDAL _productDAL = new ProductDAL();
        private readonly UserDAL _userDal = new UserDAL();
        private readonly DbConnect connect = new DbConnect();

        public IActionResult Index()
        {
            int userId = GetUserId();
            var items = GetCartItems(userId);
            return View(items);
        }

        public IActionResult Add(int productId, int qty = 1)
        {
            int userId = GetUserId();

            connect.openConnection();
            using var cmd = new SqlCommand(@"
                IF EXISTS(SELECT 1 FROM cart WHERE customerId=@uid AND productId=@pid)
                    UPDATE cart SET quantity = quantity + @qty
                    WHERE customerId=@uid AND productId=@pid
                ELSE
                    INSERT INTO cart(customerId, productId, quantity, createAt)
                    VALUES(@uid, @pid, @qty, GETDATE())",
                connect.getConnecttion());

            cmd.Parameters.AddWithValue("@uid", userId);
            cmd.Parameters.AddWithValue("@pid", productId);
            cmd.Parameters.AddWithValue("@qty", qty);

            cmd.ExecuteNonQuery();
            connect.closeConnection();

            return RedirectToAction("Index");
        }

        public IActionResult Update(int id, int qty)
        {
            connect.openConnection();
            using var cmd = new SqlCommand("UPDATE cart SET quantity=@qty WHERE id=@id", connect.getConnecttion());
            cmd.Parameters.AddWithValue("@qty", qty);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            connect.closeConnection();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            connect.openConnection();
            using var cmd = new SqlCommand("DELETE FROM cart WHERE id=@id", connect.getConnecttion());
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            connect.closeConnection();

            return RedirectToAction("Index");
        }

        private int GetUserId()
        {
            var email = User.Identity?.Name;
            var user = _userDal.GetUserByEmail(email);
            return user.Id;
        }

        private List<CartItem> GetCartItems(int userId)
        {
            var list = new List<CartItem>();

            connect.openConnection();
            using var cmd = new SqlCommand(@"
                SELECT c.id AS CartId, p.id AS ProductId, p.name, p.price, p.image_url, c.quantity
                FROM cart c 
                JOIN products p ON p.id = c.productId
                WHERE c.customerId = @uid", connect.getConnecttion());

            cmd.Parameters.AddWithValue("@uid", userId);

            using var reader = cmd.ExecuteReader();
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

            connect.closeConnection();
            return list;
        }
    }
}
