using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebDT.Areas.Admin.DAL;

namespace WebDT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class ReviewAdminController : Controller
    {
        private readonly ReviewAdminDAL _dal = new ReviewAdminDAL();
        private readonly AdminLogDAL _logDAL = new AdminLogDAL();

        // ================== INDEX ==================
        public IActionResult Index()
        {
            return View(_dal.GetAll());
        }

        // ================== DELETE ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            bool ok = _dal.Delete(id);

            if (ok)
            {
                _logDAL.InsertLog(
                    int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                    "Xóa đánh giá",
                    "Đánh giá",
                    $"Xóa review ID = {id}"
                );

                TempData["SuccessMessage"] = "Xóa đánh giá thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Xóa đánh giá thất bại.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
