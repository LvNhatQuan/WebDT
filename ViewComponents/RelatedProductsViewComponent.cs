using Microsoft.AspNetCore.Mvc;
using WebDT.DAL;
using WebDT.Models;

namespace WebD_T.ViewComponents
{
    public class RelatedProductsViewComponent : ViewComponent
    {
        private readonly ProductDAL _productDal = new ProductDAL();

        public IViewComponentResult Invoke(int productId, int? limit)
        {
            int limitProduct = limit ?? 4;
            List<Product> relatedProducts = _productDal.GetRelatedProducts(productId, limitProduct);

            // View: Views/Shared/Components/RelatedProducts/RelatedProducts.cshtml
            return View("RelatedProducts", relatedProducts);
        }
    }
}
