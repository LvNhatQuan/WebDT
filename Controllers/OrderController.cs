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
        private readonly OrderDAL _orderDal = new OrderDAL();
        private readonly AddressDAL _addressDal = new AddressDAL();
        private readonly CouponDAL _couponDal = new CouponDAL();

        private const decimal SHIPPING_FEE = 15000;

        // ================== CHECKOUT ==================
        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = HttpContext.Session
                .Get<List<CartItem>>(MyConst.CART_KEY)
                ?? new List<CartItem>();

            if (!cart.Any())
                return RedirectToAction("Index", "Cart");

            int userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var defaultAddress = _addressDal.GetDefaultAddressString(userId);

            var vm = new CheckoutViewModel
            {
                CartItems = cart,
                ShippingFee = SHIPPING_FEE,
                Discount = 0, // hiển thị tạm, server sẽ tính lại
                ShippingAddress = defaultAddress
            };

            return View(vm);
        }

        // ================== PLACE ORDER ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PlaceOrder(
            string receiverName,
            string receiverPhone,
            string shippingAddress,
            int? couponId
        )
        {
            var cart = HttpContext.Session
                .Get<List<CartItem>>(MyConst.CART_KEY)
                ?? new List<CartItem>();

            if (!cart.Any())
                return RedirectToAction("Index", "Cart");

            // validate input
            if (string.IsNullOrWhiteSpace(receiverName) ||
                string.IsNullOrWhiteSpace(receiverPhone) ||
                string.IsNullOrWhiteSpace(shippingAddress))
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin nhận hàng.";
                return RedirectToAction("Checkout");
            }

            int userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            // ================== XỬ LÝ COUPON (SERVER) ==================
            decimal discountAmount = 0;

            if (couponId.HasValue)
            {
                // gán coupon cho từng item (để insert order_items)
                foreach (var item in cart)
                    item.CouponId = couponId.Value;

                discountAmount = _couponDal.Apply(
                    couponId.Value,
                    cart.Sum(x => x.Total)
                );
            }

            // ================== TẠO ĐƠN HÀNG ==================
            int orderId = _orderDal.CreateOrder(
                userId: userId,
                cart: cart,
                shippingFee: SHIPPING_FEE,
                discountAmount: discountAmount,
                address: shippingAddress,
                receiverName: receiverName,
                receiverPhone: receiverPhone
            );

            if (orderId <= 0)
            {
                TempData["Error"] = "Đặt hàng thất bại. Vui lòng thử lại.";
                return RedirectToAction("Checkout");
            }

            // clear cart
            HttpContext.Session.Remove(MyConst.CART_KEY);

            return RedirectToAction("Success", new { id = orderId });
        }

        // ================== SUCCESS ==================
        [HttpGet]
        public IActionResult Success(int id)
        {
            var order = _orderDal.GetOrderById(id);
            if (order == null)
                return RedirectToAction("Index", "Home");

            order.Items = _orderDal.GetItems(id);
            return View(order);
        }
    }
}
