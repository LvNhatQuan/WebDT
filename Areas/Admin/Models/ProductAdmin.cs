using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebD_T.Areas.Admin.Models
{
    public class ProductAdmin
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Tên sản phẩm")]
        [Required(ErrorMessage = "*")]
        [MaxLength(200, ErrorMessage = "Tối đa 200 kí tự")]
        public string Name { get; set; } = string.Empty;  

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Display(Name = "Ảnh")]
        [MaxLength(255)]
        public string? ImageUrl { get; set; }

        [Display(Name = "Giá")]
        [Required(ErrorMessage = "*")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải >= 0")]
        public decimal Price { get; set; }

        [Display(Name = "Số lượng tồn")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải >= 0")]
        public int StockQuantity { get; set; }

        [Display(Name = "Đang hoạt động")]
        public bool IsActive { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Danh mục")]
        [Required(ErrorMessage = "*")]
        public int CategoryId { get; set; }

        [NotMapped]
        public string? CategoryName { get; set; }
    }

    public class ProductFormAdmin : ProductAdmin
    {
        public int? CategoryIdSelected { get; set; }

        public List<SelectListItem>? ListCategory { get; set; }
    }
}
