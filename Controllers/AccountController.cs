using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebD_T.DAL;
using WebD_T.Models;

namespace WebD_T.Controllers
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

            if (user.Password != password)   
            {
                TempData["Error"] = "Sai mật khẩu!";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("FullName", user.FullName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal);

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
