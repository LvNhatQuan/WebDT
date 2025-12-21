namespace WebDT.Areas.Admin.Models
{
    public class ProductStatisticVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public int TotalQuantity { get; set; }
        public decimal Revenue { get; set; }
    }
}
