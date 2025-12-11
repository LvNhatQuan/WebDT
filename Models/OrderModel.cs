namespace WebDT.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CouponId { get; set; }
        public decimal SubTotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Discount { get; set; }
        public decimal GrandTotal { get; set; }
        public string ShippingAddress { get; set; }
        public DateTime OrderDate { get; set; }

        // ⭐ rất quan trọng
        public List<OrderItemModel> Items { get; set; } = new();
    }

    public class OrderItemModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }
}
