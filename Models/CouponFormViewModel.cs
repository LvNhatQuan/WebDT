using System.ComponentModel.DataAnnotations;

namespace WebDT.Areas.Admin.Models
{
    public class CouponFormViewModel
    {
        public int Id { get; set; }  // 0 = Create, >0 = Edit

        [Required(ErrorMessage = "Vui lòng nhập tên sự kiện")]
        [Display(Name = "Tên sự kiện")]
        [MaxLength(100, ErrorMessage = "Tên sự kiện không được vượt quá 100 ký tự")]
        public string EventName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập giá trị giảm giá")]
        [Display(Name = "Giá trị giảm giá (%)")]
        [Range(1, 100, ErrorMessage = "Giá trị giảm giá phải từ 1 đến 100%")]
        public int DiscountValue { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        [Display(Name = "Ngày bắt đầu")]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        [Display(Name = "Ngày kết thúc")]
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Trạng thái")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Chọn sản phẩm áp dụng")]
        public List<int>? SelectedProductIds { get; set; }

        public List<ProductAdmin> AllProducts { get; set; } = new List<ProductAdmin>();

        public int CurrentProductPage { get; set; } = 1;
        public int ProductPageSize { get; set; } = 10;
        public int TotalProductPages { get; set; }
        public int TotalProducts { get; set; }


        // Helper properties
        public bool IsCreate => Id == 0;
        public bool IsEdit => Id > 0;
        public string ActionTitle => IsCreate ? "Tạo mã giảm giá" : "Chỉnh sửa mã giảm giá";
    }
}
