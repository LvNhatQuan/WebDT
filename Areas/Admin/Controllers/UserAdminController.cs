using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebDT.Areas.Admin.DAL;
using WebDT.Areas.Admin.Models;
using WebDT.Models;

namespace WebDT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class UserAdminController : Controller
    {
        private readonly UserAdminDAL _dal = new UserAdminDAL();
        private readonly AdminLogDAL _logDAL = new AdminLogDAL();

        // ================== INDEX ==================
        public IActionResult Index()
        {
            var users = _dal.GetAll();

            var list = users.Select(u => new UserAdmin
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber,
                Role = u.Role
            }).ToList();

            return View(list);
        }

        // ================== DETAILS ==================
        public IActionResult Details(int id)
        {
            var u = _dal.GetById(id);
            if (u == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản.";
                return RedirectToAction(nameof(Index));
            }

            return View(new UserAdmin
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber,
                Role = u.Role,
                Password = u.Password
            });
        }

        // ================== CREATE (GET) ==================
        public IActionResult Create()
        {
            return View(new UserAdmin());
        }

        // ================== CREATE (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UserAdmin model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                Password = model.Password ?? "",
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                Role = model.Role
            };

            if (_dal.Create(user))
            {
                _logDAL.InsertLog(
                    int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                    "Thêm tài khoản",
                    "Tài khoản",
                    $"Tạo user username = {model.Username}, role = {model.Role}"
                );

                TempData["SuccessMessage"] = "Tạo tài khoản thành công.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Tạo tài khoản thất bại.";
            return View(model);
        }

        // ================== EDIT (GET) ==================
        public IActionResult Edit(int id)
        {
            var u = _dal.GetById(id);
            if (u == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản.";
                return RedirectToAction(nameof(Index));
            }

            return View(new UserAdmin
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber,
                Role = u.Role,
                Password = u.Password
            });
        }

        // ================== EDIT (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, UserAdmin model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var oldUser = _dal.GetById(id);
            if (oldUser == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản.";
                return RedirectToAction(nameof(Index));
            }

            var user = new User
            {
                Id = id,
                Username = model.Username,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                Role = model.Role,
                Password = string.IsNullOrEmpty(model.Password)
                    ? oldUser.Password
                    : model.Password
            };

            if (_dal.Update(user))
            {
                _logDAL.InsertLog(
                    int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                    "Cập nhật tài khoản",
                    "Tài khoản",
                    $"Cập nhật user ID = {id}, role = {model.Role}"
                );

                TempData["SuccessMessage"] = "Cập nhật tài khoản thành công.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Cập nhật thất bại.";
            return View(model);
        }

        // ================== DELETE (GET) ==================
        public IActionResult Delete(int id)
        {
            var u = _dal.GetById(id);
            if (u == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản.";
                return RedirectToAction(nameof(Index));
            }

            return View(new UserAdmin
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber,
                Role = u.Role
            });
        }

        // ================== DELETE (POST) ==================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (_dal.Delete(id))
            {
                _logDAL.InsertLog(
                    int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                    "Xóa tài khoản",
                    "Tài khoản",
                    $"Xóa user ID = {id}"
                );

                TempData["SuccessMessage"] = "Xóa tài khoản thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Xóa tài khoản thất bại.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
