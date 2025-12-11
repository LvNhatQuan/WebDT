using Microsoft.AspNetCore.Mvc;
using WebDT.DAL;
using WebDT.Models;

namespace WebDT.ViewComponents
{
    public class FeaturedProductsViewComponent : ViewComponent
    {
        private readonly ProductDAL _productDal = new ProductDAL();

        public IViewComponentResult Invoke(int? limit)
        {
            int limitProduct = limit ?? 4;

            var featuredProducts = _productDal.GetFeaturedProducts(limitProduct);

            return View("FeatureProduct", featuredProducts);
        }
    }
}
