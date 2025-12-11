using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebD_T.Models;
using WebDT.DAL;

namespace WebDT.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductDAL _productDAL = new ProductDAL();
        private readonly CategoryDAL _categoryDAL = new CategoryDAL();
        public IActionResult Index()
        {

            ViewBag.FeaturedProducts = _productDAL.GetFeaturedProducts(8);
            ViewBag.BestSellerProducts = _productDAL.GetBestSellerProducts(8);
            ViewBag.Categories = _categoryDAL.GetAllWithCount();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Route("/404")]
        public IActionResult PageNotfound()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
