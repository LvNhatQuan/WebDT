using System.ComponentModel.DataAnnotations;

namespace WebDT.Areas.Admin.Models
{
    public class StaffAdmin
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Họ tên")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Display(Name = "Mật khẩu")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public bool IsLocked { get; set; }
    }
}
