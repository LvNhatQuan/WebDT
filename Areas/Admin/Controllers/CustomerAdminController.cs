using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebD_T.Areas.Admin.DAL;

namespace WebD_T.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class CustomerAdminController : Controller
    {
        private readonly CustomerAdminDAL _dal;

        public CustomerAdminController()
        {
            _dal = new CustomerAdminDAL();
        }

        public IActionResult Index()
        {
            var list = _dal.GetAllCustomers();
            return View(list);
        }

        public IActionResult Details(int id)
        {
            var customer = _dal.GetCustomerById(id);
            if (customer == null)
            {
                TempData["Error"] = "Không tìm thấy khách hàng.";
                return RedirectToAction("Index");
            }

            ViewBag.Orders = _dal.GetCustomerOrders(id);

            return View(customer);
        }
    }
}
