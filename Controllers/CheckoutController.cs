using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebDT.DAL;
using WebDT.Helper;
using WebDT.Models;

namespace WebDT.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly OrderDAL _orderDal = new OrderDAL();

        // =========================
        // LẤY GIỎ HÀNG TỪ SESSION
        // =========================
        private List<CartItem> Cart =>
            HttpContext.Session.Get<List<CartItem>>(MyConst.CART_KEY)
            ?? new List<CartItem>();

        // =========================
        // GET: /Checkout
        // =========================
        [HttpGet]
        public IActionResult Index()
        {
            if (!Cart.Any())
            {
                TempData["CheckoutError"] = "Giỏ hàng đang trống!";
                return RedirectToAction("Index", "Cart");
            }

            var vm = new CheckoutViewModel
            {
                CartItems = Cart
            };

            return View(vm);
        }

        // =========================
        // POST: /Checkout/Confirm
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Confirm(string shippingAddress)
        {
            // Kiểm tra giỏ
            if (!Cart.Any())
            {
                TempData["CheckoutError"] = "Giỏ hàng trống!";
                return RedirectToAction("Index", "Cart");
            }

            // Lấy userId từ claim
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(userIdClaim);

            // Các tham số mở rộng
            int? couponId = null;      // chưa dùng coupon
            decimal shippingFee = 0;
            decimal discount = 0;

            // 🔥 GỌI DAL DUY NHẤT
            int orderId = _orderDal.CreateOrder(
                userId,
                couponId,
                Cart,
                shippingFee,
                discount,
                shippingAddress ?? "Chưa nhập địa chỉ"
            );

            if (orderId <= 0)
            {
                TempData["CheckoutError"] = "Thanh toán thất bại!";
                return RedirectToAction("Index");
            }

            // Xoá giỏ hàng sau khi thanh toán thành công
            HttpContext.Session.Set(MyConst.CART_KEY, new List<CartItem>());

            TempData["CheckoutSuccess"] = "Thanh toán thành công!";
            return RedirectToAction("Success", new { id = orderId });
        }

        // =========================
        // GET: /Checkout/Success
        // =========================
        [HttpGet]
        public IActionResult Success(int id)
        {
            var order = _orderDal.GetOrderById(id);
            if (order == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var items = _orderDal.GetItems(id);

            ViewBag.Order = order;
            ViewBag.Items = items;

            return View();
        }
    }
}
