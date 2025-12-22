using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebDT.Areas.Admin.DAL;

namespace WebDT.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = "staff")]
    public class StaffShippingController : Controller
    {
        private readonly OrderAdminDAL _dal = new OrderAdminDAL();

        // ===============================
        // DANH SÁCH ĐƠN ĐANG GIAO
        // ===============================
        public IActionResult Index()
        {
            var orders = _dal.GetAll()
                             .Where(o => o.Status == "shipping")
                             .ToList();

            return View(orders);
        }

        // ===============================
        // CHI TIẾT ĐƠN GIAO
        // ===============================
        public IActionResult Details(int id)
        {
            var order = _dal.GetById(id);
            if (order == null)
                return RedirectToAction(nameof(Index));

            return View(order);
        }

        // ===============================
        // XÁC NHẬN ĐÃ GIAO HÀNG
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Complete(int id)
        {
            _dal.UpdateStatus(id, "completed");
            return RedirectToAction(nameof(Index));
        }
    }
}
