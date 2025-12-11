using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebDT.Areas.Admin.Models;
using WebDT.Areas.DAL;

namespace WebDT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class CategoryAdminController : Controller
    {
        CategoryAdminDAL categoryAdminDAL = new CategoryAdminDAL();

        // GET: CategoryAdminController 
        public ActionResult Index()
        {
            List<CategoryAdmin> categories = new List<CategoryAdmin>();
            categories = categoryAdminDAL.getAll();

            return View(categories);
        }

        // GET: CategoryAdminController/Details/5
        public ActionResult Details(int id)
        {
            var category = categoryAdminDAL.getCategoryById(id);

            if (category == null || category.Id == 0)
            {
                TempData["ErrorMessage"] = "Không tìm thấy danh mục.";
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // GET: CategoryAdminController/Create 
        public ActionResult Create()
        {
            return View(new CategoryAdmin());
        }

        // POST: CategoryAdminController/Create 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CategoryAdmin categoryNew)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(categoryNew);
                }

                bool isInserted = categoryAdminDAL.AddNew(categoryNew);

                if (isInserted)
                {
                    TempData["SuccessMessage"] = "Thêm danh mục thành công";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Thêm danh mục thất bại";
                    return View(categoryNew);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(categoryNew);
            }
        }

       

        // GET: CategoryAdminController/Edit/5
        public ActionResult Edit(int id)
        {
            CategoryAdmin category = new CategoryAdmin();

            category = categoryAdminDAL.getCategoryById(id);

            return View(category);
        }

        // POST: CategoryAdminController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, CategoryAdmin categoryNew)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(categoryNew);
                }

                Console.WriteLine($"Update Category Id = {id}");

                bool isUpdated = categoryAdminDAL.updateCategoryById(id, categoryNew);

                if (isUpdated)
                {
                    Console.WriteLine("Update Success");
                    TempData["SuccessMessage"] = "Cập nhật danh mục thành công";
                }
                else
                {
                    Console.WriteLine("Update Fail");
                    TempData["ErrorMessage"] = "Cập nhật danh mục thất bại";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Update error: " + ex.Message);
                TempData["ErrorMessage"] = ex.Message;
                return View(categoryNew);
            }
        }


        // GET: CategoryAdminController/Delete/5
        public ActionResult Delete(int id)
        {
            var category = categoryAdminDAL.getCategoryById(id);

            if (category == null || category.Id == 0)
            {
                TempData["ErrorMessage"] = "Không tìm thấy danh mục.";
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // POST: CategoryAdminController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                // Không cần ModelState ở đây, chỉ cần id là đủ
                bool isDeleted = categoryAdminDAL.deleteCategoryById(id);

                if (isDeleted)
                {
                    TempData["SuccessMessage"] = "Xóa danh mục thành công";
                }
                else
                {
                    TempData["ErrorMessage"] = "Xóa danh mục thất bại (id không tồn tại hoặc không xóa được)";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Delete error " + ex.Message);
                TempData["ErrorMessage"] = "Lỗi xóa: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }


    }
}
