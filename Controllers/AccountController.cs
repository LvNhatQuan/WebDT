using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebDT.DAL;
using WebDT.Models;

namespace WebDT.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserDAL _userDal;

        public AccountController(UserDAL userDal)
        {
            _userDal = userDal;
        }

        [Authorize]
        public IActionResult Profile()
        {
            string username = User.Identity?.Name ?? "";

            var user = _userDal.GetUserByUsername(username);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản.";
                return RedirectToAction("Login");
            }

            return View(user);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Profile(User model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var oldUser = _userDal.GetUserById(model.Id);
            if (oldUser == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản.";
                return RedirectToAction("Login");
            }

            oldUser.FullName = model.FullName;
            oldUser.Email = model.Email;
            oldUser.PhoneNumber = model.PhoneNumber;

            bool ok = _userDal.UpdateProfile(oldUser);

            if (ok)
                TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
            else
                TempData["ErrorMessage"] = "Cập nhật thất bại.";

            return View(oldUser);
        }


        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = _userDal.GetUserByEmail(email);

            if (user == null)
            {
                TempData["Error"] = "Email không tồn tại!";
                return View();
            }

            if (user.Password != password)   // So sánh plain text
            {
                TempData["Error"] = "Sai mật khẩu!";
                return View();
            }

            // QUAN TRỌNG: Thêm ClaimTypes.NameIdentifier
            var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),   // ⭐ bắt buộc
    new Claim(ClaimTypes.Name, user.Email),
    new Claim(ClaimTypes.Role, user.Role)
};

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity)
            );


            // Lưu thông tin user vào session (tuỳ chọn nhưng hữu ích)
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.Username);

            if (user.Role == "admin")
                return RedirectToAction("Index", "ProductAdmin", new { area = "Admin" });

            if (user.Role == "staff")
                return RedirectToAction("Index", "StaffDashboard");

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(User model)
        {
            if (_userDal.CreateUser(model))
                return RedirectToAction("Login");

            TempData["Error"] = "Đăng ký thất bại!";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Account");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
