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

        // ===================== PROFILE =====================
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
        [ValidateAntiForgeryToken]
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

            TempData[ok ? "SuccessMessage" : "ErrorMessage"] =
                ok ? "Cập nhật thông tin thành công!" : "Cập nhật thất bại.";

            return View(oldUser);
        }

        // ===================== LOGIN =====================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

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

            // ================= CLAIMS CHUẨN =================
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),   // BẮT BUỘC để Profile load đúng
                new Claim(ClaimTypes.Role, user.Role ?? "customer")
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity)
            );

            // Optional session
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.Username);

            // ================= REDIRECT THEO ROLE =================
            if (user.Role == "admin")
            {
                return RedirectToAction(
                    "Index",
                    "ProductAdmin",
                    new { area = "Admin" }
                );
            }

            if (user.Role == "staff")
            {
                return RedirectToAction(
                    "Index",
                    "StaffHome",
                    new { area = "Staff" }
                );
            }

            // customer
            return RedirectToAction("Index", "Home");
        }

        // ===================== REGISTER =====================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.Role = "customer";
            model.IsActive = true;
            model.IsLocked = false;
            model.CreatedAt = DateTime.Now;

            if (_userDal.CreateUser(model))
                return RedirectToAction("Login");

            TempData["Error"] = "Email hoặc Username đã tồn tại!";
            return View(model);
        }

        // ===================== LOGOUT =====================
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            return RedirectToAction("Login");
        }

        // ===================== ACCESS DENIED =====================
        public IActionResult AccessDenied()
        {
            return View();
        }

        // ===================== PASSWORD VERIFY =====================
        private bool VerifyPassword(string stored, string input)
        {
            stored ??= "";
            input ??= "";

            // plain text
            if (!stored.StartsWith("$2"))
                return stored == input;

            // bcrypt (nếu có BCrypt.Net-Next)
            try
            {
                var t = Type.GetType("BCrypt.Net.BCrypt, BCrypt.Net-Next");
                if (t == null) return false;

                var m = t.GetMethod("Verify", new[] { typeof(string), typeof(string) });
                if (m == null) return false;

                return (bool)m.Invoke(null, new object[] { input, stored })!;
            }
            catch
            {
                return false;
            }
        }
    }
}
