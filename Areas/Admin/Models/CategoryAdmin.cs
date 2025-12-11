using System.ComponentModel.DataAnnotations;

namespace WebDT.Areas.Admin.Models
{
    public class CategoryAdmin
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "*")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }

}
