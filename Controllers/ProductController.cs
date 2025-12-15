using Microsoft.AspNetCore.Mvc;
using WebDT.DAL;
using WebDT.Models;

namespace WebDT.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductDAL _productDal = new ProductDAL();

        // ============================
        // PRODUCT LIST + PAGINATION
        // ============================
        public IActionResult Index(int? categoryId, int page = 1, string sortOrder = "")
        {
            int pageSize = 6;

            ViewData["CategoryId"] = categoryId;
            ViewData["SortOrder"] = sortOrder;

            // Trường hợp không filter → load nhanh toàn bộ (trang chủ / demo)
            if (!categoryId.HasValue && string.IsNullOrEmpty(sortOrder))
            {
                var allProducts = _productDal.GetAllProducts();

                return View(new ProductPagination
                {
                    Products = allProducts,
                    CurrentPageIndex = 1,
                    PageCount = 1
                });
            }

            // Pagination
            List<Product> products = _productDal.GetProducts_Pagination(
                page,
                pageSize,
                categoryId,
                sortOrder
            );

            int totalRows = _productDal.GetTotalProducts(categoryId);
            int pageCount = (int)Math.Ceiling((double)totalRows / pageSize);

            var model = new ProductPagination
            {
                Products = products,
                CurrentPageIndex = page,
                PageCount = pageCount
            };

            return View(model);
        }

        // ============================
        // PRODUCT DETAIL
        // ============================
        public IActionResult Detail(int id)
        {
            var product = _productDal.GetProductById(id);

            if (product == null)
                return NotFound("Không tìm thấy sản phẩm");

            return View(product);
        }

        // ============================
        // SEARCH
        // ============================
        public IActionResult Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                ViewBag.Message = "Vui lòng nhập từ khóa để tìm kiếm.";
                return View(new List<Product>());
            }

            var products = _productDal.SearchProducts(keyword);
            ViewData["SearchKeyword"] = keyword;

            return View(products);
        }

        // ============================
        // CATEGORY REDIRECT
        // ============================
        public IActionResult Category(int id, int page = 1, string sortOrder = "")
        {
            return RedirectToAction("Index", new
            {
                categoryId = id,
                page,
                sortOrder
            });
        }

        // ============================
        // FEATURED PRODUCTS
        // ============================
        public IActionResult Featured()
        {
            var items = _productDal.GetFeaturedProducts(8);
            return View(items);
        }
    }
}
