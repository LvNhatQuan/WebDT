using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebD_T.Areas.Admin.DAL;
using WebD_T.Areas.Admin.Models;
using WebD_T.Areas.DAL;
using WebD_T.Helper;

namespace WebD_T.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class ProductAdminController : Controller
    {
        ProductAdminDAL productDAL = new ProductAdminDAL();
        CategoryAdminDAL categoryDAL = new CategoryAdminDAL();

        // ==================== DANH SÁCH ====================
        public ActionResult Index()
        {
            var products = productDAL.getAll();
            return View(products);
        }

        // ==================== CHI TIẾT ====================
        public ActionResult Details(int id)
        {
            var product = productDAL.GetProductById(id);

            if (product == null || product.Id == 0)
            {
                TempData["ErrorMessage"] = "Không tìm thấy sản phẩm.";
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        // ==================== CREATE (GET) ====================
        public ActionResult Create()
        {
            // Lấy danh sách Category từ DB
            var categories = categoryDAL.getAll();

            // Khai báo model form
            var model = new ProductFormAdmin();

            // Đổ data cho dropdown
            model.ListCategory = categories
                .Select(item => new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                })
                .ToList();

            // Giá trị mặc định
            model.IsActive = true;
            model.StockQuantity = 0;

            return View(model);
        }

        // ==================== CREATE (POST) ====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductFormAdmin productAddNew, IFormFile Img)
        {
            try
            {
                // 1. Map CategoryId từ dropdown TRƯỚC
                if (productAddNew.CategoryIdSelected.HasValue)
                {
                    productAddNew.CategoryId = productAddNew.CategoryIdSelected.Value;
                }
                else
                {
                    ModelState.AddModelError("CategoryIdSelected", "Vui lòng chọn danh mục");
                }

                // 2. Xoá lỗi mặc định của CategoryId (vì mình đã tự gán)
                ModelState.Remove(nameof(productAddNew.CategoryId));

                // 3. Kiểm tra ModelState sau khi đã map CategoryId
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Dữ liệu chưa hợp lệ, vui lòng kiểm tra lại.";

                    var categories = categoryDAL.getAll();
                    productAddNew.ListCategory = categories
                        .Select(item => new SelectListItem
                        {
                            Text = item.Name,
                            Value = item.Id.ToString()
                        })
                        .ToList();

                    return View(productAddNew);
                }

                
                productAddNew.CreatedAt = DateTime.Now;

                if (Img == null || Img.Length == 0)
                {
                    productAddNew.ImageUrl = string.Empty;
                }
                else
                {
                    var imageName = ImageHelper.UpLoadImage(Img, "SanPham");
                    productAddNew.ImageUrl = imageName;
                }

                bool isInserted = productDAL.AddNew(productAddNew);

                if (isInserted)
                {
                    TempData["SuccessMessage"] = "Thêm sản phẩm thành công";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể thêm sản phẩm";

                    var categories = categoryDAL.getAll();
                    productAddNew.ListCategory = categories
                        .Select(item => new SelectListItem
                        {
                            Text = item.Name,
                            Value = item.Id.ToString()
                        })
                        .ToList();

                    return View(productAddNew);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi: " + ex.Message;

                var categories = categoryDAL.getAll();
                productAddNew.ListCategory = categories
                    .Select(item => new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    })
                    .ToList();

                return View(productAddNew);
            }
        }

        // ==================== EDIT (GET) ====================
        public ActionResult Edit(int id)
        {
            var product = productDAL.GetProductById(id);

            if (product == null || product.Id == 0)
            {
                TempData["ErrorMessage"] = "Không tìm thấy sản phẩm.";
                return RedirectToAction(nameof(Index));
            }

            var model = new ProductFormAdmin
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                CategoryId = product.CategoryId,
                CategoryName = product.CategoryName,
                CategoryIdSelected = product.CategoryId
            };

            var categories = categoryDAL.getAll();
            model.ListCategory = categories
                .Select(item => new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString(),
                    Selected = (item.Id == product.CategoryId)
                })
                .ToList();

            return View(model);
        }

        // ==================== EDIT (POST) ====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ProductFormAdmin productEdit, IFormFile? ImageUpload)
        {
            try
            {
                // 1. Map CategoryId từ dropdown
                if (productEdit.CategoryIdSelected.HasValue)
                {
                    productEdit.CategoryId = productEdit.CategoryIdSelected.Value;
                }
                else
                {
                    ModelState.AddModelError("CategoryIdSelected", "Vui lòng chọn danh mục");
                }

                // 2. Xoá lỗi cũ của CategoryId
                ModelState.Remove(nameof(productEdit.CategoryId));

                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Dữ liệu chưa hợp lệ, vui lòng kiểm tra lại.";

                    var categories = categoryDAL.getAll();
                    productEdit.ListCategory = categories
                        .Select(item => new SelectListItem
                        {
                            Text = item.Name,
                            Value = item.Id.ToString(),
                            Selected = (item.Id == productEdit.CategoryId)
                        })
                        .ToList();

                    return View(productEdit);
                }

                // 3. Lấy dữ liệu cũ để giữ CreatedAt + Image nếu không upload mới
                var oldProduct = productDAL.GetProductById(id);
                if (oldProduct == null || oldProduct.Id == 0)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy sản phẩm.";
                    return RedirectToAction(nameof(Index));
                }

                // 4. Xử lý ảnh
                if (ImageUpload != null && ImageUpload.Length > 0)
                {
                    var imageName = ImageHelper.UpLoadImage(ImageUpload, "SanPham");
                    productEdit.ImageUrl = imageName;
                }
                else
                {
                    productEdit.ImageUrl = oldProduct.ImageUrl;
                }

                // 5. Giữ nguyên CreatedAt cũ
                productEdit.CreatedAt = oldProduct.CreatedAt;
                productEdit.Id = id;

                // 6. Cập nhật DB
                bool isUpdated = productDAL.UpdateProduct(productEdit, id);

                if (isUpdated)
                {
                    TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Cập nhật thất bại";

                    var categories = categoryDAL.getAll();
                    productEdit.ListCategory = categories
                        .Select(item => new SelectListItem
                        {
                            Text = item.Name,
                            Value = item.Id.ToString(),
                            Selected = (item.Id == productEdit.CategoryId)
                        })
                        .ToList();

                    return View(productEdit);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi cập nhật: " + ex.Message;

                var categories = categoryDAL.getAll();
                productEdit.ListCategory = categories
                    .Select(item => new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString(),
                        Selected = (item.Id == productEdit.CategoryId)
                    })
                    .ToList();

                return View(productEdit);
            }
        }

        // ==================== DELETE (GET) ====================
        public ActionResult Delete(int id)
        {
            var product = productDAL.GetProductById(id);

            if (product == null || product.Id == 0)
            {
                TempData["ErrorMessage"] = "Không tìm thấy sản phẩm.";
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        // ==================== DELETE (POST) ====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                var isSuccess = productDAL.DeleteProduct(id);

                if (isSuccess)
                {
                    TempData["SuccessMessage"] = "Xóa sản phẩm thành công";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Xóa sản phẩm thất bại";

                    var product = productDAL.GetProductById(id);
                    return View(product);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi xóa sản phẩm: " + ex.Message;

                var product = productDAL.GetProductById(id);
                return View(product);
            }
        }
    }
}
