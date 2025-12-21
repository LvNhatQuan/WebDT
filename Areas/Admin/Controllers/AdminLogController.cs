using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebDT.Areas.Admin.DAL;

namespace WebDT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class AdminLogController : Controller
    {
        private readonly AdminLogDAL _dal = new AdminLogDAL();

        public IActionResult Index()
        {
            return View(_dal.GetAll());
        }
    }
}
