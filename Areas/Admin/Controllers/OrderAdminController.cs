using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebDT.Areas.Admin.DAL;

namespace WebDT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class OrderAdminController : Controller
    {
        private readonly OrderAdminDAL _orderDal = new OrderAdminDAL();
        private readonly AdminLogDAL _logDAL = new AdminLogDAL();

        // ================== INDEX ==================
        public IActionResult Index()
        {
            return View(_orderDal.GetAll());
        }

        // ================== DETAILS ==================
        public IActionResult Details(int id)
        {
            var order = _orderDal.GetById(id);
            if (order == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }

        // ================== DELETE (GET) ==================
        public IActionResult Delete(int id)
        {
            var order = _orderDal.GetById(id);
            if (order == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }

        // ================== DELETE (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _orderDal.DeleteReviewsByOrderId(id);

            bool ok = _orderDal.Delete(id);

            if (ok)
            {
                _logDAL.InsertLog(
                    int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                    "Xóa đơn hàng",
                    "Đơn hàng",
                    $"Xóa đơn hàng ID = {id}"
                );

                TempData["SuccessMessage"] = "Xóa đơn hàng thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa đơn hàng.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ================== EDIT (OPTIONAL) ==================
        public IActionResult Edit(int id)
        {
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
