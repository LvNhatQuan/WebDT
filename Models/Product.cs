namespace WebDT.Models
{
    public class Product
    {
        public int Id { get; set; }

        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public string ImageUrl
        {
            get => Image_url;
            set => Image_url = value;
        }
        public string Image_url { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        // Thêm thuộc tính Discount với giá trị mặc định là 0
        public decimal Discount { get; set; } = 0;

       

        // Thuộc tính tính toán để lấy giá sau giảm
        public decimal PriceAfterDiscount => Price - (Price * Discount / 100);
    }
    public class ProductPagination
    {
        public List<Product> Products { get; set; }
        public int CurrentPageIndex { get; set; }
        public int PageCount { get; set; }
    }
}
