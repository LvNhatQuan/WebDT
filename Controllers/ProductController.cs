using Microsoft.AspNetCore.Mvc;
using WebDT.DAL;

namespace WebDT.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductDAL _productDal = new ProductDAL();

        // GET: /Product
        public IActionResult Index(int? categoryId)
        {
            if (categoryId.HasValue)
            {
                var products = _productDal.GetProductsByCategory(categoryId.Value);
                return View(products);
            }

            var allProducts = _productDal.GetAllProducts();
            return View(allProducts);
        }

        // GET: /Product/Detail/5
        public IActionResult Detail(int id)
        {
            var product = _productDal.GetProductById(id);

            if (product == null)
                return NotFound("Không tìm thấy sản phẩm");

            return View(product);
        }
    }
}
