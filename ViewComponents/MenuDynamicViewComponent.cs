using Microsoft.AspNetCore.Mvc;
using WebDT.DAL;
using WebDT.Models;

namespace WebDT.ViewComponents
{
    public class MenuDynamicViewComponent : ViewComponent
    {
        private readonly MenuDAL _menuDal = new MenuDAL();

        public IViewComponentResult Invoke()
        {
            var navMenu = _menuDal.GetNavbarMenu();
            return View("MenuDynamic", navMenu);
        }
    }
}
