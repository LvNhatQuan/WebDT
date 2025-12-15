using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebDT.DAL;
using WebDT.Models;

namespace WebDT.Controllers
{
    [Authorize] // Yêu cầu đăng nhập
    public class AddressController : Controller
    {
        private readonly AddressDAL _addressDAL;
        private readonly UserDAL _userDAL;

        public AddressController()
        {
            _addressDAL = new AddressDAL();
            _userDAL = new UserDAL();
        }
        public IActionResult Index()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return RedirectToAction("Login", "Account");

            var addresses = _addressDAL.GetAddressesByUserId(userId);

            // CHỈ giữ lại các thông báo từ AddressController, không lấy từ CartController
            // Kiểm tra nếu có thông báo thành công từ chính AddressController
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            // Kiểm tra nếu có thông báo lỗi từ chính AddressController
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();
            }

            // Nếu không có địa chỉ, đặt thông báo
            if (addresses.Count == 0)
            {
                ViewBag.NoAddressMessage = "Bạn chưa có địa chỉ giao hàng. Vui lòng thêm địa chỉ để có thể thanh toán.";
            }

            return View(addresses);
        }

        // GET: Address/Create - Hiển thị form thêm địa chỉ mới
        public IActionResult Create()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return RedirectToAction("Login", "Account");

            return View();
        }

        // POST: Address/Create - Xử lý thêm địa chỉ mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Address address)
        {
            if (ModelState.IsValid)
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return RedirectToAction("Login", "Account");

                address.user_id = userId;

                // Nếu là địa chỉ đầu tiên, đặt làm mặc định
                var userAddresses = _addressDAL.GetAddressesByUserId(userId);
                if (userAddresses.Count == 0)
                    address.is_default = true;

                bool success = _addressDAL.AddAddress(address);
                if (success)
                {
                    TempData["SuccessMessage"] = "Thêm địa chỉ thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Thêm địa chỉ thất bại!";
                }
            }
            return View(address);
        }

        // GET: Address/Edit/5 - Hiển thị form sửa địa chỉ
        public IActionResult Edit(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return RedirectToAction("Login", "Account");

            var address = _addressDAL.GetAddressById(id);

            // Kiểm tra quyền sở hữu
            if (address == null || address.user_id != userId)
            {
                TempData["ErrorMessage"] = "Không tìm thấy địa chỉ hoặc bạn không có quyền sửa!";
                return RedirectToAction("Index");
            }

            return View(address);
        }

        // POST: Address/Edit/5 - Xử lý sửa địa chỉ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Address address)
        {
            if (ModelState.IsValid)
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return RedirectToAction("Login", "Account");

                // Kiểm tra quyền sở hữu
                var existingAddress = _addressDAL.GetAddressById(id);
                if (existingAddress == null || existingAddress.user_id != userId)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy địa chỉ hoặc bạn không có quyền sửa!";
                    return RedirectToAction("Index");
                }

                address.id = id;
                address.user_id = userId;

                bool success = _addressDAL.UpdateAddress(address);
                if (success)
                {
                    TempData["SuccessMessage"] = "Cập nhật địa chỉ thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Cập nhật địa chỉ thất bại!";
                }
            }
            return View(address);
        }

        // POST: Address/Delete/5 - Xóa địa chỉ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return RedirectToAction("Login", "Account");

            // Kiểm tra quyền sở hữu
            var address = _addressDAL.GetAddressById(id);
            if (address == null || address.user_id != userId)
            {
                TempData["ErrorMessage"] = "Không tìm thấy địa chỉ hoặc bạn không có quyền xóa!";
                return RedirectToAction("Index");
            }

            // Không cho xóa nếu là địa chỉ mặc định
            if (address.is_default)
            {
                TempData["ErrorMessage"] = "Không thể xóa địa chỉ mặc định!";
                return RedirectToAction("Index");
            }

            bool success = _addressDAL.DeleteAddress(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Xóa địa chỉ thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Xóa địa chỉ thất bại!";
            }

            return RedirectToAction("Index");
        }

        // POST: Address/SetDefault/5 - Đặt địa chỉ làm mặc định
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetDefault(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return RedirectToAction("Login", "Account");

            // Kiểm tra quyền sở hữu
            var address = _addressDAL.GetAddressById(id);
            if (address == null || address.user_id != userId)
            {
                TempData["ErrorMessage"] = "Không tìm thấy địa chỉ!";
                return RedirectToAction("Index");
            }

            bool success = _addressDAL.SetDefaultAddress(userId, id);
            if (success)
            {
                TempData["SuccessMessage"] = "Đã đặt địa chỉ làm mặc định!";
            }
            else
            {
                TempData["ErrorMessage"] = "Thao tác thất bại!";
            }

            return RedirectToAction("Index");
        }

        // Helper method để lấy UserId hiện tại
        private int GetCurrentUserId()
        {
            // Sửa: Dùng ClaimTypes.NameIdentifier thay vì "UserId"
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return 0;
        }
    }
}