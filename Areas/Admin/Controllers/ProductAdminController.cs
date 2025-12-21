using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using WebDT.Areas.Admin.DAL;
using WebDT.Areas.Admin.Models;
using WebDT.Areas.DAL;
using WebDT.Helper;

namespace WebDT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class ProductAdminController : Controller
    {
        private readonly ProductAdminDAL productDAL = new ProductAdminDAL();
        private readonly CategoryAdminDAL categoryDAL = new CategoryAdminDAL();
        private readonly AdminLogDAL _logDAL = new AdminLogDAL();

        // ==================== INDEX ====================
        public IActionResult Index()
        {
            return View(productDAL.getAll());
        }

        // ==================== DETAILS ====================
        public IActionResult Details(int id)
        {
            var product = productDAL.GetProductById(id);
            if (product == null || product.Id == 0)
            {
                TempData["ErrorMessage"] = "Không tìm thấy sản phẩm.";
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // ==================== CREATE (GET) ====================
        public IActionResult Create()
        {
            return View(new ProductFormAdmin
            {
                IsActive = true,
                StockQuantity = 0,
                ListCategory = categoryDAL.getAll()
                    .Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString()
                    }).ToList()
            });
        }

        // ==================== CREATE (POST) ====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductFormAdmin model, IFormFile Img)
        {
            if (model.CategoryIdSelected.HasValue)
                model.CategoryId = model.CategoryIdSelected.Value;
            else
                ModelState.AddModelError("CategoryIdSelected", "Vui lòng chọn danh mục");

            ModelState.Remove(nameof(model.CategoryId));

            if (!ModelState.IsValid)
            {
                model.ListCategory = categoryDAL.getAll()
                    .Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString()
                    }).ToList();
                return View(model);
            }

            model.CreatedAt = DateTime.Now;
            model.ImageUrl = Img != null && Img.Length > 0
                ? ImageHelper.UpLoadImage(Img, "SanPham")
                : string.Empty;

            if (productDAL.AddNew(model))
            {
                _logDAL.InsertLog(
                    int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                    "Thêm sản phẩm",
                    "Sản phẩm",
                    $"Thêm sản phẩm: {model.Name}"
                );

                TempData["SuccessMessage"] = "Thêm sản phẩm thành công";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Không thể thêm sản phẩm";
            return View(model);
        }

        // ==================== EDIT (GET) ====================
        public IActionResult Edit(int id)
        {
            var product = productDAL.GetProductById(id);
            if (product == null || product.Id == 0)
                return RedirectToAction(nameof(Index));

            return View(new ProductFormAdmin
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                CategoryId = product.CategoryId,
                CategoryIdSelected = product.CategoryId,
                ListCategory = categoryDAL.getAll()
                    .Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString(),
                        Selected = c.Id == product.CategoryId
                    }).ToList()
            });
        }

        // ==================== EDIT (POST) ====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ProductFormAdmin model, IFormFile? ImageUpload)
        {
            if (model.CategoryIdSelected.HasValue)
                model.CategoryId = model.CategoryIdSelected.Value;
            else
                ModelState.AddModelError("CategoryIdSelected", "Vui lòng chọn danh mục");

            ModelState.Remove(nameof(model.CategoryId));

            if (!ModelState.IsValid)
                return View(model);

            var old = productDAL.GetProductById(id);
            if (old == null)
                return RedirectToAction(nameof(Index));

            model.ImageUrl = ImageUpload != null && ImageUpload.Length > 0
                ? ImageHelper.UpLoadImage(ImageUpload, "SanPham")
                : old.ImageUrl;

            model.CreatedAt = old.CreatedAt;
            model.Id = id;

            if (productDAL.UpdateProduct(model, id))
            {
                _logDAL.InsertLog(
                    int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                    "Cập nhật sản phẩm",
                    "Sản phẩm",
                    $"Cập nhật sản phẩm ID = {id}"
                );

                TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Cập nhật thất bại";
            return View(model);
        }

        // ==================== DELETE (GET) ====================
        public IActionResult Delete(int id)
        {
            var product = productDAL.GetProductById(id);
            if (product == null)
                return RedirectToAction(nameof(Index));
            return View(product);
        }

        // ==================== DELETE (POST) ====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (productDAL.DeleteProduct(id))
            {
                _logDAL.InsertLog(
                    int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                    "Xóa sản phẩm",
                    "Sản phẩm",
                    $"Xóa sản phẩm ID = {id}"
                );

                TempData["SuccessMessage"] = "Xóa sản phẩm thành công";
            }
            else
            {
                TempData["ErrorMessage"] = "Xóa sản phẩm thất bại";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
