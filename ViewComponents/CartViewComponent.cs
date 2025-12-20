using Microsoft.AspNetCore.Mvc;
using WebDT.Models;
using WebDT.Helper;

namespace WebDT.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session
                .Get<List<CartItem>>(MyConst.CART_KEY)
                ?? new List<CartItem>();

            int totalQuantity = cart.Sum(x => x.Quantity);

            return View(totalQuantity);
        }
    }
}
