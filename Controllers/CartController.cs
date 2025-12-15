using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebDT.DAL;
using WebDT.Helper;
using WebDT.Models;

namespace WebDT.Controllers
{
    public class CartController : Controller
    {
        private readonly ProductDAL _productDal = new ProductDAL();
        private readonly UserDAL _userDal = new UserDAL();
        private readonly CartDAL _cartDal = new CartDAL();
        private readonly AddressDAL _addressDal = new AddressDAL();

        public List<CartItem> Cart =>
            HttpContext.Session.Get<List<CartItem>>(MyConst.CART_KEY)
            ?? new List<CartItem>();

        public IActionResult Index()
        {
            return View(Cart);
        }

        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var claim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Vui lòng đăng nhập.",
                    redirectUrl = "/Account/Login"
                });
            }

            var cart = Cart;

            var item = cart.SingleOrDefault(p => p.IdProduct == id);
            Product p = _productDal.GetProductById(id);

            if (item == null)
            {
                item = new CartItem
                {
                    IdProduct = p.Id,
                    Name = p.Name,
                    Img = p.Image_url,
                    Price = p.Price,
                    Discount = p.Discount,
                    Quantity = quantity
                };
                cart.Add(item);
            }
            else
            {
                item.Quantity += quantity;
            }

            HttpContext.Session.Set(MyConst.CART_KEY, cart);

            return Json(new
            {
                success = true,
                cartCount = cart.Sum(x => x.Quantity),
                cartTotal = cart.Sum(x => x.Total)
            });
        }
    }
}
