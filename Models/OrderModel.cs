namespace WebDT.Models
{
    public class OrderModel
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public decimal SubTotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Discount { get; set; }
        public decimal GrandTotal { get; set; }

        public string ShippingAddress { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = "pending";

        // ⭐ Danh sách item
        public List<OrderItemModel> Items { get; set; } = new();
    }

    public class OrderItemModel
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        // ✅ coupon_id ĐÃ CHUYỂN SANG order_items
        public int? CouponId { get; set; }

        public int? ProductId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Img { get; set; } = string.Empty;

        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }
}
