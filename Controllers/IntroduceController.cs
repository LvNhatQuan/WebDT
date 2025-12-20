using Microsoft.AspNetCore.Mvc;

namespace WebDT.Controllers
{
    public class IntroduceController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }
    }
}