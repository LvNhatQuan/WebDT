using Microsoft.AspNetCore.Mvc;
using WebDT.Helper;
using WebDT.Models;

namespace WebDT.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>(MyConst.CART_KEY) ?? new List<CartItem>();

            return View(new CartModel()
            {
                Quantity = cart.Sum(p => p.Quantity),
                Total = cart.Sum(p => p.Total)
            });
        }
    }
}
