using System.ComponentModel.DataAnnotations;

namespace WebD_T.Areas.Admin.Models
{
    public class UserAdmin
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Username")]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Mật khẩu")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }  

        [Display(Name = "Họ và tên")]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Số điện thoại")]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn quyền")]
        [Display(Name = "Quyền")]
        public string Role { get; set; } = "customer";

          [Display(Name = "Trạng thái")]
        public bool IsLocked { get; set; }
    }
}
