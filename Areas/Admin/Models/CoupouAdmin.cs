using System.ComponentModel.DataAnnotations;

namespace WebDT.Areas.Admin.Models
{
    public class CouponAdmin
    {
        public int Id { get; set; }

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
    }

}
