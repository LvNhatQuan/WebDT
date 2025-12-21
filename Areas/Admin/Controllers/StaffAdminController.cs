using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebDT.Areas.Admin.DAL;
using WebDT.Areas.Admin.Models;

namespace WebDT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class StaffAdminController : Controller
    {
        private readonly StaffAdminDAL _dal = new StaffAdminDAL();
        private readonly AdminLogDAL _logDAL = new AdminLogDAL();

        // ================== INDEX ==================
        public IActionResult Index()
        {
            return View(_dal.GetAll());
        }

        // ================== CREATE (GET) ==================
        public IActionResult Create()
        {
            return View(new StaffAdmin());
        }

        // ================== CREATE (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(StaffAdmin model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (_dal.Create(model))
            {
                _logDAL.InsertLog(
                    int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                    "Thêm nhân viên",
                    "Nhân viên",
                    $"Thêm staff: {model.FullName}"
                );

                TempData["SuccessMessage"] = "Thêm nhân viên thành công";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Thêm nhân viên thất bại";
            return View(model);
        }

        // ================== EDIT (GET) ==================
        public IActionResult Edit(int id)
        {
            var staff = _dal.GetById(id);
            if (staff == null)
                return RedirectToAction(nameof(Index));

            return View(staff);
        }

        // ================== EDIT (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, StaffAdmin model)
        {
            model.Id = id;

            if (_dal.Update(model))
            {
                _logDAL.InsertLog(
                    int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                    "Cập nhật nhân viên",
                    "Nhân viên",
                    $"Cập nhật staff ID = {id}"
                );

                TempData["SuccessMessage"] = "Cập nhật thành công";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Cập nhật thất bại";
            return View(model);
        }

        // ================== TOGGLE LOCK ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleLock(int id, bool lockState)
        {
            bool ok = _dal.ToggleLock(id, lockState);

            if (ok)
            {
                _logDAL.InsertLog(
                    int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                    lockState ? "Khóa nhân viên" : "Mở khóa nhân viên",
                    "Nhân viên",
                    $"Thay đổi trạng thái staff ID = {id}"
                );

                TempData["SuccessMessage"] =
                    lockState ? "Đã khóa nhân viên" : "Đã mở khóa nhân viên";
            }
            else
            {
                TempData["ErrorMessage"] = "Thay đổi trạng thái thất bại";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
