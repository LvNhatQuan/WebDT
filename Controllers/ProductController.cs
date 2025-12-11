using Microsoft.AspNetCore.Mvc;
using WebDT.DAL;
using WebDT.Models;

namespace WebDT.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductDAL _productDAL = new ProductDAL();
        private readonly CategoryDAL _categoryDAL = new CategoryDAL();

        public IActionResult Index(int categoryId, int page = 1, string sortOrder = "")
        {
            int pageSize = 6;

            ViewBag.Categories = _categoryDAL.GetAllWithCount();

            var products = _productDAL.GetPaged(categoryId, page, pageSize, sortOrder);
            int totalProducts = _productDAL.CountProducts(categoryId);

            int maxPage = (int)Math.Ceiling((double)totalProducts / pageSize);

            var model = new ProductPagination
            {
                Products = products,
                CurrentPageIndex = page,
                PageCount = maxPage
            };

            return View(model);
        }

        public IActionResult Detail(int id)
        {
            var product = _productDAL.GetById(id);
            if (product == null) return NotFound();

            ViewBag.RelatedProducts = _productDAL.GetRelated(id, 6);
            return View(product);
        }

        public IActionResult Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return View(new List<Product>());

            var results = _productDAL.Search(keyword);
            ViewBag.SearchKeyword = keyword;

            return View(results);
        }

        public IActionResult Featured()
        {
            var products = _productDAL.GetFeatured(8);
            return View(products);
        }

        public IActionResult BestSellers()
        {
            var products = _productDAL.GetBestSeller(10);
            return View(products);
        }
    }
}
