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

            // Sử dụng phương thức GetProducts mới (đã tích hợp sắp xếp)
            var products = _productDal.GetProducts(page, pageSize, categoryId, sortOrder);

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
                return NotFound();

            var reviewDal = new ReviewDAL();
            var reviews = reviewDal.GetByProductId(id);

            ViewBag.Reviews = reviews;
            ViewBag.ReviewCount = reviews.Count;
            ViewBag.AvgRating = reviews.Count > 0 ? reviews.Average(r => r.Rating) : 0;

            return View(product);
        }

        // ============================
        // SEARCH
        // ============================
        public IActionResult Search(string keyword, int page = 1, string sortOrder = "")
        {
            int pageSize = 6;

            // Nếu keyword rỗng hoặc null, hiển thị tất cả sản phẩm
            if (string.IsNullOrWhiteSpace(keyword))
            {
                // Redirect về trang Index với tất cả sản phẩm
                return RedirectToAction("Index", new { page, sortOrder });
            }

            // Lấy danh sách sản phẩm với phân trang và sắp xếp
            var products = _productDal.SearchProducts(keyword, page, pageSize, sortOrder);

            // Lấy tổng số kết quả
            int totalRows = _productDal.GetTotalSearchResults(keyword);
            int pageCount = (int)Math.Ceiling((double)totalRows / pageSize);

            ViewData["SearchKeyword"] = keyword;
            ViewData["SortOrder"] = sortOrder;

            var model = new ProductPagination
            {
                Products = products,
                CurrentPageIndex = page,
                PageCount = pageCount,
                TotalItems = totalRows
            };

            return View(model);
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

        // ============================
        // SIMPLE SEARCH (for backward compatibility)
        // ============================
        public IActionResult SimpleSearch(string keyword)
        {
            var products = _productDal.SimpleSearch(keyword);
            return View(products);
        }
    }
}