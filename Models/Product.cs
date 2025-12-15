namespace WebDT.Models
{
    public class Product
    {
        public int Id { get; set; }
        public int? CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public int? CouponId { get; set; }

        public decimal Discount { get; set; } // % từ coupon.discount_value

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }
}
