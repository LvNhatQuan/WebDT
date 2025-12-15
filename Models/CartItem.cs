namespace WebDT.Models
{
    public class CartItem
    {
        public int IdProduct { get; set; }
        public int? IdCoupon { get; set; }
        public decimal Discount { get; set; } // Đổi từ CouponValue sang Discount
        public string Name { get; set; }
        public string Img { get; set; }
        public decimal Price { get; set; } // Đổi từ int sang decimal
        public double Rate { get; set; }
        public int Quantity { get; set; }
        public decimal Total => Price * Quantity * (1 - Discount / 100); // Tính giá sau giảm
    }
}
