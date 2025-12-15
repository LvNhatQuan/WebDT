using Microsoft.AspNetCore.Mvc;
using WebDT.DAL;
using WebDT.Models;

namespace WebDT.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductDAL _productDal = new ProductDAL();

        // ============================
        //        PRODUCT LIST
        // ============================
        // Hỗ trợ: categoryId, search, sort, pagination
        public IActionResult Index(int? categoryId, int page = 1, string sortOrder = "")
        {
            int pageSize = 6;

            // Lưu thông tin filter vào ViewData để View dùng lại
            ViewData["CategoryId"] = categoryId;
            ViewData["SortOrder"] = sortOrder;

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



            // Ngược lại → dùng phân trang
            List<Product> products = _productDal.GetProducts_Pagination(page, pageSize, categoryId, sortOrder);

            int totalRows = _productDal.GetTotalProducts(categoryId);
            int maxPage = (int)Math.Ceiling((double)totalRows / pageSize);

            ProductPagination model = new ProductPagination
            {
                Products = products,
                CurrentPageIndex = page,
                PageCount = maxPage
            };

            return View(model);
        }

        // ============================
        //        PRODUCT DETAIL
        // ============================
        public IActionResult Detail(int id)
        {
            var product = _productDal.GetProductById(id);

            if (product == null)
                return NotFound("Không tìm thấy sản phẩm");

            return View(product);
        }

        // ============================
        //          SEARCH
        // ============================
        public IActionResult Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                ViewBag.Message = "Vui lòng nhập từ khóa để tìm kiếm.";
                return View(new List<Product>());
            }

            List<Product> products = _productDal.SearchProducts(keyword);
            ViewData["SearchKeyword"] = keyword;

            return View(products);
        }

        // ============================
        //      CATEGORY REDIRECT
        // ============================
        public IActionResult Category(int id, int page = 1, string sortOrder = "")
        {
            return RedirectToAction("Index", new { categoryId = id, page, sortOrder });
        }

        // ============================
        //       FEATURED VIEW PAGE
        // ============================
        public IActionResult Featured()
        {
            var items = _productDal.GetFeaturedProducts(8);
            return View(items);
        }
    }
}
