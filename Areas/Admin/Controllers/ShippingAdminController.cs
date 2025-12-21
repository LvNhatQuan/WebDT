using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebDT.Areas.Admin.DAL;

namespace WebDT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin,staff")]
    public class ShippingAdminController : Controller
    {
        private readonly ShippingAdminDAL _dal = new ShippingAdminDAL();
        private readonly AdminLogDAL _logDAL = new AdminLogDAL();

        // ================== INDEX ==================
        public IActionResult Index()
        {
            return View(_dal.GetShippingOrders());
        }

        // ================== UPDATE STATUS ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStatus(int id, string status)
        {
            bool ok = _dal.UpdateStatus(id, status);

            if (ok)
            {
                _logDAL.InsertLog(
                    int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                    "Cập nhật giao hàng",
                    "Giao hàng",
                    $"Đơn hàng ID = {id} → trạng thái {status}"
                );

                TempData["SuccessMessage"] = "Cập nhật trạng thái giao hàng thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Cập nhật trạng thái giao hàng thất bại.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
