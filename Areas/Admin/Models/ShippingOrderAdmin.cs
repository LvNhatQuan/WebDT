namespace WebDT.Areas.Admin.Models
{
    public class ShippingOrderAdmin
    {
        public int Id { get; set; }
        public string ReceiverName { get; set; } = "";
        public string ReceiverPhone { get; set; } = "";
        public string ShippingAddress { get; set; } = "";
        public decimal GrandTotal { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = "";
    }
}
