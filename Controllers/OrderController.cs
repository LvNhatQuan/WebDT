using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebDT.DAL;
using WebDT.Helper;
using WebDT.Models;

namespace WebDT.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly CartDAL cartDal = new CartDAL();
        private readonly OrderDAL orderDal = new OrderDAL();

        // ============================
        // 1) TRANG CHECKOUT
        // ============================
        [HttpGet]
        public IActionResult Checkout()
        {
            // LẤY USER ID TỪ CLAIM (KHÔNG DÙNG EMAIL NỮA)
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdClaim);

            var cart = cartDal.GetCart(userId);
            if (!cart.Any())
                return RedirectToAction("Index", "Cart");

            var vm = new CheckoutViewModel
            {
                CartItems = cart
            };

            return View(vm);
        }

        // ============================
        // 2) ĐẶT HÀNG
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PlaceOrder(string shippingAddress, int? couponId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdClaim);

            var cart = cartDal.GetCart(userId);
            if (!cart.Any())
                return RedirectToAction("Index", "Cart");

            // PHÍ & GIẢM GIÁ (mở rộng sau)
            decimal shippingFee = 15000;
            decimal discount = 0;

            // 🔥 GỌI DAL DUY NHẤT
            int orderId = orderDal.CreateOrder(
                userId,
                couponId,
                cart,
                shippingFee,
                discount,
                shippingAddress ?? "Chưa nhập địa chỉ"
            );

            if (orderId <= 0)
            {
                TempData["OrderError"] = "Đặt hàng thất bại!";
                return RedirectToAction("Checkout");
            }

            // TODO: Clear cart DB nếu bạn có table cart
            // cartDal.ClearCart(userId);

            return RedirectToAction("Success", new { id = orderId });
        }

        // ============================
        // 3) TRANG SUCCESS
        // ============================
        [HttpGet]
        public IActionResult Success(int id)
        {
            var order = orderDal.GetOrderById(id);
            if (order == null)
                return RedirectToAction("Index", "Home");

            order.Items = orderDal.GetItems(id);

            return View(order);
        }
    }
}
