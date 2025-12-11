using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebDT.Models
{
    public class Address
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Mã người dùng")]
        [Required(ErrorMessage = "*")]
        public int user_id { get; set; }

        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "*")]
        [MaxLength(255, ErrorMessage = "Tối đa 255 kí tự")]
        public string address_line { get; set; }

        [Display(Name = "Thành phố")]
        [MaxLength(100, ErrorMessage = "Tối đa 100 kí tự")]
        public string city { get; set; }

        [Display(Name = "Quận/Huyện")]
        [MaxLength(100, ErrorMessage = "Tối đa 100 kí tự")]
        public string district { get; set; }

        [Display(Name = "Số điện thoại người nhận")]
        [MaxLength(20, ErrorMessage = "Tối đa 20 kí tự")]
        public string phone_receiver { get; set; }

        [Display(Name = "Mặc định")]
        public bool is_default { get; set; }
    }
}
