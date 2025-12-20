using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WebDT.Areas.Admin.DAL;
using WebDT.Areas.Admin.Models;
using WebDT.Areas.DAL;

namespace WebDT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin,staff")]
    public class CouponAdminController : Controller
    {
        CouponAdminDAL _dal = new CouponAdminDAL();
        ProductAdminDAL _productDAL = new ProductAdminDAL();

        // ================== INDEX ==================
        public IActionResult Index()
        {
            var coupons = _dal.GetAll();
            return View(coupons);
        }

        // ================== CREATE (GET) ==================
        public IActionResult Create(int productPage = 1)
        {
            // Lấy tất cả sản phẩm đang hoạt động
            var allProducts = _productDAL.getAll()
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name) // Sắp xếp theo tên
                .ToList();

            // Phân trang
            int pageSize = 30; // 30 sản phẩm mỗi trang
            int totalProducts = allProducts.Count;
            int totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            // Đảm bảo trang hợp lệ
            productPage = Math.Max(1, Math.Min(productPage, totalPages));

            // Lấy sản phẩm cho trang hiện tại
            var pagedProducts = allProducts
                .Skip((productPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new CouponFormViewModel
            {
                Id = 0, // Mark as create
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                AllProducts = pagedProducts,
                CurrentProductPage = productPage,
                ProductPageSize = pageSize,
                TotalProductPages = totalPages,
                TotalProducts = totalProducts
            };

            return View(viewModel);
        }

        // ================== CREATE (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CouponFormViewModel model)
        {
            // Kiểm tra model validation
            if (!ModelState.IsValid)
            {
                model.AllProducts = _productDAL.getAll().Where(p => p.IsActive).ToList();
                return View(model);
            }

            // Validate date range
            if (model.StartDate >= model.EndDate)
            {
                ModelState.AddModelError("EndDate", "Ngày kết thúc phải sau ngày bắt đầu");
                model.AllProducts = _productDAL.getAll().Where(p => p.IsActive).ToList();
                return View(model);
            }

            // Tạo coupon mới
            var coupon = new CouponAdmin
            {
                EventName = model.EventName,
                DiscountValue = model.DiscountValue,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                IsActive = model.IsActive
            };

            // Lưu coupon vào database
            bool couponCreated = _dal.Create(coupon);

            if (!couponCreated)
            {
                TempData["ErrorMessage"] = "Tạo mã giảm giá thất bại.";
                model.AllProducts = _productDAL.getAll().Where(p => p.IsActive).ToList();
                return View(model);
            }

            // Lấy ID của coupon vừa tạo
            var allCoupons = _dal.GetAll();
            var newCoupon = allCoupons.OrderByDescending(c => c.Id).FirstOrDefault();

            if (newCoupon != null && model.SelectedProductIds != null && model.SelectedProductIds.Any())
            {
                // Áp dụng coupon cho các sản phẩm được chọn
                bool productsUpdated = _dal.AssignCouponToProducts(newCoupon.Id, model.SelectedProductIds);

                if (!productsUpdated)
                {
                    TempData["WarningMessage"] = "Tạo mã giảm giá thành công nhưng áp dụng cho sản phẩm có lỗi.";
                }
                else
                {
                    TempData["SuccessMessage"] = $"Tạo mã giảm giá thành công. Đã áp dụng cho {model.SelectedProductIds.Count} sản phẩm.";
                }
            }
            else
            {
                TempData["SuccessMessage"] = "Tạo mã giảm giá thành công.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ================== DETAILS ==================
        public IActionResult Details(int id)
        {
            var coupon = _dal.GetById(id);
            if (coupon == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy mã giảm giá.";
                return RedirectToAction(nameof(Index));
            }

            var products = _dal.GetProductsByCouponId(id);

            // Tạo viewModel cho details
            var viewModel = new
            {
                Coupon = coupon,
                Products = products,
                ProductCount = products.Count
            };

            return View(viewModel);
        }

        // ================== EDIT (GET) ==================
        public IActionResult Edit(int id, int productPage = 1)
        {
            var coupon = _dal.GetById(id);
            if (coupon == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy mã giảm giá.";
                return RedirectToAction(nameof(Index));
            }

            // Lấy tất cả sản phẩm đang hoạt động
            var allProducts = _productDAL.getAll()
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name) // Sắp xếp theo tên để dễ tìm
                .ToList();

            // Phân trang - chỉ lấy dữ liệu cần hiển thị
            int pageSize = 30; // Tăng lên 30 sản phẩm mỗi trang để ít phân trang hơn
            int totalProducts = allProducts.Count;
            int totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            // Đảm bảo trang hợp lệ
            productPage = Math.Max(1, Math.Min(productPage, totalPages));

            // Lấy sản phẩm cho trang hiện tại
            var pagedProducts = allProducts
                .Skip((productPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Lấy sản phẩm đang áp dụng coupon này
            var selectedProductIds = _dal.GetProductsByCouponId(id).Select(p => p.Id).ToList();

            var viewModel = new CouponFormViewModel
            {
                Id = coupon.Id,
                EventName = coupon.EventName,
                DiscountValue = coupon.DiscountValue,
                StartDate = coupon.StartDate,
                EndDate = coupon.EndDate,
                IsActive = coupon.IsActive,
                SelectedProductIds = selectedProductIds,
                AllProducts = pagedProducts,
                CurrentProductPage = productPage,
                ProductPageSize = pageSize,
                TotalProductPages = totalPages,
                TotalProducts = totalProducts
            };

            return View(viewModel);
        }

        // ================== EDIT (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, CouponFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AllProducts = _productDAL.getAll().Where(p => p.IsActive).ToList();
                return View(model);
            }

            // Validate date range
            if (model.StartDate >= model.EndDate)
            {
                ModelState.AddModelError("EndDate", "Ngày kết thúc phải sau ngày bắt đầu");
                model.AllProducts = _productDAL.getAll().Where(p => p.IsActive).ToList();
                return View(model);
            }

            // Cập nhật thông tin coupon
            var coupon = new CouponAdmin
            {
                Id = id,
                EventName = model.EventName,
                DiscountValue = model.DiscountValue,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                IsActive = model.IsActive
            };

            bool ok = _dal.Update(coupon);

            if (ok)
            {
                // Cập nhật sản phẩm áp dụng coupon
                if (model.SelectedProductIds != null && model.SelectedProductIds.Any())
                {
                    _dal.AssignCouponToProducts(id, model.SelectedProductIds);
                    TempData["SuccessMessage"] = $"Cập nhật mã giảm giá thành công. Đã áp dụng cho {model.SelectedProductIds.Count} sản phẩm.";
                }
                else
                {
                    // Nếu không chọn sản phẩm nào, xóa coupon khỏi tất cả sản phẩm
                    _dal.AssignCouponToProducts(id, new List<int>());
                    TempData["SuccessMessage"] = "Cập nhật mã giảm giá thành công. Đã xóa coupon khỏi tất cả sản phẩm.";
                }
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Cập nhật mã giảm giá thất bại.";
            model.AllProducts = _productDAL.getAll().Where(p => p.IsActive).ToList();
            return View(model);
        }

        // ================== TOGGLE STATUS ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id)
        {
            var coupon = _dal.GetById(id);
            if (coupon == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy mã giảm giá.";
                return RedirectToAction(nameof(Index));
            }

            bool newStatus = !coupon.IsActive;
            bool ok = _dal.UpdateStatus(id, newStatus);

            if (ok)
            {
                TempData["SuccessMessage"] = $"Đã {(newStatus ? "kích hoạt" : "vô hiệu hóa")} mã giảm giá thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Thay đổi trạng thái thất bại.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ================== DELETE (GET) ==================
        public IActionResult Delete(int id)
        {
            var coupon = _dal.GetById(id);
            if (coupon == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy mã giảm giá.";
                return RedirectToAction(nameof(Index));
            }

            var products = _dal.GetProductsByCouponId(id);

            var viewModel = new
            {
                Coupon = coupon,
                Products = products,
                ProductCount = products.Count
            };

            return View(viewModel);
        }

        // ================== DELETE (POST) ==================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            bool ok = _dal.Delete(id);

            if (ok)
            {
                TempData["SuccessMessage"] = "Xóa mã giảm giá thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Xóa mã giảm giá thất bại.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
