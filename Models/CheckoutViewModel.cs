using WebDT.Models;

namespace WebDT.Models
{
    public class CheckoutViewModel
    {
        public List<CartItem> CartItems { get; set; } = new();

        public decimal SubTotal =>
            CartItems.Sum(x => x.Total);

        public decimal ShippingFee { get; set; } = 0;
        public decimal Discount { get; set; } = 0;

        public decimal GrandTotal =>
            SubTotal + ShippingFee - Discount;
    }
}
