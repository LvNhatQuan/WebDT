using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebDT.Areas.Admin.DAL;
using WebDT.Areas.Admin.Models;

namespace WebDT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin,staff")]
    public class CouponAdminController : Controller
    {
        private readonly CouponAdminDAL _dal = new CouponAdminDAL();
        private readonly ProductAdminDAL _productDAL = new ProductAdminDAL();
        private readonly AdminLogDAL _logDAL = new AdminLogDAL();

        // ================== INDEX ==================
        public IActionResult Index()
        {
            return View(_dal.GetAll());
        }

        // ================== CREATE (GET) ==================
        public IActionResult Create(int productPage = 1)
        {
            var allProducts = _productDAL.getAll()
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToList();

            int pageSize = 30;
            int totalProducts = allProducts.Count;
            int totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            productPage = Math.Max(1, Math.Min(productPage, totalPages));

            var pagedProducts = allProducts
                .Skip((productPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return View(new CouponFormViewModel
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                AllProducts = pagedProducts,
                CurrentProductPage = productPage,
                ProductPageSize = pageSize,
                TotalProductPages = totalPages,
                TotalProducts = totalProducts
            });
        }

        // ================== CREATE (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CouponFormViewModel model)
        {
            if (!ModelState.IsValid || model.StartDate >= model.EndDate)
            {
                model.AllProducts = _productDAL.getAll().Where(p => p.IsActive).ToList();
                if (model.StartDate >= model.EndDate)
                    ModelState.AddModelError("EndDate", "Ngày kết thúc phải sau ngày bắt đầu");
                return View(model);
            }

            var coupon = new CouponAdmin
            {
                EventName = model.EventName,
                DiscountValue = model.DiscountValue,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                IsActive = model.IsActive
            };

            if (!_dal.Create(coupon))
            {
                TempData["ErrorMessage"] = "Tạo mã giảm giá thất bại.";
                model.AllProducts = _productDAL.getAll().Where(p => p.IsActive).ToList();
                return View(model);
            }

            _logDAL.InsertLog(
                int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                "Thêm giảm giá",
                "Giảm giá",
                $"Tạo coupon {coupon.EventName} - giảm {coupon.DiscountValue}%"
            );

            var newCoupon = _dal.GetAll().OrderByDescending(x => x.Id).FirstOrDefault();

            if (newCoupon != null && model.SelectedProductIds?.Any() == true)
                _dal.AssignCouponToProducts(newCoupon.Id, model.SelectedProductIds);

            TempData["SuccessMessage"] = "Tạo mã giảm giá thành công.";
            return RedirectToAction(nameof(Index));
        }

        // ================== EDIT (GET) ==================
        public IActionResult Edit(int id, int productPage = 1)
        {
            var coupon = _dal.GetById(id);
            if (coupon == null) return RedirectToAction(nameof(Index));

            var allProducts = _productDAL.getAll().Where(p => p.IsActive).OrderBy(p => p.Name).ToList();

            int pageSize = 30;
            int totalPages = (int)Math.Ceiling((double)allProducts.Count / pageSize);
            productPage = Math.Max(1, Math.Min(productPage, totalPages));

            return View(new CouponFormViewModel
            {
                Id = coupon.Id,
                EventName = coupon.EventName,
                DiscountValue = coupon.DiscountValue,
                StartDate = coupon.StartDate,
                EndDate = coupon.EndDate,
                IsActive = coupon.IsActive,
                SelectedProductIds = _dal.GetProductsByCouponId(id).Select(p => p.Id).ToList(),
                AllProducts = allProducts.Skip((productPage - 1) * pageSize).Take(pageSize).ToList(),
                CurrentProductPage = productPage,
                ProductPageSize = pageSize,
                TotalProductPages = totalPages,
                TotalProducts = allProducts.Count
            });
        }

        // ================== EDIT (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, CouponFormViewModel model)
        {
            if (!ModelState.IsValid || model.StartDate >= model.EndDate)
            {
                model.AllProducts = _productDAL.getAll().Where(p => p.IsActive).ToList();
                if (model.StartDate >= model.EndDate)
                    ModelState.AddModelError("EndDate", "Ngày kết thúc phải sau ngày bắt đầu");
                return View(model);
            }

            if (_dal.Update(new CouponAdmin
            {
                Id = id,
                EventName = model.EventName,
                DiscountValue = model.DiscountValue,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                IsActive = model.IsActive
            }))
            {
                _logDAL.InsertLog(
                    int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                    "Cập nhật giảm giá",
                    "Giảm giá",
                    $"Cập nhật coupon ID = {id}"
                );

                _dal.AssignCouponToProducts(id, model.SelectedProductIds ?? new List<int>());
                TempData["SuccessMessage"] = "Cập nhật mã giảm giá thành công.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Cập nhật mã giảm giá thất bại.";
            return View(model);
        }

        // ================== DELETE (GET) ==================
        public IActionResult Delete(int id)
        {
            var coupon = _dal.GetById(id);
            if (coupon == null) return RedirectToAction(nameof(Index));

            var productCount = _dal.GetProductsByCouponId(id).Count;

            ViewBag.Coupon = coupon;
            ViewBag.ProductCount = productCount;

            return View();
        }

        // ================== DELETE (POST) - ĐƠN GIẢN HÓA ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, bool confirm = false)
        {
            if (_dal.Delete(id))
            {
                _logDAL.InsertLog(
                    int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                    "Xóa giảm giá",
                    "Giảm giá",
                    $"Xóa coupon ID = {id}"
                );
                TempData["SuccessMessage"] = "Xóa mã giảm giá thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Xóa mã giảm giá thất bại.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ================== TOGGLE STATUS ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("ToggleStatus")]
        public IActionResult ToggleStatusPost(int id)
        {
            var coupon = _dal.GetById(id);
            if (coupon != null)
            {
                var newStatus = !coupon.IsActive;
                if (_dal.UpdateStatus(id, newStatus))
                {
                    _logDAL.InsertLog(
                        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                        "Thay đổi trạng thái giảm giá",
                        "Giảm giá",
                        $"Thay đổi trạng thái coupon ID = {id} thành {(newStatus ? "Bật" : "Tắt")}"
                    );
                    TempData["SuccessMessage"] = "Thay đổi trạng thái thành công.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Thay đổi trạng thái thất bại.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy mã giảm giá.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}