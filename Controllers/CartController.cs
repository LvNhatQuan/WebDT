using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebDT.DAL;
using WebDT.Helper;
using WebDT.Models;

namespace WebDT.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ProductDAL _productDal = new ProductDAL();

        // =========================
        // VIEW CART
        // =========================
        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        // =========================
        // ADD TO CART (POST ONLY)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(int productId, int quantity)
        {
            var cart = GetCart();

            var product = _productDal.GetProductById(productId);
            if (product == null)
                return NotFound();

            var item = cart.FirstOrDefault(x => x.IdProduct == productId);
            if (item != null)
            {
                item.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    IdProduct = product.Id,
                    Name = product.Name,
                    Img = product.ImageUrl,
                    Price = product.Price,
                    Quantity = quantity,
                    CouponId = product.CouponId,
                    Discount = product.Discount
                });
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // =========================
        // REMOVE
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int productId)
        {
            var cart = GetCart();
            cart.RemoveAll(x => x.IdProduct == productId);
            SaveCart(cart);

            return RedirectToAction("Index");
        }

        // =========================
        // UPDATE
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int productId, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.IdProduct == productId);

            if (item != null)
            {
                if (quantity <= 0)
                    cart.Remove(item);
                else
                    item.Quantity = quantity;
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // =========================
        // SESSION HELPERS
        // =========================
        private List<CartItem> GetCart()
        {
            return HttpContext.Session.Get<List<CartItem>>(MyConst.CART_KEY)
                   ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.Set(MyConst.CART_KEY, cart);
        }
    }
}
