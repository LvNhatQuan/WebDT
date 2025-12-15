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

            TempData[ok ? "SuccessMessage" : "ErrorMessage"] = ok
                ? "Cập nhật thông tin thành công!"
                : "Cập nhật thất bại.";

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

            if (!user.IsActive)
            {
                TempData["Error"] = "Tài khoản đã bị vô hiệu hóa!";
                return View();
            }

            if (user.IsLocked)
            {
                TempData["Error"] = "Tài khoản đã bị khóa!";
                return View();
            }

            if (!VerifyPassword(user.Password, password))
            {
                TempData["Error"] = "Sai mật khẩu!";
                return View();
            }

            // ⭐ Claims chuẩn
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),     // ⭐ MUST be Username để Profile load đúng
                new Claim(ClaimTypes.Role, user.Role ?? "customer")
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity)
            );

            // Optional session
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

        public IActionResult AccessDenied() => View();

        // =========================
        // Password verify (plain + bcrypt nếu có thư viện)
        // =========================
        private bool VerifyPassword(string stored, string input)
        {
            stored ??= "";
            input ??= "";

            // plain text
            if (!stored.StartsWith("$2")) return stored == input;

            // bcrypt (nếu project có BCrypt.Net-Next)
            try
            {
                var t = Type.GetType("BCrypt.Net.BCrypt, BCrypt.Net-Next");
                if (t == null) return false;

                var m = t.GetMethod("Verify", new[] { typeof(string), typeof(string) });
                if (m == null) return false;

                var ok = (bool)m.Invoke(null, new object[] { input, stored })!;
                return ok;
            }
            catch
            {
                return false;
            }
        }
    }
}
