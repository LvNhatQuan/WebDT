using System.ComponentModel.DataAnnotations;

namespace WebDT.Models
{
    public class CategoryMenu
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        public int Count { get; set; }
    }
}
