using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebDT.Areas.Admin.DAL;
using WebDT.Areas.Admin.Models;

namespace WebDT.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = "staff")]
    public class StaffOrderController : Controller
    {
        private readonly OrderAdminDAL _orderDal = new OrderAdminDAL();

        // =========================
        // DANH SÁCH ĐƠN HÀNG
        // =========================
        public IActionResult Index()
        {
            var orders = _orderDal.GetAll();
            return View(orders);
        }

        // =========================
        // CHI TIẾT ĐƠN HÀNG
        // =========================
        public IActionResult Details(int id)
        {
            var order = _orderDal.GetById(id);
            if (order == null)
                return RedirectToAction(nameof(Index));

            return View(order);
        }

        // =========================
        // XÁC NHẬN ĐƠN
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Confirm(int id)
        {
            _orderDal.UpdateStatus(id, "confirmed");
            TempData["SuccessMessage"] = "Đã xác nhận đơn hàng";
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // CHUYỂN SANG ĐANG GIAO
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Shipping(int id)
        {
            _orderDal.UpdateStatus(id, "shipping");
            TempData["SuccessMessage"] = "Đơn hàng đang được giao";
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // HỦY ĐƠN (STAFF)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(int id)
        {
            _orderDal.UpdateStatus(id, "cancelled");
            TempData["SuccessMessage"] = "Đã hủy đơn hàng";
            return RedirectToAction(nameof(Index));
        }
    }
}
