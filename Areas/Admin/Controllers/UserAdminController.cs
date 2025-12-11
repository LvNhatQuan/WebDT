using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

            var model = new UserAdmin
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber,
                Role = u.Role,
                Password = u.Password
            };

            return View(model);
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

            bool ok = _dal.Create(user);

            if (ok)
            {
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

            var model = new UserAdmin
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber,
                Role = u.Role,
                Password = u.Password
            };

            return View(model);
        }

        // ================== EDIT (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, UserAdmin model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // lấy user cũ
            var oldUser = _dal.GetById(id);
            if (oldUser == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản.";
                return RedirectToAction(nameof(Index));
            }
            string passwordToSave = string.IsNullOrEmpty(model.Password)
                ? oldUser.Password
                : model.Password;

            var user = new User
            {
                Id = id,
                Username = model.Username,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                Role = model.Role,
                Password = passwordToSave       
            };

            bool ok = _dal.Update(user);

            if (ok)
            {
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

            var model = new UserAdmin
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber,
                Role = u.Role
            };

            return View(model);
        }

        // ================== DELETE (POST) ==================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            bool ok = _dal.Delete(id);

            if (ok)
            {
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
