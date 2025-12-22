using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebDT.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = "staff")]
    public class StaffHomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
