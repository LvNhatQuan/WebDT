using Microsoft.AspNetCore.Mvc;
using WebDT.DAL;
using WebDT.Models;

namespace WebDT.Controllers
{
    public class OrderController : Controller
    {
        private readonly UserDAL userDal = new UserDAL();
        private readonly CartDAL cartDal = new CartDAL();
        private readonly OrderDAL orderDal = new OrderDAL();
        private readonly CouponDAL couponDal = new CouponDAL(); // nếu có

        // ============================
        // 1) TRANG CHECKOUT
        // ============================
        public IActionResult Checkout()
        {
            int userId = GetUserId();
            var cart = cartDal.GetCart(userId);

            if (!cart.Any())
                return RedirectToAction("Index", "Cart");

            ViewBag.Cart = cart;
            ViewBag.SubTotal = cart.Sum(x => x.Total);
            ViewBag.ShippingFee = 15000;
            ViewBag.Discount = 0;

            return View();
        }

        // ============================
        // 2) ĐẶT HÀNG
        // ============================
        [HttpPost]
        public IActionResult PlaceOrder(string shippingAddress, int? couponId)
        {
            int userId = GetUserId();
            var cart = cartDal.GetCart(userId);

            if (!cart.Any())
                return RedirectToAction("Index", "Cart");

            decimal subTotal = cart.Sum(x => x.Total);
            decimal shippingFee = 15000;
            decimal discount = 0;

            if (couponId != null)
                discount = couponDal.Apply(couponId.Value, subTotal);

            decimal grandTotal = subTotal + shippingFee - discount;

            // Tạo Order
            int orderId = orderDal.CreateOrder(userId, couponId, subTotal, shippingFee, discount, grandTotal, shippingAddress);

            // Thêm từng sản phẩm vào order_items
            foreach (var item in cart)
            {
                orderDal.AddOrderItem(orderId, item.IdProduct, item.Quantity, item.Price);
            }

            // Clear Cart
            cartDal.ClearCart(userId);

            return RedirectToAction("Success", new { id = orderId });
        }

        // ============================
        // 3) TRANG SUCCESS
        // ============================
        public IActionResult Success(int id)
        {
            var order = orderDal.GetOrderById(id);
            order.Items = orderDal.GetItems(id);

            return View(order);
        }

        // ============================
        // HELPER — GET USER ID
        // ============================
        private int GetUserId()
        {
            string email = User.Identity.Name;
            var u = userDal.GetUserByEmail(email);
            return u.Id;
        }
    }
}
