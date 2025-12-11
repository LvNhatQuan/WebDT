using Microsoft.AspNetCore.Mvc;
using WebDT.DAL;
using WebDT.Models;

namespace WebD_T.ViewComponents
{
    public class MenuDynamicViewComponent : ViewComponent
    {
        private readonly MenuDAL _menuDal = new MenuDAL();

        public IViewComponentResult Invoke()
        {
            // 1. Lấy toàn bộ menu từ DB
            List<MenuItem> listMenu = _menuDal.GetAllMenu();

            // 2. Lọc bỏ các menu không hiển thị
            listMenu = listMenu.Where(m => m.isVisible).ToList();

            // 3. Build danh sách NavbarItem (cha + con)
            var navBar = new List<NavbarItem>();

            // Parent items (ParentId == null)
            foreach (var item in listMenu.Where(m => m.ParentId == null))
            {
                navBar.Add(new NavbarItem
                {
                    Id = item.Id,
                    ParentId = item.ParentId,
                    Title = item.Title,
                    MenuUrl = item.MenuUrl,
                    MenuIndex = item.MenuIndex,
                    isVisible = item.isVisible,
                    subItems = new List<NavbarItem>()
                });
            }

            // Child items (dropdown)
            foreach (var item in listMenu.Where(m => m.ParentId != null))
            {
                var parent = navBar.FirstOrDefault(p => p.Id == item.ParentId);
                if (parent != null)
                {
                    parent.subItems!.Add(new NavbarItem
                    {
                        Id = item.Id,
                        ParentId = item.ParentId,
                        Title = item.Title,
                        MenuUrl = item.MenuUrl,
                        MenuIndex = item.MenuIndex,
                        isVisible = item.isVisible,
                        subItems = null
                    });
                }
            }

            // View: Views/Shared/Components/MenuDynamic/MenuDynamic.cshtml
            return View("MenuDynamic", navBar);
        }
    }
}
