using Microsoft.AspNetCore.Mvc;
using WebDT.DAL;
using WebDT.Models;

namespace WebD_T.ViewComponents
{
    public class FeaturedProductsViewComponent : ViewComponent
    {
        private readonly ProductDAL _productDal = new ProductDAL();

        public IViewComponentResult Invoke(int? limit)
        {
            int limitProduct = limit ?? 4; // nếu không truyền, mặc định lấy 4
            List<Product> featuredProducts = _productDal.GetFeaturedProducts(limitProduct);

            // View: Views/Shared/Components/FeaturedProducts/FeatureProduct.cshtml
            return View("FeatureProduct", featuredProducts);
        }
    }
}
