using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebDT.Areas.Admin.DAL;
using WebDT.Areas.Admin.Models;
using WebDT.Areas.DAL;

namespace WebDT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class CategoryAdminController : Controller
    {
        private readonly CategoryAdminDAL categoryAdminDAL = new CategoryAdminDAL();
        private readonly AdminLogDAL _logDAL = new AdminLogDAL();

        // ================== INDEX ==================
        public IActionResult Index()
        {
            return View(categoryAdminDAL.getAll());
        }

        // ================== DETAILS ==================
        public IActionResult Details(int id)
        {
            var category = categoryAdminDAL.getCategoryById(id);

            if (category == null || category.Id == 0)
            {
                TempData["ErrorMessage"] = "Không tìm thấy danh mục.";
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // ================== CREATE (GET) ==================
        public IActionResult Create()
        {
            return View(new CategoryAdmin());
        }

        // ================== CREATE (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CategoryAdmin categoryNew)
        {
            if (!ModelState.IsValid)
                return View(categoryNew);

            bool isInserted = categoryAdminDAL.AddNew(categoryNew);

            if (isInserted)
            {
                _logDAL.InsertLog(
                    int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                    "Thêm danh mục",
                    "Danh mục",
                    $"Thêm danh mục: {categoryNew.Name}"
                );

                TempData["SuccessMessage"] = "Thêm danh mục thành công";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Thêm danh mục thất bại";
            return View(categoryNew);
        }

        // ================== EDIT (GET) ==================
        public IActionResult Edit(int id)
        {
            var category = categoryAdminDAL.getCategoryById(id);

            if (category == null || category.Id == 0)
            {
                TempData["ErrorMessage"] = "Không tìm thấy danh mục.";
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // ================== EDIT (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, CategoryAdmin categoryNew)
        {
            if (!ModelState.IsValid)
                return View(categoryNew);

            bool isUpdated = categoryAdminDAL.updateCategoryById(id, categoryNew);

            if (isUpdated)
            {
                _logDAL.InsertLog(
                    int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                    "Cập nhật danh mục",
                    "Danh mục",
                    $"Cập nhật danh mục ID = {id}"
                );

                TempData["SuccessMessage"] = "Cập nhật danh mục thành công";
            }
            else
            {
                TempData["ErrorMessage"] = "Cập nhật danh mục thất bại";
            }

            return RedirectToAction(nameof(Index));
        }

        // ================== DELETE (GET) ==================
        public IActionResult Delete(int id)
        {
            var category = categoryAdminDAL.getCategoryById(id);

            if (category == null || category.Id == 0)
            {
                TempData["ErrorMessage"] = "Không tìm thấy danh mục.";
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // ================== DELETE (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            bool isDeleted = categoryAdminDAL.deleteCategoryById(id);

            if (isDeleted)
            {
                _logDAL.InsertLog(
                    int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                    "Xóa danh mục",
                    "Danh mục",
                    $"Xóa danh mục ID = {id}"
                );

                TempData["SuccessMessage"] = "Xóa danh mục thành công";
            }
            else
            {
                TempData["ErrorMessage"] = "Xóa danh mục thất bại";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
