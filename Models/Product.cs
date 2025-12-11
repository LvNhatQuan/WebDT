using System.ComponentModel.DataAnnotations;

namespace WebDT.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int Stock_quantity { get; set; }
        public string Image_url { get; set; }
        public bool Is_active { get; set; }
        public DateTime Created_at { get; set; }
    }
    public class ProductPagination
    {
        public List<Product> Products { get; set; }
        public int CurrentPageIndex { get; set; }
        public int PageCount { get; set; }
    }
}
