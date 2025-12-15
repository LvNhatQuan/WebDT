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

        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>(MyConst.CART_KEY)
                       ?? new List<CartItem>();

            if (!cart.Any())
                return RedirectToAction("Index", "Cart");

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var defaultAddress = _addressDal.GetDefaultAddressString(userId);

            var vm = new CheckoutViewModel
            {
                CartItems = cart,
                ShippingFee = 15000,
                Discount = 0,
                ShippingAddress = defaultAddress
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PlaceOrder(string shippingAddress, int? couponId)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>(MyConst.CART_KEY)
                       ?? new List<CartItem>();

            if (!cart.Any())
                return RedirectToAction("Index", "Cart");

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Nếu bạn muốn couponId áp cho toàn đơn -> chuyển thành discount_amount
            decimal discountAmount = 0;
            if (couponId.HasValue)
            {
                discountAmount = _couponDal.Apply(couponId.Value, cart.Sum(x => x.Total));
            }

            int orderId = _orderDal.CreateOrder(
                userId: userId,
                cart: cart,
                shippingFee: 15000,
                discountAmount: discountAmount,
                address: shippingAddress ?? ""
            );

            if (orderId <= 0)
                return RedirectToAction("Checkout");

            HttpContext.Session.Remove(MyConst.CART_KEY);

            return RedirectToAction("Success", new { id = orderId });
        }

        public IActionResult Success(int id)
        {
            var order = _orderDal.GetOrderById(id);
            if (order == null) return RedirectToAction("Index", "Home");

            order.Items = _orderDal.GetItems(id);
            return View(order);
        }
    }
}
