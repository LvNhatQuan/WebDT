namespace WebDT.Models
{
    public class CheckoutViewModel
    {
        public List<CartItem> CartItems { get; set; } = new();

        // ====== THÔNG TIN NHẬN HÀNG ======
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;

        // ====== GIÁ TIỀN ======
        public decimal ShippingFee { get; set; }
        public decimal Discount { get; set; }

        public decimal SubTotal => CartItems.Sum(x => x.Total);
        public decimal GrandTotal => SubTotal + ShippingFee - Discount;
    }
}
