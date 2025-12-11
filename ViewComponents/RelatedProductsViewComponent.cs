using Microsoft.AspNetCore.Mvc;
using WebDT.DAL;
using WebDT.Models;

namespace WebDT.ViewComponents
{
    public class RelatedProductsViewComponent : ViewComponent
    {
        private readonly ProductDAL _productDal = new ProductDAL();

        public IViewComponentResult Invoke(int productId, int? limit)
        {
            int limitProduct = limit ?? 4;

            var mainProduct = _productDal.GetProductById(productId);
            if (mainProduct == null)
                return View("RelatedProducts", new List<Product>());


            var related = _productDal.GetRelatedProducts(productId, limitProduct);

            return View("RelatedProducts", related);
        }
    }
}
