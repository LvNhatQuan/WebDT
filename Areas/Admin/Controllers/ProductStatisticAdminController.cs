using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WebDT.Areas.Admin.DAL;

namespace WebDT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class ProductStatisticAdminController : Controller
    {
        private readonly ProductStatisticDAL _dal = new ProductStatisticDAL();

        // ================== INDEX ==================
        public IActionResult Index()
        {
            var data = _dal.GetProductStatistics() ?? new List<dynamic>();

            ViewBag.TotalProducts = data.Count;
            ViewBag.TotalRevenue = data.Sum(x => (decimal)x.Revenue);
            ViewBag.BestSeller = data
                .OrderByDescending(x => x.TotalQuantity)
                .FirstOrDefault();

            return View(data);
        }
    }
}
