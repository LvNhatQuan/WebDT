namespace WebDT.Models
{
    public class CheckoutViewModel
    {
        public List<CartItem> CartItems { get; set; } = new();

        public decimal ShippingFee { get; set; }
        public decimal Discount { get; set; }

        public decimal SubTotal => CartItems.Sum(x => x.Total);
        public decimal GrandTotal => SubTotal + ShippingFee - Discount;

        public string ShippingAddress { get; set; } = string.Empty;
    }
}
