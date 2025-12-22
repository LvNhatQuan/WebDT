using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebDT.Areas.Admin.DAL;

namespace WebDT.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = "staff")]
    public class StaffReviewController : Controller
    {
        private readonly ReviewAdminDAL _dal = new ReviewAdminDAL();

        // =========================
        // DANH SÁCH ĐÁNH GIÁ
        // =========================
        public IActionResult Index()
        {
            var reviews = _dal.GetAll();
            return View(reviews);
        }

        // =========================
        // STAFF ĐƯỢC PHÉP XÓA REVIEW
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            bool ok = _dal.Delete(id);

            TempData[ok ? "SuccessMessage" : "ErrorMessage"] =
                ok ? "Đã xóa đánh giá." : "Xóa thất bại.";

            return RedirectToAction(nameof(Index));
        }
    }
}
