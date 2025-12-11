using Microsoft.AspNetCore.Mvc;
using WebDT.DAL;
using WebDT.Models;

namespace WebDT.ViewComponents
{
    public class MenuCategoryViewComponent : ViewComponent
    {
        private readonly CategoryDAL _categoryDal = new CategoryDAL();

        public IViewComponentResult Invoke()
        {
            List<CategoryMenu> categoryMenus = _categoryDal.GetAllWithCount();
            return View("Default", categoryMenus);   // View: Views/Shared/Components/MenuCategory/Default.cshtml
        }
    }
}
