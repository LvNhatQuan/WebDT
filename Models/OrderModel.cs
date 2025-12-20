namespace WebDT.Models
{
    public class OrderModel
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        // ===== THÔNG TIN NGƯỜI NHẬN =====
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;

        // ===== GIÁ TIỀN =====
        public decimal SubTotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Discount { get; set; }
        public decimal GrandTotal { get; set; }

        // ===== GIAO HÀNG =====
        public string ShippingAddress { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = "pending";

        // ===== DANH SÁCH SẢN PHẨM =====
        public List<OrderItemModel> Items { get; set; } = new();
    }

    public class OrderItemModel
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        // coupon gắn theo item
        public int? CouponId { get; set; }

        public int? ProductId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Img { get; set; } = string.Empty;

        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }
}
