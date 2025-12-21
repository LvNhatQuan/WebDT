namespace WebDT.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }

        public string UserName { get; set; } = "";
        public int Rating { get; set; }
        public string Comment { get; set; } = "";

        public DateTime CreatedAt { get; set; }
    }
}
