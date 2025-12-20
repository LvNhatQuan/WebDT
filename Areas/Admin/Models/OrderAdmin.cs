namespace WebDT.Areas.Admin.Models
{
    public class OrderAdmin
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }

        // ===== THÔNG TIN NGƯỜI NHẬN (MỚI) =====
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; }

        public decimal SubTotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal { get; set; }

        public string ShippingAddress { get; set; } = string.Empty;

        // ===== TRẠNG THÁI (MỚI) =====
        public string Status { get; set; } = string.Empty;

        public List<OrderItemAdmin> Items { get; set; } = new();
    }

    public class OrderItemAdmin
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
