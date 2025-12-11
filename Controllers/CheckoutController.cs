using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebD_T.DAL;
using WebDT.Database;

namespace WebD_T.Controllers
{
    public class CheckoutController : Controller
    {
        DbConnect connect = new DbConnect();
        private readonly UserDAL userDal = new UserDAL();

        public IActionResult Index()
        {
            int userId = GetUserId();
            ViewBag.Addresses = GetAddresses(userId);
            ViewBag.CartItems = GetCartItems(userId);

            return View();
        }

        [HttpPost]
        public IActionResult PlaceOrder(string shipping_address, int? coupon_id)
        {
            int userId = GetUserId();
            var items = GetCartItems(userId);

            decimal subTotal = items.Sum(x => x.Price * x.Quantity);
            decimal discount = 0;
            decimal shipFee = 15000;

            if (coupon_id.HasValue)
                discount = ApplyCoupon(coupon_id.Value, subTotal);

            decimal grand = subTotal + shipFee - discount;

            connect.openConnection();

            // INSERT ORDER
            int orderId;

            using (var cmd = new SqlCommand(@"
            INSERT INTO orders (user_id, coupon_id, sub_total, shipping_fee, discount_amount, grand_total, shipping_address)
            OUTPUT INSERTED.id
            VALUES(@uid, @cid, @sub, @ship, @disc, @grand, @addr)",
                connect.getConnecttion()))
            {
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@cid", (object)coupon_id ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@sub", subTotal);
                cmd.Parameters.AddWithValue("@ship", shipFee);
                cmd.Parameters.AddWithValue("@disc", discount);
                cmd.Parameters.AddWithValue("@grand", grand);
                cmd.Parameters.AddWithValue("@addr", shipping_address);

                orderId = (int)cmd.ExecuteScalar();
            }

            // INSERT order_items
            foreach (var i in items)
            {
                using var cmd = new SqlCommand(@"
                INSERT INTO order_items(order_id, product_id, quantity, price, total_price)
                VALUES(@oid, @pid, @qty, @price, @total)",
                    connect.getConnecttion());

                cmd.Parameters.AddWithValue("@oid", orderId);
                cmd.Parameters.AddWithValue("@pid", i.IdProduct);
                cmd.Parameters.AddWithValue("@qty", i.Quantity);
                cmd.Parameters.AddWithValue("@price", i.Price);
                cmd.Parameters.AddWithValue("@total", i.Total);
                cmd.ExecuteNonQuery();
            }

            // CLEAR CART
            using var clear = new SqlCommand("DELETE FROM cart WHERE customerId=@uid", connect.getConnecttion());
            clear.Parameters.AddWithValue("@uid", userId);
            clear.ExecuteNonQuery();

            connect.closeConnection();

            return RedirectToAction("Success", new { id = orderId });
        }

        public IActionResult Success(int id)
        {
            ViewBag.OrderId = id;
            return View();
        }

        // Helpers...
        private decimal ApplyCoupon(int cid, decimal subtotal)
        {
            connect.openConnection();

            using var cmd = new SqlCommand("SELECT discount_value FROM coupons WHERE id=@id AND is_active=1", connect.getConnecttion());
            cmd.Parameters.AddWithValue("@id", cid);

            var result = cmd.ExecuteScalar();
            connect.closeConnection();

            if (result == null) return 0;

            int percent = Convert.ToInt32(result);
            return subtotal * percent / 100;
        }

        private int GetUserId()
        {
            var email = User.Identity.Name;
            return userDal.GetUserByEmail(email).Id;
        }
    }

}
