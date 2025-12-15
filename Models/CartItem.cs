namespace WebDT.Models
{
    public class CartItem
    {
        public int IdProduct { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Img { get; set; } = string.Empty;

        public decimal Price { get; set; }
        public int Quantity { get; set; }

        // ✅ coupon gắn theo từng item
        public int? CouponId { get; set; }

        // % giảm giá (lấy từ coupon.discount_value)
        public decimal Discount { get; set; }

        public decimal Total
        {
            get
            {
                var total = Price * Quantity;
                if (Discount > 0)
                    total -= total * Discount / 100;
                return total;
            }
        }
    }
}
