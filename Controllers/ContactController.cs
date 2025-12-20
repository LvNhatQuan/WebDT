using Microsoft.AspNetCore.Mvc;

namespace WebDT.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
