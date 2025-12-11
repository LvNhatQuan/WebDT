using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebDT.Areas.Admin.DAL;
using WebDT.Areas.Admin.Models;

namespace WebDT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class OrderAdminController : Controller
    {
        private readonly OrderAdminDAL _orderDal = new OrderAdminDAL();

        // =======================================
        // INDEX – DANH SÁCH ĐƠN HÀNG
        // =======================================
        public IActionResult Index()
        {
            var orders = _orderDal.GetAll();
            return View(orders);
        }

        // =======================================
        // DETAILS – XEM CHI TIẾT ĐƠN HÀNG
        // =======================================
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

        // =======================================
        // DELETE (GET) – XÁC NHẬN XÓA
        // =======================================
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

        // =======================================
        // DELETE (POST) – XÓA ĐƠN HÀNG
        // =======================================
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            // 1) Xóa reviews trước
            _orderDal.DeleteReviewsByOrderId(id);

            // 2) Xóa đơn hàng
            bool ok = _orderDal.Delete(id);

            if (ok)
                TempData["SuccessMessage"] = "Xóa đơn hàng thành công!";
            else
                TempData["ErrorMessage"] = "Không thể xóa đơn hàng.";

            return RedirectToAction(nameof(Index));
        }


        // =======================================
        // EDIT STATUS (TÙY CHỌN – NẾU CÓ STATUS)
        // =======================================
        public IActionResult Edit(int id)
        {
            // Chỉ hoạt động nếu sau này bạn thêm cột 'status'
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
