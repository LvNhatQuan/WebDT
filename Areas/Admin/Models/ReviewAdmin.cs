namespace WebDT.Areas.Admin.Models
{
    public class ReviewAdmin
    {
        public int Id { get; set; }

        public string ProductName { get; set; } = "";
        public string CustomerName { get; set; } = "";

        public int Rating { get; set; }
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
